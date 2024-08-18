using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace WeatherFunctionApp
{
    public static class WeatherDataFunction
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        // Function fetches weather data every minute
        [FunctionName("FetchWeatherData")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo timer, // TimerTrigger executes the function every minute
            ILogger log,
            ExecutionContext context // Use ExecutionContext to access the Function Invocation ID
        )
        {
            // Defining the city and API key for fetching weather data
            string city = "London";
            string apiKey = Environment.GetEnvironmentVariable("OpenWeatherApiKey");
            string requestUri = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}";

            // Generate a unique ID for the data
            string dataId = Guid.NewGuid().ToString();

            // Log the Function Invocation ID for clarity
            log.LogInformation($"Azure Function Invocation ID: {context.InvocationId}");

            try
            {
                // Makes an HTTP GET request to the OpenWeatherMap API
                HttpResponseMessage response = await HttpClient.GetAsync(requestUri);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body
                    var weatherData = JObject.Parse(responseBody);

                    // Log the success with the generated dataId
                    await LogSuccessAsync(dataId, responseBody);
                    log.LogInformation($"Data fetched successfully for {city} at {DateTime.UtcNow}");
                    log.LogInformation($"Data ID (used for storage): {dataId}");
                }
                else
                {
                    // If the API call fails, log the failure with a new data ID
                    await LogFailureAsync();
                    log.LogError($"Failed to fetch data for {city}: {responseBody}");
                }
            }
            catch (Exception ex)
            {
                // Logs any exceptions that occur during the API call
                await LogFailureAsync();
                log.LogError($"Exception occurred: {ex.Message}");
            }
        }

        /*
         Both methods are kept separate to maintain clarity and single responsibility.
         Separating LogSuccessAsync and LogFailureAsync ensures that each handles its
         specific case without adding unnecessary complexity, making future modifications easier
         in case it is necessary to add functionality to the code.
        */
        private static async Task LogSuccessAsync(string dataId, string payload)
        {
            // Initialize Azure Storage account and clients.
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WeatherLog");
            await table.CreateIfNotExistsAsync(); // Creates the table if it does not exist

            // Creates a new log entry for the successful API call
            var logEntry = new WeatherLogEntry
            {
                PartitionKey = "WeatherData",
                RowKey = dataId, // Use the provided dataId
                Timestamp = DateTime.UtcNow,
                Status = "Success"
            };

            // Inserts the log entry into the Azure Table Storage
            var insertOperation = TableOperation.Insert(logEntry);
            await table.ExecuteAsync(insertOperation);

            // Stores the API payload in Azure Blob Storage
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("weatherpayloads");
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlockBlobReference(dataId); // Use the same dataId for the blob name
            await blob.UploadTextAsync(payload);

           
        }

        // Log failure in case of an unsuccessful API call
        private static async Task LogFailureAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WeatherLog");
            await table.CreateIfNotExistsAsync();

            var logEntry = new WeatherLogEntry
            {
                PartitionKey = "WeatherData",
                RowKey = Guid.NewGuid().ToString(), // Generate a new unique RowKey
                Timestamp = DateTime.UtcNow,
                Status = "Failure"
            };

            // Inserts the log entry into the Azure Table Storage
            var insertOperation = TableOperation.Insert(logEntry);
            await table.ExecuteAsync(insertOperation);

            
        }

        // HTTP-triggered function to get all logs within a specified time period
        [FunctionName("GetWeatherLogs")]
        public static async Task<IActionResult> GetWeatherLogs(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "logs")] HttpRequest req,
            ILogger log)
        {
            // Parses the 'from' and 'to' query parameters
            string from = req.Query["from"];
            string to = req.Query["to"];

            if (!DateTime.TryParse(from, out DateTime fromDate) || !DateTime.TryParse(to, out DateTime toDate))
            {
                return new BadRequestObjectResult("Please provide valid 'from' and 'to' date query parameters.");
            }

            // Initializes Azure Storage account and clients
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("WeatherLog");

            // Filters logs based on the date range
            var query = new TableQuery<WeatherLogEntry>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "WeatherData"),
                    TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.GreaterThanOrEqual, fromDate),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForDate("Timestamp", QueryComparisons.LessThanOrEqual, toDate)
                    )
                )
            );

            // Executes the query and returns the log entries
            var logEntries = await table.ExecuteQuerySegmentedAsync(query, null);
            return new OkObjectResult(logEntries);
        }

        // HTTP-triggered function to retrieve a specific payload from Blob Storage
        [FunctionName("GetWeatherPayload")]
        public static async Task<IActionResult> GetWeatherPayload(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "payload/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("weatherpayloads");

            // Retrieves the blob using the provided log entry ID
            var blob = container.GetBlockBlobReference(id);

            if (!await blob.ExistsAsync())
            {
                log.LogWarning($"Blob with ID: {id} not found.");
                return new NotFoundObjectResult("Blob not found.");
            }

            // Downloads the blob content and returns it as the response
            string payload = await blob.DownloadTextAsync();
            log.LogInformation($"Blob {id} retrieved successfully.");
            return new OkObjectResult(payload);
        }
    }
}
