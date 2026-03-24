using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppImpersonation.Pages;

[Authorize]
public class ImpersonateModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public ImpersonateModel(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID is required.");
        }

        // Prevent nested impersonation
        if (User.HasClaim(c => c.Type == CustomClaimTypes.ImpersonatorUserId))
        {
            return BadRequest("Nested impersonation is not allowed. Please stop current impersonation first.");
        }

        // 1. Get current admin user
        var adminUser = await _userManager.GetUserAsync(User);
        if (adminUser == null)
        {
            return Challenge();
        }

        // 2. Get target user
        var targetUser = await _userManager.FindByIdAsync(userId);
        if (targetUser == null)
        {
            return NotFound($"User with ID {userId} not found.");
        }

        // 3. Create claims for the impersonation session
        // We include a claim to track who the real impersonator is
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.ImpersonatorUserId, adminUser.Id),
            new(CustomClaimTypes.ImpersonatorUserName, adminUser.UserName ?? adminUser.Email ?? "Admin")
        };

        // 4. Sign out current session and sign in as target user with extra claims
        await _signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);

        // Use SignInWithClaimsAsync to include the impersonator claims in the new cookie
        await _signInManager.SignInWithClaimsAsync(targetUser, isPersistent: false, claims);

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostStopImpersonationAsync()
    {
        // 1. Check if we are actually impersonating
        var impersonatorId = User.FindFirstValue(CustomClaimTypes.ImpersonatorUserId);
        if (string.IsNullOrEmpty(impersonatorId))
        {
            return BadRequest("Not currently impersonating.");
        }

        // 2. Get the original admin user
        var adminUser = await _userManager.FindByIdAsync(impersonatorId);
        if (adminUser == null)
        {
            // If admin user no longer exists, just sign out
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }

        // 3. Sign back in as the original admin user
        await _signInManager.Context.SignOutAsync(IdentityConstants.ApplicationScheme);
        await _signInManager.SignInAsync(adminUser, isPersistent: false);

        return RedirectToPage("/Index");
    }
}

public static class CustomClaimTypes
{
    public const string ImpersonatorUserId = "impersonator_userid";
    public const string ImpersonatorUserName = "impersonator_username";
}
