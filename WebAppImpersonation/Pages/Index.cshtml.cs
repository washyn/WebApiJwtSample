using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using WebAppImpersonation.Data;

namespace WebAppImpersonation.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ICurrentUser _currentUser;
    private readonly ApplicationDbContext _dbContext;

    // Atribute inject only exists for abp framework
    public ICurrentUser CurrentUser { get; set; }
    public List<IdentityUser> Users { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager,
        IHttpContextAccessor contextAccessor,
        ICurrentUser currentUser,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _currentUser = currentUser;
        _dbContext = dbContext;
        CurrentUser = currentUser;
    }

    public void OnGet()
    {
        Users = _dbContext.Users.ToList();
        _logger.LogInformation("Start display Claims");
        foreach (var claim in _contextAccessor.HttpContext?.User.Claims)
        {
            _logger.LogInformation($"{claim.Type}: {claim.Value}");
        }

        _logger.LogInformation("End display Claims");
    }
}
