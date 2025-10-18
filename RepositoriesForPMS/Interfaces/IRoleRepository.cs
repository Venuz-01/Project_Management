using ModelForPMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();

        // New method to get Role by Name
        Task<IEnumerable<Employee>> GetEmployeesByRoleNameAsync(string roleName);
    }
}
