using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebAppServer.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Auth(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Auth(AuthViewModel model, string? returnUrl = null)
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
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) == false)
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) == false)
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}

public class AuthViewModel
{
    public required string User { get; set; }
    public required string Password { get; set; }
}
