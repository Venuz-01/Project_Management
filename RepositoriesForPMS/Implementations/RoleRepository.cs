using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoriesForPMS.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly PMSAppDBContext _context;
        public RoleRepository(PMSAppDBContext context) => _context = context;

        public async Task<IEnumerable<Role>> GetAllAsync() =>
            await _context.Roles.ToListAsync();

        // Fetch role by RoleName
        public async Task<IEnumerable<Employee>> GetEmployeesByRoleNameAsync(string roleName)
        {
            return await _context.Employees
                .Where(e => e.Roles!.Any(r => r.RoleName.ToLower() == roleName.ToLower()))
                .Include(e => e.Roles)
                .ToListAsync();
        }
    }
}
