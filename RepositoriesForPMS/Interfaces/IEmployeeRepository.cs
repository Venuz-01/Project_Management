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
    }
}
