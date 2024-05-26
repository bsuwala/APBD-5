using Microsoft.AspNetCore.Mvc;
using Zadanie5.DataTransferObjects;
using Zadanie5.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Zadanie5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public TripsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetTrips()
        {
            var trips = _dbContext.Trips.OrderByDescending(t => t.StartDate).ToList();
            return Ok(trips);
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] AddTripClientDto addTripClientDto)
        {
            var trip = _dbContext.Trips.FirstOrDefault(t => t.IdTrip == idTrip);
            if (trip == null)
            {
                return NotFound("Trip not found");
            }

            var client = _dbContext.Clients.FirstOrDefault(c => c.IdClient == addTripClientDto.ClientId);
            if (client == null)
            {
                return NotFound("Client not found");
            }

            var clientTrip = new ClientTrip
            {
                IdTrip = idTrip,
                IdClient = addTripClientDto.ClientId,
                RegisteredAt = addTripClientDto.RegisteredAt,
                PaymentDate = addTripClientDto.PaymentDate
            };
            _dbContext.ClientTrips.Add(clientTrip);

            await _dbContext.SaveChangesAsync();

            return Ok(clientTrip);
        }
    }
}