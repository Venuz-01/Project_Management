using DataContextForPMS;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;

namespace RepositoriesForPMS.Implementations
{
    public class ClientRepository : IClientRepository
    {
        private readonly PMSAppDBContext _context;

        public ClientRepository(PMSAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _context.Clients
                .Include(c => c.Projects)
                .ToListAsync();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _context.Clients
                .Include(c => c.Projects)
                .FirstOrDefaultAsync(c => c.ClientId == id);
        }

        public async Task<Client> AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateClientAsync(Client client)
        {
            _context.Entry(client).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}

