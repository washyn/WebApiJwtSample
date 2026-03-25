using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppImpersonation.Pages;

// default auth scheme IdentityConstants.ApplicationScheme
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

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        if (!_currentUser.CanImpersonateAs(Guid.Parse(userId)))
        {
            throw new Exception("User can not be impersonate.");
        }

        var targetUser = await _userManager.FindByIdAsync(userId);
        var extraClaims = new List<Claim>()
        {
            new Claim(CustomClaimTypes.ImpersonatorUserId, _currentUser.Id.ToString()),
            new Claim(CustomClaimTypes.ImpersonatorUserName, _currentUser.Name),
        };

        await _signInManager.SignOutAsync();
        await _signInManager.SignInWithClaimsAsync(targetUser, false, extraClaims);
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostStopImpersonationAsync()
    {
        var isImpersonated = _currentUser.IsImpersonated();
        if (!isImpersonated)
        {
            return BadRequest("Not currently impersonating.");
        }

        var impersonatorId = _currentUser.FindImpersonatorUserId();
        var originalUser = await _userManager.FindByIdAsync(impersonatorId.ToString());

        await _signInManager.SignOutAsync();
        await _signInManager.SignInAsync(originalUser, false);
        return RedirectToPage("/Index");
    }
}