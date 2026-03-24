using System.Security.Claims;

using JetBrains.Annotations;

using Volo.Abp;

namespace WebAppImpersonation;

public static class CurrentUserExtensions
{
    public static Guid? FindImpersonatorUserId(this ICurrentUser currentUser)
    {
        var impersonatorUserId = currentUser.FindClaimValue(CustomClaimTypes.ImpersonatorUserId);
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
        return currentUser.FindClaimValue(CustomClaimTypes.ImpersonatorUserName);
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

public static class AbpClaimsIdentityExtensions
{
    public static Guid? FindUserId([NotNull] this ClaimsPrincipal principal)
    {
        Check.NotNull(principal, nameof(principal));

        var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId);
        if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Guid.TryParse(userIdOrNull.Value, out Guid guid))
        {
            return guid;
        }

        return null;
    }
}
