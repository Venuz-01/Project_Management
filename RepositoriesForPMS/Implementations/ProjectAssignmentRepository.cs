using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Implementations
{
    public class ProjectAssignmentRepository : IProjectAssignmentRepository
    {
        private readonly PMSAppDBContext _context;
        public ProjectAssignmentRepository(PMSAppDBContext context) => _context = context;

        public async Task<ProjectAssignment> AddAsync(ProjectAssignment assignment)
        {
            _context.ProjectAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task DeleteTotalAllocationForEmployeeAsync(int employeeId)
        {
            var employee = await _context.ProjectAssignments.FindAsync(employeeId);
            if (employee != null)
            {
                _context.ProjectAssignments.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProjectAssignment>> GetByProjectIdAsync(int projectId) =>
            await _context.ProjectAssignments
                .Where(pa => pa.ProjectId == projectId)
                .Include(pa => pa.Employee)
                .Include(pa => pa.Role)
                .ToListAsync();

        public async Task<decimal> GetTotalAllocationForEmployeeAsync(int employeeId) =>
            await _context.ProjectAssignments
                .Where(pa => pa.EmployeeId == employeeId)
                .SumAsync(pa => pa.AllocationPercent);
    }
}
