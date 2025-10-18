using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using Project_Management_System.Filter;
using RepositoriesForPMS.Interfaces;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[AuthorizeRole("Sales Manager")]
    public class SalesManagerController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IProjectRepository _projectRepository;

        public SalesManagerController(IClientRepository clientRepository, IProjectRepository projectRepository)
        {
            _clientRepository = clientRepository;
            _projectRepository = projectRepository;
        }

        [HttpPost("add-client")]
        public async Task<ActionResult<Client>> AddClient(Client client)
        {
            var newClient = await _clientRepository.AddClientAsync(client);
            return CreatedAtAction(nameof(GetClientById), new { id = newClient.ClientId }, newClient);
        }

        [HttpGet("clients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("clients/{id}")]
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost("add-project/{clientId}")]
        public async Task<ActionResult<Project>> AddProjectToClient(int clientId, Project project)
        {
            project.ClientId = clientId;
            var createdProject = await _projectRepository.AddAsync(project);
            return CreatedAtAction(nameof(GetProjectsByClientId), new { clientId = createdProject.ClientId }, createdProject);
        }

        [HttpGet("clients/{clientId}/projects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByClientId(int clientId)
        {
            var projects = await _projectRepository.GetByClientIdAsync(clientId);
            return Ok(projects);
        }
    }
}
