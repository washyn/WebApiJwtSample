using System.ComponentModel.DataAnnotations;
using System.IO.Compression;

using Microsoft.AspNetCore.Mvc;

namespace Deploy.Agent.Controllers;

[ApiController]
[Route("app")]
public class AppController : ControllerBase
{
    private readonly ILogger<AppController> _logger;

    public AppController(ILogger<AppController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("deploy")]
    public async Task Deploy([FromForm] AppDeployRequest request)
    {
        var tempPath = Path.GetTempPath();

        var randomPathSource = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        var sourcePath = Path.Combine(tempPath, randomPathSource);
        var storedFileCompresed = Path.Combine(sourcePath, request.File.FileName);
        var randomPathDest = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        var saveDataPath = Path.Combine(tempPath, randomPathDest);

        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(saveDataPath);

        // save
        using (var stream = new FileStream(storedFileCompresed, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        using (var fs = System.IO.File.OpenRead(storedFileCompresed))
        {
            using (var zip = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                zip.ExtractToDirectory(saveDataPath);
            }
        }

        _logger.LogInformation($"Deploying {request.NameApp} to {request.PoolName}");
        _logger.LogInformation($"storedFileCompresed: {sourcePath}");
        _logger.LogInformation($"saveDataPath: {saveDataPath}");
        // Directory.Delete(randomPathSource, true);
        // Directory.Delete(saveDataPath, true);
    }
}

// TODO: only accept zip files
public class AppDeployRequest
{
    [Required] public IFormFile File { get; set; }

    [Required] public string NameApp { get; set; }
    public string PoolName { get; set; }
}