using Microsoft.AspNetCore.Mvc;
using ModelForPMS;
using RepositoriesForPMS.Interfaces;

[Route("api/[controller]")]
[ApiController]
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
    public async Task<ActionResult<Client>> AddClient([FromBody] Client client)
    {
        if (client == null)
            return BadRequest("Client data is required.");

        var existingClients = await _clientRepository.GetAllAsync();
        bool exists = existingClients.Any(c =>
            c.ClientName.ToLower() == client.ClientName.ToLower() ||
            c.ClientEmail.ToLower() == client.ClientEmail.ToLower());

        if (exists)
            return Conflict(new { message = "Client with this name or email already exists." });

        if (client.ClientId == 0)
        {
            client.ClientId = existingClients.Any() ? existingClients.Max(c => c.ClientId) + 1 : 1;
        }

        var newClient = await _clientRepository.AddClientAsync(client);
        return Ok(newClient);
    }

    [HttpGet("clients")]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
    {
        var clients = await _clientRepository.GetAllAsync();
        return Ok(clients);
    }

    [HttpPost("add-project/{clientId}")]
    public async Task<ActionResult<Project>> AddProjectToClient(int clientId, [FromBody] Project project)
    {
        if (project == null)
            return BadRequest("Project data is required.");

        var allProjects = await _projectRepository.GetAllAsync();
        bool duplicateProject = allProjects.Any(p =>
            p.ClientId == clientId &&
            p.ProjectName.ToLower() == project.ProjectName.ToLower());

        if (duplicateProject)
            return Conflict(new { message = "Project with this name already exists for this client." });

        project.ClientId = clientId;
        var createdProject = await _projectRepository.AddAsync(project);

        return Ok(createdProject);
    }

    [HttpGet("clients/{clientId}/projects")]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjectsByClientId(int clientId)
    {
        var projects = await _projectRepository.GetByClientIdAsync(clientId);
        return Ok(projects);
    }
}
