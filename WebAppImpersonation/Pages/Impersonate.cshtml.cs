using System.Security.Claims;
using System.Security.Principal;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Volo.Abp.Security.Claims;
using Volo.Abp.Users;


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
        // Impersonate the user with the specified userId
        // get user from id
        var user = await _userManager.FindByIdAsync(userId);
        // _signInManager.AuthenticationScheme // Identity.Application
        // add impersonator claims
        var impersonatorUserId = _currentUser.Id;
        var claims = new List<Claim> { new(CustomClaimTypes.ImpersonatorUserId, impersonatorUserId!.ToString()!), };
        await _signInManager.UserManager.AddClaimsAsync(user!, claims);
        await _signInManager.SignInAsync(user!, isPersistent: true);
    }
}

public static class CustomClaimTypes
{
    public static string ImpersonatorUserId { get; set; } = "impersonator_userid";
    public static string ImpersonatorTenantName { get; set; } = "impersonator_tenantname";
}

public static class CurrentUserExtensions
{
    public static Guid? FindImpersonatorUserId(this ICurrentUser currentUser)
    {
        var impersonatorUserId = currentUser.FindClaimValue(AbpClaimTypes.ImpersonatorUserId);
        if (impersonatorUserId.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Guid.TryParse(impersonatorUserId, out var guid))
        {
            return guid;
        }

        return null;
    }

    public static string? FindImpersonatorUserName(this ICurrentUser currentUser)
    {
        return currentUser.FindClaimValue(AbpClaimTypes.ImpersonatorUserName);
    }

    public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.FindClaim(claimType)?.Value;
    }

    public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
        where T : struct
    {
        var value = currentUser.FindClaimValue(claimType);
        if (value == null)
        {
            return default;
        }

        return value.To<T>();
    }
}

// public class CurrentUser : ICurrentUser
// {
//     private static readonly Claim[] EmptyClaimsArray = new Claim[0];

//     public virtual bool IsAuthenticated => Id.HasValue;

//     public virtual Guid? Id => _principalAccessor.Principal?.FindUserId();

//     public virtual string? UserName => this.FindClaimValue(AbpClaimTypes.UserName);

//     public virtual string? Name => this.FindClaimValue(AbpClaimTypes.Name);

//     public virtual string? SurName => this.FindClaimValue(AbpClaimTypes.SurName);

//     public virtual string? PhoneNumber => this.FindClaimValue(AbpClaimTypes.PhoneNumber);

//     public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.PhoneNumberVerified),
//         "true", StringComparison.InvariantCultureIgnoreCase);

//     public virtual string? Email => this.FindClaimValue(AbpClaimTypes.Email);

//     public virtual bool EmailVerified => string.Equals(this.FindClaimValue(AbpClaimTypes.EmailVerified), "true",
//         StringComparison.InvariantCultureIgnoreCase);

//     public virtual string[] Roles => FindClaims(AbpClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

//     private readonly ICurrentPrincipalAccessor _principalAccessor;

//     public CurrentUser(ICurrentPrincipalAccessor principalAccessor)
//     {
//         _principalAccessor = principalAccessor;
//     }

//     public virtual Claim? FindClaim(string claimType)
//     {
//         return _principalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
//     }

//     public virtual Claim[] FindClaims(string claimType)
//     {
//         return _principalAccessor.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
//     }

//     public virtual Claim[] GetAllClaims()
//     {
//         return _principalAccessor.Principal?.Claims.ToArray() ?? EmptyClaimsArray;
//     }

//     public virtual bool IsInRole(string roleName)
//     {
//         return FindClaims(AbpClaimTypes.Role).Any(c => c.Value == roleName);
//     }
// }
