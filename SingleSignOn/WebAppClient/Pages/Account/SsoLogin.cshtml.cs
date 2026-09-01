using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppClient.Pages.Account;

public class SsoLoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SsoLoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        var identityBaseUrl = _configuration["Sso:IdentityBaseUrl"] ??
                              throw new InvalidOperationException("Sso:IdentityBaseUrl no configurado.");

        var clientBaseUrl = _configuration["Sso:ClientBaseUrl"] ??
                            throw new InvalidOperationException("Sso:ClientBaseUrl no configurado.");

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        var absoluteReturnUrl = $"{clientBaseUrl.TrimEnd('/')}{returnUrl}";
        var loginUrl = $"{identityBaseUrl.TrimEnd('/')}/Identity/Account/Login?returnUrl={Uri.EscapeDataString(absoluteReturnUrl)}";

        return Redirect(loginUrl);
    }
}
