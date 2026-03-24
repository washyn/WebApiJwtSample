using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

// using Volo.Abp.Security.Claims;
// using Volo.Abp.Users;


namespace WebAppImpersonation.Pages;

[Authorize]
public class ImpersonateModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ICurrentUser _currentUser;
    private readonly SignInManager<IdentityUser> _signInManager;

    public ImpersonateModel(UserManager<IdentityUser> userManager, ICurrentUser currentUser,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _signInManager = signInManager;
    }

    public async Task OnGetAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var impersonatorUserId = _currentUser.Id;
        await _signInManager.SignOutAsync();
        var claims = new List<Claim> { new(CustomClaimTypes.ImpersonatorUserId, impersonatorUserId!.ToString()!), };
        await _signInManager.UserManager.AddClaimsAsync(user!, claims);
        await _signInManager.SignInAsync(user!, isPersistent: true);
    }
}