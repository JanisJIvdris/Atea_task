# Weather Data Azure Function

This project is an Azure Function application that fetches weather data for London and logs the results in Azure Storage. The project consists of a timer-triggered function that runs every minute, as well as HTTP-triggered functions for retrieving logs and payloads.

## Azure Function

### Features

- **Periodic Data Fetching:** Automatically fetches weather data for London every minute using a timer-triggered function.
- **Logging:** Logs each successful or failed attempt in Azure Table Storage.
- **Blob Storage:** Stores the full API response payload in Azure Blob Storage.
- **API Endpoints:**
  - **/api/logs:** Retrieves log entries within a specified time period.
  - **/api/payload/{id}:** Retrieves the specific payload from Blob Storage based on the log entry ID.

### Technologies

- .NET 6
- Azure Functions
- Azure Storage (Table and Blob)
- HTTP Client

### Instructions

1. **Clone the repository:**
   

   git clone https://github.com/yourusername/WeatherDataFunction.git

2. **Navigate to the project directory:**

    cd WeatherDataFunction

3. **Set up local settings:**

Create a local.settings.json file in the project directory with the following content:


    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "OpenWeatherApiKey": "YOUR_API_KEY"
      }
    }
Replace "YOUR_API_KEY" with your actual OpenWeatherMap API key.

4. **Install dependencies:**

    dotnet restore

5. **Build the project:**

    dotnet build

6. **Run the project locally:**

    func start

### API Endpoints

## Retrieve logs:

- Endpoint: GET /api/logs
- Description: Retrieves log entries within a specified time period.
- Example: http://localhost:7071/api/logs?from=2024-08-01&to=2024-08-31

## Retrieve specific payload:
- Endpoint: GET /api/payload/{id}
- Description: Retrieves the specific payload from Blob Storage based on the log entry ID.
- Example: http://localhost:7071/api/payload/your-log-id

### Deployment

## Deploy to Azure:

1. **Publish the function app:**

You can deploy this function app to Azure using Visual Studio or Azure CLI.

2. **Configure Azure settings:**

Ensure that your Azure Storage connection string and API key are correctly set in the application settings on the Azure portal.

### Running the Application

1. **Ensure the Azure Function is running.**

The function app should be running either locally via Azure Functions Core Tools or deployed to an Azure environment.

2. **Access the API endpoints:**

Use the provided HTTP endpoints to interact with the stored weather data logs and payloads.


### Notes
- Ensure your OpenWeatherMap API key is correctly set in local.settings.json.
- The function app is designed to log both successful and failed attempts to fetch weather data, providing robust tracking of operations.

### Potential Improvements
- Expand the functionality to support fetching weather data for multiple cities.
- Add more detailed error handling and logging.
- Implement additional HTTP endpoints for more advanced querying of logs and data.
