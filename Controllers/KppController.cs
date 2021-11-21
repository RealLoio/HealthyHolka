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
                return BadRequest($"You can't start a new shift, close the last one that started on {openedShift.Start}!");
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
            // TODO
            // Check if there's employee with this id (throw error if false)
            // Check if employees shift exists (throw error if false)
            // Find shift and update with end time
            // Update employees hours worked
            // Check if employee left earlier than allowed (add +1 to fuck ups)
            return Ok();
        }
    }
}