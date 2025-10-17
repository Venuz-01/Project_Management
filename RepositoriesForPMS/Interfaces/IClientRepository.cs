using ModelForPMS;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Management_System.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<Client?> GetClientByIdAsync(int id);
        Task<Client> AddClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);
    }
}
