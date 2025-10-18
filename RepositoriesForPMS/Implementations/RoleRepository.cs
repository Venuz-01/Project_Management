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

        public async Task<IEnumerable<Role>> GetAllAsync() => await _context.Roles.ToListAsync();
    }
}
