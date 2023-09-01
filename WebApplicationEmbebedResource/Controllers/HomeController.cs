using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using WebApplicationEmbebedResource.Models;

namespace WebApplicationEmbebedResource.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var manifestEmbeddedProvider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly);
        var file = manifestEmbeddedProvider.GetFileInfo("Views/bootstrap.css");
        if (file.Exists)
        {
            var content = file.ReadAsString();
            ViewData["style"] = content;
        }
        return View();
    }

    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}