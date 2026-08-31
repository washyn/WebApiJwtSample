using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppClient.Pages.Account;

public class SsoLogoutModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SsoLogoutModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> OnGet(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync("Identity.Application");

        var identityBaseUrl = _configuration["Sso:IdentityBaseUrl"] ??
                              throw new InvalidOperationException("Sso:IdentityBaseUrl no configurado.");

        var clientBaseUrl = _configuration["Sso:ClientBaseUrl"] ??
                            throw new InvalidOperationException("Sso:ClientBaseUrl no configurado.");

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        var absoluteReturnUrl = $"{clientBaseUrl.TrimEnd('/')}{returnUrl}";
        var logoutUrl = $"{identityBaseUrl.TrimEnd('/')}/Identity/Account/Logout?returnUrl={Uri.EscapeDataString(absoluteReturnUrl)}";

        return Redirect(logoutUrl);
    }
}
