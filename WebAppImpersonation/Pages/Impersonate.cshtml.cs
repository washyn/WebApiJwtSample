using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// using Volo.Abp.Security.Claims;
// using Volo.Abp.Users;


namespace WebAppImpersonation.Pages;

// auth scheme IdentityConstants.ApplicationScheme
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
        // var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(targetUser);
        // add claim for recognize original user id

        var extraClaims = new List<Claim>()
        {
            new Claim(CustomClaimTypes.ImpersonatorUserId, _currentUser.Id.ToString()),
            new Claim(CustomClaimTypes.ImpersonatorUserName, _currentUser.Name),
        };

        await _signInManager.SignOutAsync();
        await _signInManager.SignInWithClaimsAsync(targetUser, false, extraClaims);
        return RedirectToPage("/Index");
        // end

        // // 3. Create claims for the impersonation session
        // // We include a claim to track who the real impersonator is
        // var claims = new List<Claim>
        // {
        //     new(CustomClaimTypes.ImpersonatorUserId, adminUser.Id),
        //     new(CustomClaimTypes.ImpersonatorUserName, adminUser.UserName ?? adminUser.Email ?? "Admin")
        // };
        // // 4. Sign out current session and sign in as target user with extra claims
        // await _signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);
        // // Use SignInWithClaimsAsync to include the impersonator claims in the new cookie
        // await _signInManager.SignInWithClaimsAsync(targetUser, isPersistent: false, claims);
        // return RedirectToPage("/Index");


        // var user = await _userManager.FindByIdAsync(userId);
        // var impersonatorUserId = _currentUser.Id;
        // await _signInManager.SignOutAsync();
        // var claims = new List<Claim> { new(CustomClaimTypes.ImpersonatorUserId, impersonatorUserId!.ToString()!), };
        // await _signInManager.UserManager.AddClaimsAsync(user!, claims);
        // await _signInManager.SignInAsync(user!, isPersistent: true);
    }

    public async Task<IActionResult> OnPostStopImpersonationAsync()
    {
        // // 1. Check if we are actually impersonating
        // var impersonatorId = User.FindFirstValue(CustomClaimTypes.ImpersonatorUserId);
        // if (string.IsNullOrEmpty(impersonatorId))
        // {
        //     return BadRequest("Not currently impersonating.");
        // }
        // // 2. Get the original admin user
        // var adminUser = await _userManager.FindByIdAsync(impersonatorId);
        // if (adminUser == null)
        // {
        //     // If admin user no longer exists, just sign out
        //     await _signInManager.SignOutAsync();
        //     return RedirectToPage("/Index");
        // }
        // // 3. Sign back in as the original admin user
        // await _signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);
        // await _signInManager.SignInAsync(adminUser, isPersistent: false);
        // return RedirectToPage("/Index");

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