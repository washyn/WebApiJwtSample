using System.ComponentModel.DataAnnotations;
using System.IO.Compression;

using Microsoft.AspNetCore.Mvc;

namespace Agent.Controllers;

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
        var randomPathSource = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
        var randomPathDest = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

        Directory.CreateDirectory(randomPathSource);
        Directory.CreateDirectory(randomPathDest);

        // Usar un nombre seguro para el archivo temporal
        var safeFileName = "deploy_package.zip";
        var storedFileCompressed = Path.Combine(randomPathSource, safeFileName);

        // Guardar el archivo
        using (var stream = new FileStream(storedFileCompressed, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        // Extraer el archivo
        ZipFile.ExtractToDirectory(storedFileCompressed, randomPathDest);

        _logger.LogInformation($"Desplegando {request.NameApp} en el pool {request.PoolName}");
        _logger.LogInformation($"Archivo guardado en: {randomPathSource}");
        _logger.LogInformation($"Datos extraídos en: {randomPathDest}");


        // Aquí iría la lógica real de despliegue (IIS, copiar archivos, etc.)

        // if (Directory.Exists(randomPathSource))
        // {
        //     Directory.Delete(randomPathSource, true);
        // }

        // if (Directory.Exists(randomPathDest))
        // {
        //     Directory.Delete(randomPathDest, true);
        // }
    }
}

// TODO: only accept zip files
public class AppDeployRequest
{
    [Required] public IFormFile File { get; set; }

    [Required] public string NameApp { get; set; }
    public string PoolName { get; set; }
}