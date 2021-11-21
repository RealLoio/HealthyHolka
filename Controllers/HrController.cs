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
                .Where(e => e.IsDeleted == false || e.IsDeleted == showDisabled));
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
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            var employeeToUpdate = await _context.Employees.FindAsync(id);

            if (employeeToUpdate is null)
            {
                return BadRequest();
            }

            _context.Entry(employeeToUpdate).CurrentValues.SetValues(employee);

            //_context.Employees.Update(employeeToUpdate);
            await _context.SaveChangesAsync();
            
            return Ok(employeeToUpdate);
        }

        [HttpPost("employees/disable/{id}")]
        public async Task<ActionResult> Disable(int id)
        {
            var employeeToDisable = await _context.Employees.FindAsync(id);

            if (employeeToDisable is null)
            {
                return BadRequest();
            }
            if (employeeToDisable.IsDeleted)
            {
                return BadRequest();
            }

            employeeToDisable.IsDeleted = true;

            _context.Employees.Update(employeeToDisable);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}
