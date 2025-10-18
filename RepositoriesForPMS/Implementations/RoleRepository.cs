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
        public async Task<Role> GetByNameAsync(string roleName) =>
            await _context.Roles
                          .Include(r => r.Employee) // include employees related to role
                          .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleName.ToLower());
    }
}
