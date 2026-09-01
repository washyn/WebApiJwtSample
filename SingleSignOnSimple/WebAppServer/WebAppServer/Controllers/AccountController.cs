using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebAppServer.Controllers;

public class AccountController : Controller
{

    [HttpGet]
    public IActionResult Auth()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Auth(AuthViewModel model)
    {
        if (ModelState.IsValid)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, model.User),
                new Claim(ClaimTypes.NameIdentifier, model.User),
                new Claim(ClaimTypes.Role, "rol1"),
                new Claim(ClaimTypes.Role, "rol2"),
                new Claim(ClaimTypes.Role, "rol3"),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction("Index", "Home");
        }
        return View();
    }
    
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}

public class AuthViewModel
{
    public string User { get; set; }
    public string Password { get; set; }
}
