namespace WeatherDashboard.Models
{
    public class WeatherInformation
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public float Temperature { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
