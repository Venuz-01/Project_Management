using DataContextForPMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelForPMS;

namespace Project_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {

        private readonly PMSAppDBContext _context;

        public ClientController(PMSAppDBContext context)
        {
               _context = context;
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients
                .Include(c => c.Projects)
                .ToListAsync();

        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Projects)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            return client == null ? NotFound() : Ok(client);
        }

        [HttpPost]

        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, client);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }


    }
}
