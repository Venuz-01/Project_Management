using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Implementations
{
    public class EngineerProjectRepository : IEngineerProjectRepository
    {
        private readonly PMSAppDBContext _context;
        public EngineerProjectRepository(PMSAppDBContext context) => _context = context;

        public async Task<IEnumerable<Project>> GetAllWithAssignmentsAsync()
        {
            return await _context.Projects
                .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Employee)
                .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Role)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int projectId)
        {
            return await _context.Projects
                .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Employee)
                .Include(p => p.ProjectAssignments)
                    .ThenInclude(pa => pa.Role)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }
    }
}
