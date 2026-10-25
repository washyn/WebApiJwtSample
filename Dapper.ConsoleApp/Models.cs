using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using JetBrains.Annotations;

namespace Dapper.ConsoleApp;

public class AspNetRole
{
    public string Id { get; set; }
    [CanBeNull] public string Name { get; set; }
    [CanBeNull] public string NormalizedName { get; set; }
    [CanBeNull] public string ConcurrencyStamp { get; set; }
}

public class AspNetUserRole
{
    public string UserId { get; set; }
    public string RoleId { get; set; }
}

public class AspNetUserClaim
{
    public int Id { get; set; }
    public string UserId { get; set; }
    [CanBeNull] public string ClaimType { get; set; }
    [CanBeNull] public string ClaimValue { get; set; }
}

public class AspNetRoleClaim
{
    public int Id { get; set; }
    public string RoleId { get; set; }
    [CanBeNull] public string ClaimType { get; set; }
    [CanBeNull] public string ClaimValue { get; set; }
}

public class AspNetUserLogin
{
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
    [CanBeNull] public string ProviderDisplayName { get; set; }
    public string UserId { get; set; }
}

public class AspNetUserToken
{
    public string UserId { get; set; }
    public string LoginProvider { get; set; }
    public string Name { get; set; }
    [CanBeNull] public string Value { get; set; }
}

public class AspNetUserWithRole
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string RoleId { get; set; }
    public string RoleName { get; set; }
}

public class UserClaimsSummary
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public int TotalClaims { get; set; }
    public int TotalRoles { get; set; }
}
