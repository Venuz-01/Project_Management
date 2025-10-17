using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(Project project)
        {
            var created = await _projectRepository.AddAsync(project);
            return CreatedAtAction(nameof(GetProject), new { id = created.ProjectId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            var updated = await _projectRepository.UpdateAsync(id, project);
            if (updated == null)
                return NotFound();

            return Ok("Project Updated Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return Ok("Deleted Successfully");
        }
    }
}
