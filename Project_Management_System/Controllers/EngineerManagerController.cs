using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using Project_Management_System.Filter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AuthorizeRole("Engineering Manager")]
    public class EngineeringManagerController : ControllerBase
    {
        private readonly IEngineerProjectRepository _projectRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IProjectAssignmentRepository _assignmentRepo;

        public EngineeringManagerController(
            IEngineerProjectRepository projectRepo,
            IEmployeeRepository employeeRepo,
            IRoleRepository roleRepo,
            IProjectAssignmentRepository assignmentRepo)
        {
            _projectRepo = projectRepo;
            _employeeRepo = employeeRepo;
            _roleRepo = roleRepo;
            _assignmentRepo = assignmentRepo;
        }

        // Get all projects with assignments
        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _projectRepo.GetAllWithAssignmentsAsync();
            return Ok(projects);
        }

        // Get all roles
        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _roleRepo.GetAllAsync();
            return Ok(roles);
        }

        // Get employees by RoleId
        //[HttpGet("employees/{roleId}")]
        //public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByRole(int roleId)
        //{
        //    var employees = await _employeeRepo.GetByRoleAsync(roleId);
        //    return Ok(employees);
        //}

        // New: Get employees by RoleName
        [HttpGet("employees/byRoleName/{roleName}")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployeesByRoleName(string roleName)
        {
            var role = await _roleRepo.GetByNameAsync(roleName);
            if (role == null)
                return NotFound($"Role '{roleName}' not found.");

            var employees = role.Employee ?? new List<Employee>();
            return Ok(employees);
        }

        // Assign employee to project
        [HttpPost("assign")]
        public async Task<ActionResult<ProjectAssignment>> AssignEmployee(ProjectAssignment assignment)
        {
            var totalAllocation = await _assignmentRepo.GetTotalAllocationForEmployeeAsync(assignment.EmployeeId);
            if (totalAllocation + assignment.AllocationPercent > 100)
                return BadRequest("Total allocation exceeds 100% for this employee.");

            var result = await _assignmentRepo.AddAsync(assignment);
            return Ok(result);
        }

        // Get all assignments for a project
        [HttpGet("projects/{projectId}/assignments")]
        public async Task<ActionResult<IEnumerable<ProjectAssignment>>> GetProjectAssignments(int projectId)
        {
            var assignments = await _assignmentRepo.GetByProjectIdAsync(projectId);
            return Ok(assignments);
        }
    }
}
