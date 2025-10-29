
using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using Project_Management_System.Filter;
using RepositoriesForPMS.Interfaces;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpDelete("Employee/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteEmployeeAsync(id);
            return NoContent();
        }
        [HttpPut("Employee/{id}")]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return BadRequest();
            await _repository.UpdateEmployeeAsync(employee);
            return NoContent();
        }

        [HttpGet("leave")]
        public async Task<IActionResult> GetAllLeaves() => Ok(await _repository.GetAllLeavesAsync());

        [HttpGet("leave/{EmployeeId}")]
        public async Task<IActionResult> GetLeavesByEmployeeId(int EmployeeId)
        {
            var leave = await _repository.GetAllLeavesWithEmployeeIDAsync(EmployeeId);
            if (leave == null) return NotFound();
            return Ok(leave);
        }


        [HttpGet("leave/{EmployeeId}/count")]
        public async Task<IActionResult> GetLeavesCountByEmployeeId(int EmployeeId)
        {
            var leave = await _repository.GetAllLeavesCountWithEmployeeIDAsync(EmployeeId);
            if (leave == null) return NotFound();
            return Ok(leave);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> CreateLeave(Leave leave)
        {
            await _repository.AddLeaveAsync(leave);
            return CreatedAtAction(nameof(GetById), new { id = leave.LeaveId }, leave);
        }
        [HttpDelete("leave/{EmployeeId}")]
        public async Task<IActionResult> DeleteLeave(int EmployeeId)
        {
            await _repository.DeleteLeaveAsync(EmployeeId);
            return NoContent();
        }
        [HttpPut("leave/{EmployeeId}")]
        public async Task<IActionResult> UpdateLeave(int EmployeeId, Leave leave)
        {
            if (EmployeeId != leave.EmployeeId) return BadRequest();
            await _repository.UpdateLeaveAsync(leave);
            return NoContent();
        }
    }
}

