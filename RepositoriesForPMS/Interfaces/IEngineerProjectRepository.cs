using ModelForPMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Interfaces
{
    public interface IEngineerProjectRepository
    {
        Task<IEnumerable<Project>> GetAllWithAssignmentsAsync();
        Task<Project?> GetByIdAsync(int projectId);
    }
}
