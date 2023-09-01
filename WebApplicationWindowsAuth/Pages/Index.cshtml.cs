using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplicationWindowsAuth.Pages;
[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IHttpContextAccessor _contextAccessor;

    public IndexModel(ILogger<IndexModel> logger, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    public void OnGet()
    {
        _logger.LogInformation("context identity name", _contextAccessor.HttpContext?.User.Identity?.Name);
        _logger.LogInformation(_contextAccessor.HttpContext?.User.Identity?.AuthenticationType);
        _logger.LogInformation(_contextAccessor.HttpContext?.User.Identity?.IsAuthenticated.ToString());
        _logger.LogInformation(_contextAccessor.HttpContext?.User.IsInRole("admin").ToString());
    }
}