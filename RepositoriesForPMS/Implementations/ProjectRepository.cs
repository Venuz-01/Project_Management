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
            return project;
        }

        public async Task<Project?> UpdateAsync(int id, Project project)
        {
            if (id != project.ProjectId)
                return null;

            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Project>> GetByClientIdAsync(int clientId)
        {
            return await _context.Projects
                .Where(p => p.ClientId == clientId)
                .Include(p => p.ProjectAssignments)
                .ThenInclude(pa => pa.Employee)
                .ToListAsync();
        }
    }
}
