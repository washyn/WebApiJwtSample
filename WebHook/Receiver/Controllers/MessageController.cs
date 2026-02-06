using Lib;

using Microsoft.AspNetCore.Mvc;

namespace Receiver.Controllers;

[ApiController]
[Route("api/message")]
public class MessageController : ControllerBase
{
    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult ReceiveMessage([FromBody] object message)
    {
        _logger.LogInformation("Received message: {Message}", message.ToString());
        return Ok(new { status = "success", receivedAt = DateTime.UtcNow });
    }

    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "Healthy" });
    }

    // TODO: validate headers and signature, use midlware for reference...
    [Route("receive")]
    [HttpPost]
    public async Task ReceiveMessageAsync([FromBody] WebhookPayload message)
    {
        _logger.LogInformation("Received message");
        _logger.LogInformation("Received message : {@Message}", this.Request.Body);
        _logger.LogInformation("Received message : {@message}", message);
    }
}
