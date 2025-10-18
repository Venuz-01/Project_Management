using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly PMSAppDBContext _context;
        public EmployeeRepository(PMSAppDBContext context) => _context = context;

        public async Task<IEnumerable<Employee>> GetAllAsync() =>
            await _context.Employees
                .Include(e => e.Roles) // Include roles collection
                .ToListAsync();

        public async Task<Employee?> GetByIdAsync(int employeeId) =>
            await _context.Employees
                .Include(e => e.Roles)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        public async Task<IEnumerable<Employee>> GetByRoleAsync(int roleId) =>
            await _context.Employees
                .Where(e => e.Roles.Any(er => er.RoleId == roleId))
                .Include(e => e.Roles)
                .ToListAsync();

        // New method: get all employees by RoleName
        public async Task<IEnumerable<Employee>> GetByRoleNameAsync(string roleName) =>
            await _context.Employees
                .Include(e => e.Roles)
                .Where(e => e.Roles.Any(r => r.RoleName.ToLower() == roleName.ToLower()))
                .ToListAsync();
    }
}
