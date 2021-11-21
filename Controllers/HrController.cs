using HealthyHolka.DataContext;
using HealthyHolka.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace HealthyHolka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HrController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public HrController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("employees")]
        public ActionResult<IEnumerable<Employee>> GetEmployees([FromQuery] bool showDisabled = false)
        {
            return Ok(_context.Employees
                .Include(e => e.Position)
                .Include(e => e.Shifts)
                .Where(e => e.IsEnabled == true || e.IsEnabled == !showDisabled));
        }

        [HttpGet]
        [Route("positions")]
        public ActionResult<IEnumerable<Employee>> GetPositions()
        {
            return Ok(_context.Positions);
        }

        [HttpPost]
        [Route("employees")]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            
            return Ok(employee);
        }

        [HttpPost("employees/update/{id}")]
        public void Put(int id, [FromBody] Employee employee)
        {
            // TODO
            // Check if there's employee with this id (throw error if false)
            // Update employee
            // Return updated employee
        }

        [HttpPost("employees/disable/{id}")]
        public async Task<ActionResult> Disable(int id)
        {
            var employeeToDisable = await _context.Employees.FindAsync(id);

            if (employeeToDisable is null)
            {
                return BadRequest();
            }
            if (!employeeToDisable.IsEnabled)
            {
                return BadRequest();
            }

            employeeToDisable.IsEnabled = false;

            _context.Employees.Update(employeeToDisable);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}
