using HealthyHolka.DataContext;
using HealthyHolka.Models;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult> StartShift(int employeeId, [FromQuery] DateTimeOffset startTime)
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

            // TODO Check if employee came later than allowed (mark this shift as time violated)

            Shift newShift = new Shift()
            {
                EmployeeId = employeeId,
                Start = startTime,
                End = null,
                HoursWorked = 0
            };
            _context.Shifts.Add(newShift);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("end/{employeeId}")]
        public async Task<ActionResult> EndShift(int employeeId, [FromQuery] DateTimeOffset endTime)
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
            await _context.SaveChangesAsync();

            // TODO Check if employee left earlier than allowed (add +1 to fuck ups)

            return Ok();
        }
    }
}