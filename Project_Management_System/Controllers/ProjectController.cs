using DataContextForPMS;
using ModelForPMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {

        private readonly PMSAppDBContext _context;

        public ProjectController(PMSAppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelForPMS.Project>>> GetProjects()
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Employee)
                .ToListAsync();
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ModelForPMS.Project>> GetProject(int id)
        {
            var project = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                     .ThenInclude(pa => pa.Employee)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
            return project == null ? NotFound() : Ok(project);

        }

        [HttpPost]

        public async Task<ActionResult<ModelForPMS.Project>> CreateProject(ModelForPMS.Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProject), new { id = project.ProjectId }, project);
        }

        [HttpPut("id")]

        public async Task<IActionResult> UpdateProject(int id, ModelForPMS.Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }
            _context.Entry(project).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Projects.Any(e => e.ProjectId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok("Project Updated Succesfully");
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok("Deleted Successfully");
        }



    }
}
