using ModelForPMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Interfaces
{
    public interface IProjectAssignmentRepository
    {
        Task<ProjectAssignment> AddAsync(ProjectAssignment assignment);
        Task<IEnumerable<ProjectAssignment>> GetByProjectIdAsync(int projectId);
        Task<decimal> GetTotalAllocationForEmployeeAsync(int employeeId);

        Task DeleteTotalAllocationForEmployeeAsync(int employeeId);
    }
}
