# Weather Dashboard Application

This project is a Weather Dashboard application that fetches weather data for selected cities and displays it in both tabular and graphical formats. The project consists of a .NET 6 backend and a React frontend.

## Frontend

### Features

- **Responsive Table**: Displays the latest weather data for selected cities, including temperature and last update time in local time and 24-hour format.
- **Interactive Chart**: Graphically displays the minimum and maximum temperatures for the cities.
- **Auto-Refresh**: The data is automatically updated every minute without refreshing the entire page.
- **Clean UI**: A simple and clean design with a responsive layout.

### Technologies

- React
- Chart.js
- CSS

### Instructions

1. **Clone the repository**:

   
   git clone https://github.com/yourusername/weather-dashboard.git

2. **Navigate to the frontend directory:**

    cd weather-dashboard/clientapp

3. **Install dependencies:**

    npm install

4. **Build the project:**

    npm run build

The npm run build command will not only create the production build of your React app but will also automatically move the build files to the backend's wwwroot/react-app directory using the postbuild.js script.

## Backend

### Features

- **API Endpoints:**
/api/weather/recent-data: Retrieves the latest weather data entries for the cities.
/api/weather/temperature-data: Retrieves the minimum and maximum temperatures for the cities.
- **Periodic Data Fetching:** Automatically fetches and stores weather data every minute.
- **SQLite Database:** Stores weather information, including temperature, city, country, and last update time.
- **Dependency Injection:** Services are injected to ensure modular and maintainable code.

### Technologies
- .NET 6
- Entity Framework Core
- SQLite
- ASP.NET Core

### Instructions


1. **Navigate to the backend directory:**

    cd weather-dashboard

2. **Update the appsettings.json file:**

Update the connection string to ensure it points to your local SQLite database.

    {
      "ConnectionStrings": {
        "WeatherDatabase": "Data Source=weather.db"
      },
      "WeatherApiSettings": {
        "ApiKey": "YourOpenWeatherMapApiKeyHere"
      }
    }

3.**Run database migrations:**

    dotnet ef database update

4.**Start the backend:**

Open the solution in Visual Studio and press F5 to run.

## Running the Application

1. **Ensure both frontend and backend are running.**

2. **Access the application:**
Open your browser and navigate to https://localhost:7086/ (or the appropriate port your backend is running on).

## Notes

- Ensure your API key from OpenWeatherMap is correctly set in appsettings.json.
- The frontend design has been kept simple and clean for ease of use and better readability.
- The table and chart automatically update every 60 seconds to reflect the latest weather data.

## Potential Improvements
- Add more cities and countries to track.
- Implement user authentication for personalized dashboards.
- Enable users to choose their preferred cities for tracking weather data.

