using Microsoft.WindowsAzure.Storage.Table;

namespace WeatherFunctionApp
{
    public class WeatherLogEntry : TableEntity
    {
        public string Status { get; set; }
    }
}
