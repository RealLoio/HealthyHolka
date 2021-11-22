using HealthyHolka.Models;
using HealthyHolka.DataContext;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HealthyHolka.Controllers
{
    [Route("api/[controller]/shift")]
    [ApiController]
    public class KppController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public KppController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("start/{employeeId}")]
        public async Task<ActionResult> StartShift(int employeeId, [FromQuery] DateTime startTime)
        {
            Employee employee = await _context.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return BadRequest($"Employee with id:{employeeId} was not found!");
            }

            Shift openedShift = _context.Shifts
                .Where(s => s.EmployeeId == employeeId)
                .Where(s => s.End == null)
                .FirstOrDefault();
            if (openedShift is not null)
            {
                return BadRequest($"Can't start shift for employee with id:{employeeId}, there's an open shift from {openedShift.Start}!");
            }

            Shift newShift = new Shift()
            {
                EmployeeId = employeeId,
                Start = startTime
            };

            Position position = await _context.Positions.FindAsync(employee.PositionId);
            DateTime requiredStartTime = startTime.Date.Add(position.StartingHour);
            if (startTime.CompareTo(requiredStartTime) > 0)
            {
                newShift.TimesViolated++;
            }

            _context.Shifts.Add(newShift);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("end/{employeeId}")]
        public async Task<ActionResult> EndShift(int employeeId, [FromQuery] DateTime endTime)
        {
            Employee employee = await _context.Employees.FindAsync(employeeId);
            if (employee is null)
            {
                return BadRequest($"Employee with id:{employeeId} was not found!");
            }

            Shift openedShift = _context.Shifts
                .Where(s => s.EmployeeId == employeeId)
                .Where(s => s.End == null)
                .FirstOrDefault();
            if (openedShift is null)
            {
                return BadRequest($"Employee with id:{employeeId} has no shifts to end!");
            }

            openedShift.End = endTime;
            openedShift.HoursWorked = (int)endTime.Subtract(openedShift.Start).TotalHours;

            Position position = await _context.Positions.FindAsync(employee.PositionId);
            DateTime requiredEndTime = openedShift.Start.Date
                .Add(position.StartingHour)
                .Add(position.RequiredWorkHours);
            if (endTime.CompareTo(requiredEndTime) < 0)
            {
                openedShift.TimesViolated++;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}