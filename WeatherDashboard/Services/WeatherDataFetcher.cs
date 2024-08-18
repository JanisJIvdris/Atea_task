using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeatherDashboard.Data;

namespace WeatherDashboard.Services
{
    public class WeatherDataFetcher : IHostedService, IDisposable
    {
        private Timer _timer; // Timer to periodically fetch data
        private readonly IServiceProvider _services;

        public WeatherDataFetcher(IServiceProvider services)
        {
            _services = services;
        }

        // Start the service and set up the timer to fetch weather data every minute
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        // The work to be done periodically: fetch weather data and save it to the database
        private async void DoWork(object state)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WeatherContext>();
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();

                // Fetch weather data for three cities
                var londonWeather = await weatherService.FetchWeatherDataAsync("London", "UK");
                var parisWeather = await weatherService.FetchWeatherDataAsync("Paris", "France");
                var berlinWeather = await weatherService.FetchWeatherDataAsync("Berlin", "Germany");

                // Add the fetched data to the database
                context.WeatherInformation.AddRange(londonWeather, parisWeather, berlinWeather);
                await context.SaveChangesAsync();
            }
        }

        // Stop the service
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        // Dispose of the timer when the service is no longer needed
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
