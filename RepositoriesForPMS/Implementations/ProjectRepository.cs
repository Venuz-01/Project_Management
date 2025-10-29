using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;

namespace RepositoriesForPMS.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly PMSAppDBContext _context;

        public ProjectRepository(PMSAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .FirstOrDefaultAsync(p => p.ProjectId == id);
        }

        public async Task<Project> AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Re-fetch the project with navigation properties
            var createdProject = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .FirstOrDefaultAsync(p => p.ProjectId == project.ProjectId);

            return createdProject ?? project;
        }

        public async Task<IEnumerable<Project>> GetByClientIdAsync(int clientId)
        {
            return await _context.Projects
                .Where(p => p.ClientId == clientId)
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .ToListAsync();
        }

        public async Task<Project?> UpdateAsync(int id, Project project)
        {
            var existing = await _context.Projects.FindAsync(id);
            if (existing == null)
                return null;

            existing.ProjectName = project.ProjectName;
            existing.Description = project.Description;
            existing.StartDate = project.StartDate;
            existing.EndDate = project.EndDate;
            existing.DailyRate = project.DailyRate;
            existing.ClientId = project.ClientId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Projects.FindAsync(id);
            if (existing == null)
                return false;

            _context.Projects.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Project>> GetAllProjectAsync()
        {
            return await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .ToListAsync();
        }
    }
}
