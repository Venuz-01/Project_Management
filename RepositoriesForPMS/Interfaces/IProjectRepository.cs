using ModelForPMS;

namespace RepositoriesForPMS.Interfaces
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<Project> AddAsync(Project project);
        Task<Project?> UpdateAsync(int id, Project project);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Project>> GetByClientIdAsync(int clientId);

        Task<IEnumerable<Project>> GetAllProjectAsync();

    }
}
