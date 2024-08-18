using Microsoft.EntityFrameworkCore;
using WeatherDashboard.Data;
using WeatherDashboard.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient(); // Register HttpClient for making HTTP requests

// Register the application's database context, specifying the use of SQLite
builder.Services.AddDbContext<WeatherContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WeatherDatabase")));

// Register services to be used within the app's lifetime
builder.Services.AddScoped<WeatherService>();
builder.Services.AddHostedService<WeatherDataFetcher>();

builder.Services.AddControllers();  // Registering controllers as services

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();  // Serve static files, including React app files

app.UseRouting();

app.UseAuthorization();

app.MapControllers();  // Map API controllers

app.MapFallbackToFile("/react-app/index.html");  // Serve React frontend for all non-API routes 

app.Run();
