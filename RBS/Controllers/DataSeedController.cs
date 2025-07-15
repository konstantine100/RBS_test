using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RBS.Helpers;

namespace RBS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSeedController : ControllerBase
    {
        private readonly BookingDataSeeder _seeder;
        private readonly ILogger<DataSeedController> _logger;

        public DataSeedController(BookingDataSeeder seeder, ILogger<DataSeedController> logger)
        {
            _seeder = seeder;
            _logger = logger;
        }

        [HttpPost("seed-bookings")]
        public async Task<IActionResult> SeedBookings([FromQuery] int count = 10000)
        {
            if (count <= 0 || count > 1000000)
            {
                return BadRequest("Count must be between 1 and 1,000,000");
            }

            _logger.LogInformation("Received request to seed {Count} bookings", count);
            
            var result = await _seeder.SeedBookingsAsync(count); // FIXED: Added "Async"
            
            if (result.Success)
            {
                return Ok(new
                {
                    result.Success,
                    result.Message,
                    result.TotalRecords,
                    ElapsedSeconds = result.ElapsedTime.TotalSeconds,
                    RecordsPerSecond = result.TotalRecords / result.ElapsedTime.TotalSeconds
                });
            }

            return StatusCode(500, new { result.Success, result.Message });
        }
    }
}