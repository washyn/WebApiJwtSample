using System.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebAppClient.Models;

namespace WebAppClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult SignIn()
    {
        var serverBaseUrl = _configuration["SSOServerBaseUrl"] ?? "https://localhost:7053";
        var returnUrl = Url.Action("Index", "Home", null, Request.Scheme);
        return Redirect($"{serverBaseUrl}/Account/Auth?returnUrl={Uri.EscapeDataString(returnUrl!)}");
    }

    [HttpPost]
    public new async Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var serverBaseUrl = _configuration["SSOServerBaseUrl"] ?? "https://localhost:7053";
        var returnUrl = Url.Action("Index", "Home", null, Request.Scheme);
        return Redirect($"{serverBaseUrl}/Account/Logout?returnUrl={Uri.EscapeDataString(returnUrl!)}");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
