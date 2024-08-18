using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherDashboard.Models;
using Newtonsoft.Json.Linq;

namespace WeatherDashboard.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient; // HttpClient for making API requests
        private readonly string _apiKey; // API key for the weather service

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApiSettings:ApiKey"];
        }

        // Fetch weather data from the API for a specific city and country
        public async Task<WeatherInformation> FetchWeatherDataAsync(string city, string country)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city},{country}&appid={_apiKey}&units=metric";

            var response = await _httpClient.GetStringAsync(url); // Make the API request
            var data = JObject.Parse(response); // Parse the JSON response

            return new WeatherInformation
            {
                Country = country,
                City = city,
                Temperature = data["main"]["temp"].Value<float>(), // Extract temperature
                LastUpdate = DateTime.UtcNow // Record the current time
            };
        }
    }
}
