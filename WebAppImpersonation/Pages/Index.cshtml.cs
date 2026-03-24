using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using WebAppImpersonation.Data;

namespace WebAppImpersonation.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public List<IdentityUser> Users { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger, UserManager<IdentityUser> userManager,
        ApplicationDbContext dbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public void OnGet()
    {
        Users = _dbContext.Users.ToList();
    }
}
