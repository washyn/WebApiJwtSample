using System.ComponentModel.DataAnnotations;
using System.IO.Compression;

using Microsoft.AspNetCore.Mvc;

namespace Deploy.Agent.Controllers;

[ApiController]
[Route("app")]
public class AppController : ControllerBase
{
    [HttpPost]
    [Route("deploy")]
    public async Task Deploy([FromForm] AppDeployRequest request)
    {
        var tempPath = Path.GetTempPath();
        var filePath = Path.Combine(tempPath, request.File.FileName);
        var storedFileCompresed = Path.Combine(filePath, request.File.FileName);
        var randomPath = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        var saveDataPath = Path.Combine(tempPath, randomPath);

        Directory.CreateDirectory(saveDataPath);
        Directory.CreateDirectory(filePath);

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

        Directory.Delete(storedFileCompresed, true);
        Directory.Delete(saveDataPath, true);
    }
}

// TODO: only accept zip files
public class AppDeployRequest
{
    [Required] public IFormFile File { get; set; }

    [Required] public string NameApp { get; set; }
    public string PoolName { get; set; }
}