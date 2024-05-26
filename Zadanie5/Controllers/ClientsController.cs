using Microsoft.AspNetCore.Mvc;
using Zadanie5.DataTransferObjects;
using Zadanie5.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public ClientsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var client = _dbContext.Clients.FirstOrDefault(c => c.IdClient == idClient);
            if (client == null)
            {
                return NotFound("Client not found");
            }

            var clientTrips = _dbContext.ClientTrips.Where(ct => ct.IdClient == idClient).ToList();
            if (clientTrips.Count > 0)
            {
                return BadRequest("Client has assigned trips");
            }

            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

            return Ok(client);
        }
    }
}