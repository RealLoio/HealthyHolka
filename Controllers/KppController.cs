using HealthyHolka.DataContext;
using HealthyHolka.Models;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyHolka.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class KppController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public KppController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("shiftStart/{employeeId}")]
        public async Task<ActionResult> StartShift(int employeeId, [FromQuery] DateTimeOffset startTime)
        {
            // TODO
            // Check if there's employee with this id (throw error if false)
            // Check if this employee null value in end time field (throw error if false)
            // Check if employee came later than allowed (add +1 to fuck ups)
            var newShift = new Shift()
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
        [Route("shiftEnd/{employeeId}")]
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