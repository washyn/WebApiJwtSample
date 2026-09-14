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

// Id                   nvarchar(450) collate SQL_Latin1_General_CP1_CI_AS not null
//     constraint PK_AspNetUsers
//         primary key,
// UserName             nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
// NormalizedUserName   nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
// Email                nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
// NormalizedEmail      nvarchar(256) collate SQL_Latin1_General_CP1_CI_AS,
// EmailConfirmed       bit                                                not null,
// PasswordHash         nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
// SecurityStamp        nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
// ConcurrencyStamp     nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
// PhoneNumber          nvarchar(max) collate SQL_Latin1_General_CP1_CI_AS,
// PhoneNumberConfirmed bit                                                not null,
// TwoFactorEnabled     bit                                                not null,
// LockoutEnd           datetimeoffset,
// LockoutEnabled       bit                                                not null,
// AccessFailedCount    int                                                not null


// AspNetUsers
// Id varchar
// UserName varchar
// NormalizedUserName varchar
// Email varchar
// NormalizedEmail varchar
// EmailConfirmed bit
// PasswordHash varchar
// SecurityStamp varchar
// ConcurrencyStamp varchar
// PhoneNumber varchar
// PhoneNumberConfirmed bit
// TwoFactorEnabled bit
// LockoutEnd datetimeoffset
// LockoutEnabled bit
// AccessFailedCount int