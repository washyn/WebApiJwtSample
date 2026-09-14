using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using JetBrains.Annotations;

using Volo.Abp.DependencyInjection;

public class NewWay : ITransientDependency
{
    private readonly IDbConnection _connection;

    public NewWay(IDbConnection connection)
    {
        _connection = connection;
    }

    public List<AspNetUser> GetUsers()
    {
        return _connection.Query<AspNetUser>("SELECT * FROM AspNetUsers").ToList();
    }
}

public class AspNetUser
{
    public string Id { get; set; }
    [CanBeNull] public string UserName { get; set; }
    [CanBeNull] public string NormalizedUserName { get; set; }
    [CanBeNull] public string Email { get; set; }
    [CanBeNull] public string NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    [CanBeNull] public string PasswordHash { get; set; }
    [CanBeNull] public string SecurityStamp { get; set; }
    [CanBeNull] public string ConcurrencyStamp { get; set; }
    [CanBeNull] public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
}
