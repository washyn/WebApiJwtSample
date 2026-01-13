using Microsoft.AspNetCore.Mvc;

namespace WebAppMultitenancyInfraestructure.Controllers;

[Route("api/app")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly ILogger<AppController> _logger;
    private readonly ICurrentTenant _currentTenant;

    public AppController(ILogger<AppController> logger, ICurrentTenant currentTenant)
    {
        _logger = logger;
        _currentTenant = currentTenant;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogInformation("Getting weather forecasts");
        _logger.LogInformation($"Current Tenant Id: {_currentTenant.Id}");
        _logger.LogInformation($"Current Tenant : {_currentTenant.Name}");
        
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();
        return forecast;
    }
}