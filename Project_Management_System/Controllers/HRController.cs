
using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using Project_Management_System.Filter;
using RepositoriesForPMS.Interfaces;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRole("HR Manager")]
    public class HRController : ControllerBase
    {
        private readonly IHRrepository _repository;

        public HRController(IHRrepository repository)
        {
            _repository = repository;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repository.GetAllEmployeesAsync());

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _repository.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        // POST: api/Employee
        [HttpPost("employee")]

        public async Task<IActionResult> Create(Employee employee)
        {
            await _repository.AddEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, employee);
        }
        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteEmployeeAsync(id);
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest();
            await _repository.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        [HttpPost("leave")]

        public async Task<IActionResult> CreateLeave(Leave leave)
        {
            await _repository.AddLeaveAsync(leave);
            return CreatedAtAction(nameof(GetById), new { id = leave.LeaveId }, leave);
        }
    }
}
