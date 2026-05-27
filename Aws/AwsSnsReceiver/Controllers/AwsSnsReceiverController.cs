using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.Mvc;

namespace AwsSnsReceiver;

[Route("api/aws-sns-receiver")]
public class AwsSnsReceiverController : AbpController
{
    private readonly ILogger<AwsSnsReceiverController> _logger;


    public AwsSnsReceiverController(ILogger<AwsSnsReceiverController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task Post()
    {
        _logger.LogInformation("Post");
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        _logger.LogInformation("Mensaje recibido:");
        _logger.LogInformation(body);
    }

    [HttpGet]
    public async Task Get()
    {
        _logger.LogInformation("Get");
        using var reader = new StreamReader(this.Request.Body);
        var body = await reader.ReadToEndAsync();
        _logger.LogInformation(body);
    }
}