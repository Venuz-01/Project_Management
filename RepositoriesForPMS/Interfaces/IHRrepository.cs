using ModelForPMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Interfaces
{
    public interface IHRrepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        // GET by ID: Retrieve a single employee by ID
        Task<Employee> GetEmployeeByIdAsync(int employeeId);

        // POST: Add a new employee
        Task AddEmployeeAsync(Employee employee);

        // DELETE: Delete an employee by ID
        Task DeleteEmployeeAsync(int employeeId);

        Task UpdateEmployeeAsync(Employee employee);

        // Utility: Check if an employee exists
        Task<bool> EmployeeExistsAsync(int employeeId);

        Task AddLeaveAsync(Leave leave);

    }
}
