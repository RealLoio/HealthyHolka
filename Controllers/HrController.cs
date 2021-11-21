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
        public ActionResult<IEnumerable<Employee>> GetEmployees()
        {
            // TODO add optional field to filter employees if enabled/disabled (alternative to deleted)
            return Ok(_context.Employees
                .Include(e => e.Position)
                .Include(e => e.Shifts));
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

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Employee employee)
        {
            // TODO
            // Check if there's employee with this id (throw error if false)
            // Update employee
            // Return updated employee
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            // TODO
            // Check if there's employee with this id (throw error if false)
            // Disable this employee instead of deleting him
        }
    }
}
