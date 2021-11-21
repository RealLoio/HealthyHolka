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
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees([FromQuery] bool showDisabled = false, int? positionId = null)
        {
            Position position = await _context.Positions.FindAsync(positionId);

            if (position is null) return BadRequest();

            IQueryable<Employee> employees = _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Shifts)
                .Where(e => e.IsDeleted == false || e.IsDeleted == showDisabled);

            if (positionId is not null) employees = employees.Where(e => e.PositionId == positionId);

            return Ok(employees);
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
            // TODO
            // Add includes to show Position 
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            
            return Ok(employee);
        }

        [HttpPost("employees/update/{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id) return BadRequest();

            Employee employeeToUpdate = await _context.Employees.FindAsync(id);

            if (employeeToUpdate is null)
            {
                return BadRequest();
            }

            _context.Entry(employeeToUpdate).CurrentValues.SetValues(employee);

            await _context.SaveChangesAsync();
            
            return Ok(employeeToUpdate);
        }

        [HttpPost("employees/disable/{id}")]
        public async Task<ActionResult> Disable(int id)
        {
            Employee employeeToDisable = await _context.Employees.FindAsync(id);

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
