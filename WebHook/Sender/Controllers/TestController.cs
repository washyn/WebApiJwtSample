using Microsoft.AspNetCore.Mvc;

namespace Sender.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Test GET endpoint called");
        return Ok(new { message = "Sender API is up and running!" });
    }

    [HttpPost("send-test")]
    public IActionResult SendTest([FromBody] string content)
    {
        _logger.LogInformation("Test POST endpoint called with content: {Content}", content);
        return Ok(new { status = "Message processed", content });
    }
}
