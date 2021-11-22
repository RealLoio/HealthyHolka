using HealthyHolka.Models;
using HealthyHolka.DataContext;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees([FromQuery] bool showDeleted = false, [FromQuery] int? positionId = null)
        {
            IQueryable<Employee> employees = _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Shifts)
                .Where(e => e.IsDeleted == false || e.IsDeleted == showDeleted);

            if (positionId is not null)
            {
                Position position = await _context.Positions.FindAsync(positionId);
                if (position is null)
                {
                    return BadRequest($"Position with id:{positionId} was not found!");
                }

                employees = employees.Where(e => e.PositionId == positionId);
            }

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
            Position position = await _context.Positions.FindAsync(employee.PositionId);
            if (position is null)
            {
                return BadRequest($"Position with id:{employee.PositionId} was not found!");
            }

            await _context.Employees.Add(employee).Reference(e => e.Position).LoadAsync();
            await _context.SaveChangesAsync();
            
            return Ok(employee);
        }

        [HttpPost("employees/update/{id}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest($"Passed id:{id} does not match id from json:{employee.Id}!");
            }

            Employee employeeToUpdate = await _context.Employees.FindAsync(id);
            if (employeeToUpdate is null)
            {
                return BadRequest($"Employee with id:{id} was not found!");
            }

            Position position = await _context.Positions.FindAsync(employee.PositionId);
            if (position is null)
            {
                return BadRequest($"Position with id:{employee.PositionId} was not found!");
            }

            EntityEntry<Employee> entry = _context.Entry(employeeToUpdate);
            entry.CurrentValues.SetValues(employee);
            await entry.Reference(e => e.Position).LoadAsync();
            await _context.SaveChangesAsync();
            
            return Ok(employeeToUpdate);
        }

        [HttpPost("employees/disable/{id}")]
        public async Task<ActionResult> Disable(int id)
        {
            Employee employeeToDisable = await _context.Employees.FindAsync(id);
            if (employeeToDisable is null)
            {
                return BadRequest($"Employee with id:{id} was not found!");
            }
            if (employeeToDisable.IsDeleted)
            {
                return BadRequest($"Employee with id:{id} is already deleted!");
            }

            employeeToDisable.IsDeleted = true;
            _context.Employees.Update(employeeToDisable);
            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}