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
            await _context.Employees.Include(e => e.Roles).ThenInclude(er => er.RoleName).ToListAsync();

        public async Task<Employee?> GetByIdAsync(int employeeId) =>
            await _context.Employees.Include(e => e.Roles).ThenInclude(er => er.RoleName)
                                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

        public async Task<IEnumerable<Employee>> GetByRoleAsync(int roleId) =>
            await _context.Employees
                .Where(e => e.Roles.Any(er => er.RoleId == roleId))
                .Include(e => e.Roles).ThenInclude(er => er.RoleName)
                .ToListAsync();
    }
}
