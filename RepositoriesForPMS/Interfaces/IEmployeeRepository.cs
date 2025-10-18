using ModelForPMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<IEnumerable<Employee>> GetByRoleAsync(int roleId);

        // New method to fetch employees by RoleName
        Task<IEnumerable<Employee>> GetByRoleNameAsync(string roleName);
    }
}
