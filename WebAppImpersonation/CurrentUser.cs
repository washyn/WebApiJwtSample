using System.Security.Claims;

using Volo.Abp.DependencyInjection;


namespace WebAppImpersonation;

public static class CustomClaimTypes
{
    public static string ImpersonatorUserId { get; set; } = "impersonator_userid";
    public static string ImpersonatorUserName { get; set; } = "impersonator_username";
    public static string Role { get; set; } = ClaimTypes.Role;
    public static string UserId { get; set; } = ClaimTypes.NameIdentifier;
}

// add prop for verificate if user is impersonate
public interface ICurrentUser : ITransientDependency
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? UserName { get; }
    string? Name { get; }
    string? SurName { get; }
    string? PhoneNumber { get; }
    bool PhoneNumberVerified { get; }
    string? Email { get; }

    bool EmailVerified { get; }

    // Guid? TenantId { get; }
    string[] Roles { get; }

    Claim? FindClaim(string claimType);

    // Claim[] FindClaims(string claimType);
    // Claim[] GetAllClaims();
    bool IsInRole(string roleName);
}

public class CurrentUser : ICurrentUser
{
    private static readonly Claim[] EmptyClaimsArray = new Claim[0];

    public virtual bool IsAuthenticated => Id.HasValue;

    public virtual Guid? Id => _claimsPrincipal?.FindUserId();

    public virtual string? UserName => this.FindClaimValue(ClaimTypes.Name); // fix 

    public virtual string? Name => this.FindClaimValue(ClaimTypes.Name);

    public virtual string? SurName => this.FindClaimValue("ClaimTypes.SurName"); // fix

    public virtual string? PhoneNumber => this.FindClaimValue("ClaimTypes.PhoneNumber"); // fix

    public virtual bool PhoneNumberVerified => string.Equals(this.FindClaimValue("ClaimTypes.PhoneNumberVerified"),
        "true", StringComparison.InvariantCultureIgnoreCase); // fix

    public virtual string? Email => this.FindClaimValue(ClaimTypes.Email);

    public virtual bool EmailVerified => string.Equals(this.FindClaimValue("ClaimTypes.EmailVerified"), "true",
        StringComparison.InvariantCultureIgnoreCase); // fix

    public virtual string[] Roles => FindClaims(ClaimTypes.Role).Select(c => c.Value).Distinct().ToArray();

    private readonly ClaimsPrincipal? _claimsPrincipal;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _claimsPrincipal = httpContextAccessor.HttpContext?.User;
    }

    public virtual Claim? FindClaim(string claimType)
    {
        return _claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public virtual Claim[] FindClaims(string claimType)
    {
        return _claimsPrincipal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
    }

    public virtual Claim[] GetAllClaims()
    {
        return _claimsPrincipal?.Claims.ToArray() ?? EmptyClaimsArray;
    }

    public virtual bool IsInRole(string roleName)
    {
        return FindClaims(CustomClaimTypes.Role).Any(c => c.Value == roleName);
    }
}
