using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeatherDashboard.Data;

namespace WeatherDashboard.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherApiController : ControllerBase
    {
        private readonly WeatherContext _context;

        public WeatherApiController(WeatherContext context)
        {
            _context = context;
        }

        // Endpoint to get the most recent weather data (for the last three entries)
        [HttpGet("recent-data")]
        public async Task<IActionResult> GetRecentWeatherData()
        {
            var recentWeatherData = await _context.WeatherInformation
                .OrderByDescending(w => w.LastUpdate)
                .Take(3)
                .ToListAsync();

            return Ok(recentWeatherData); // Returns the data in JSON format
        }

        // Endpoint to get temperature data grouped by city for charts
        [HttpGet("temperature-data")]
        public async Task<IActionResult> GetTemperatureData()
        {
            var temperatureData = await _context.WeatherInformation
                .GroupBy(w => w.City)
                .Select(g => new {
                    City = g.Key,
                    MinTemperature = g.Min(w => w.Temperature),
                    MaxTemperature = g.Max(w => w.Temperature),
                    LastUpdate = g.Max(w => w.LastUpdate).ToString("yyyy-MM-ddTHH:mm:ss")
                })
                .ToListAsync();

            return Ok(temperatureData); // Returns the data in JSON format
        }
    }
}
