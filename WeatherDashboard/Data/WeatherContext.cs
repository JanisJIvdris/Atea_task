using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Models;

namespace WeatherDashboard.Data
{
    // Database context class for the WeatherInformation entity
    public class WeatherContext : DbContext
    {
        public WeatherContext(DbContextOptions<WeatherContext> options)
            : base(options)
        {
        }

        public DbSet<WeatherInformation> WeatherInformation { get; set; }
    }
}
