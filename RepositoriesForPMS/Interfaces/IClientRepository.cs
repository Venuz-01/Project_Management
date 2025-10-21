using ModelForPMS;

namespace RepositoriesForPMS.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> AddClientAsync(Client client);
        Task<bool> DeleteClientAsync(int id);

        Task UpdateClientAsync(Client client);
    }
}
