using HealthyHolka.Models;
using HealthyHolka.DataContext;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            if (employee is null || employee.IsDeleted)
            {
                return BadRequest($"Employee with id:{employeeId} was not found!");
            }

            if (!HasOpenedShift(employeeId, out Shift openedShift))
            {
                return BadRequest($"Can't start shift for employee with id:{employeeId}, there's an open shift from {openedShift.Start}!");
            }

            Shift newShift = new Shift()
            {
                EmployeeId = employeeId,
                Start = startTime
            };

            if (await IsEmployeeCameLate(employee, startTime))
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
            if (employee is null || employee.IsDeleted)
            {
                return BadRequest($"Employee with id:{employeeId} was not found!");
            }
            
            if (HasOpenedShift(employeeId, out Shift openedShift))
            {
                return BadRequest($"Employee with id:{employeeId} has no shifts to end!");
            }

            openedShift.End = endTime;
            openedShift.HoursWorked = (int)endTime.Subtract(openedShift.Start).TotalHours;
            
            if (await IsEmployeeLeftEarly(employee, endTime, openedShift))
            {
                openedShift.TimesViolated++;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        #region Private methods
        private bool HasOpenedShift(int employeeId, out Shift openedShift)
        {
            openedShift = _context.Shifts
                .Where(s => s.EmployeeId == employeeId)
                .Where(s => s.End == null)
                .Include(s => s.Employee)
                .FirstOrDefault();
            return openedShift is null ? false : true;
        }

        private async Task<bool> IsEmployeeCameLate(Employee employee, DateTime startTime)
        {
            Position position = await _context.Positions.FindAsync(employee.PositionId);
            DateTime requiredTime = startTime.Date.Add(position.StartingHour);
            
            return startTime.CompareTo(requiredTime) > 0 ? true : false;
        }

        private async Task<bool> IsEmployeeLeftEarly(Employee employee, DateTime endTime, Shift openedShift)
        {
            Position position = await _context.Positions.FindAsync(employee.PositionId);
            DateTime requiredEndTime = openedShift.Start.Date
                .Add(position.StartingHour)
                .Add(position.RequiredWorkHours);

            return endTime.CompareTo(requiredEndTime) < 0 ? true : false;
        }
        #endregion
    }
}