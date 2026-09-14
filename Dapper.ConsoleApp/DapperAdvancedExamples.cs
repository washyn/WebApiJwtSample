using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Volo.Abp.DependencyInjection;

namespace Dapper.ConsoleApp;

public class DapperAdvancedExamples : ITransientDependency
{
    private readonly IDbConnection _connection;

    public DapperAdvancedExamples(IDbConnection connection)
    {
        _connection = connection;
    }

    #region 1. Multi-Mapping Relación 1 a N: Usuario con Roles (JOIN simple)

    public List<AspNetUserWithRole> GetUsersWithRoles_SimpleJoin()
    {
        var sql = @"
            SELECT
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.EmailConfirmed,
                r.Id AS RoleId,
                r.Name AS RoleName
            FROM AspNetUsers u
            INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            ORDER BY u.UserName, r.Name";

        return _connection.Query<AspNetUserWithRole>(sql).ToList();
    }

    #endregion

    #region 2. Multi-Mapping con SplitOn: Objeto Usuario anidado con Rol

    public List<AspNetUser> GetUsersWithRoles_SplitOn()
    {
        var sql = @"
            SELECT
                u.Id, u.UserName, u.NormalizedUserName, u.Email, u.NormalizedEmail,
                u.EmailConfirmed, u.PasswordHash, u.SecurityStamp, u.ConcurrencyStamp,
                u.PhoneNumber, u.PhoneNumberConfirmed, u.TwoFactorEnabled,
                u.LockoutEnd, u.LockoutEnabled, u.AccessFailedCount,
                r.Id, r.Name, r.NormalizedName, r.ConcurrencyStamp
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
            ORDER BY u.UserName";

        var userDictionary = new Dictionary<string, AspNetUser>();

        var result = _connection.Query<AspNetUser, AspNetRole, AspNetUser>(
            sql,
            (user, role) =>
            {
                if (!userDictionary.TryGetValue(user.Id, out var userEntry))
                {
                    userEntry = user;
                    userDictionary.Add(user.Id, userEntry);
                }

                return userEntry;
            },
            splitOn: "Id"
        ).Distinct().ToList();

        return result;
    }

    #endregion

    #region 3. Multi-Mapping 3 niveles: Usuario -> Roles -> RoleClaims

    public class UserWithRolesAndClaims
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<RoleWithClaims> Roles { get; set; } = new();
    }

    public class RoleWithClaims
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AspNetRoleClaim> Claims { get; set; } = new();
    }

    public List<UserWithRolesAndClaims> GetUserWithRolesAndRoleClaims_ThreeLevel()
    {
        var sql = @"
            SELECT
                u.Id, u.UserName, u.Email,
                r.Id, r.Name,
                rc.Id, rc.RoleId, rc.ClaimType, rc.ClaimValue
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
            LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
            ORDER BY u.Id, r.Id, rc.Id";

        var userMap = new Dictionary<string, UserWithRolesAndClaims>();
        var roleMap = new Dictionary<string, RoleWithClaims>();

        _connection.Query<UserWithRolesAndClaims, RoleWithClaims, AspNetRoleClaim, UserWithRolesAndClaims>(
            sql,
            (user, role, roleClaim) =>
            {
                if (!userMap.TryGetValue(user.UserId, out var userEntry))
                {
                    userEntry = user;
                    userEntry.Roles = new List<RoleWithClaims>();
                    userMap.Add(userEntry.UserId, userEntry);
                }

                if (role != null && !string.IsNullOrEmpty(role.RoleId))
                {
                    var roleKey = userEntry.UserId + "|" + role.RoleId;
                    if (!roleMap.TryGetValue(roleKey, out var roleEntry))
                    {
                        roleEntry = role;
                        roleEntry.Claims = new List<AspNetRoleClaim>();
                        roleMap.Add(roleKey, roleEntry);
                        userEntry.Roles.Add(roleEntry);
                    }

                    if (roleClaim != null && roleClaim.Id != 0)
                    {
                        roleEntry.Claims.Add(roleClaim);
                    }
                }

                return userEntry;
            },
            splitOn: "Id,Id,Id"
        ).AsList();

        return userMap.Values.ToList();
    }

    #endregion

    #region 4. QueryMultiple - Múltiples resultados en una sola conexión

    public (List<AspNetUser> Users, List<AspNetRole> Roles, int TotalUsers, int TotalRoles)
        GetUsersAndRolesCounts_OneRoundTrip()
    {
        var sql = @"
            SELECT * FROM AspNetUsers ORDER BY UserName;
            SELECT * FROM AspNetRoles ORDER BY Name;
            SELECT COUNT(*) FROM AspNetUsers;
            SELECT COUNT(*) FROM AspNetRoles;";

        using (var multi = _connection.QueryMultiple(sql))
        {
            var users = multi.Read<AspNetUser>().ToList();
            var roles = multi.Read<AspNetRole>().ToList();
            var totalUsers = multi.ReadSingle<int>();
            var totalRoles = multi.ReadSingle<int>();

            return (users, roles, totalUsers, totalRoles);
        }
    }

    public async Task<(List<AspNetUser> Users, List<AspNetRole> Roles)>
        GetUsersAndRoles_OneRoundTripAsync()
    {
        var sql = @"
            SELECT * FROM AspNetUsers ORDER BY UserName;
            SELECT * FROM AspNetRoles ORDER BY Name;";

        using (var multi = await _connection.QueryMultipleAsync(sql))
        {
            var users = (await multi.ReadAsync<AspNetUser>()).ToList();
            var roles = (await multi.ReadAsync<AspNetRole>()).ToList();

            return (users, roles);
        }
    }

    public (AspNetUser User, List<AspNetRole> Roles, List<AspNetUserClaim> Claims,
            List<AspNetUserLogin> Logins, List<AspNetUserToken> Tokens)
        GetUserFullProfile_QueryMultiple(string userId)
    {
        var sql = @"
            SELECT * FROM AspNetUsers WHERE Id = @UserId;
            SELECT r.* FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId;
            SELECT * FROM AspNetUserClaims WHERE UserId = @UserId ORDER BY ClaimType;
            SELECT * FROM AspNetUserLogins WHERE UserId = @UserId;
            SELECT * FROM AspNetUserTokens WHERE UserId = @UserId;";

        using (var multi = _connection.QueryMultiple(sql, new { UserId = userId }))
        {
            var user = multi.ReadSingleOrDefault<AspNetUser>();
            var roles = multi.Read<AspNetRole>().ToList();
            var claims = multi.Read<AspNetUserClaim>().ToList();
            var logins = multi.Read<AspNetUserLogin>().ToList();
            var tokens = multi.Read<AspNetUserToken>().ToList();

            return (user, roles, claims, logins, tokens);
        }
    }

    #endregion

    #region 5. Transacciones (Identity patterns)

    public int CreateRoleWithClaims_Transaction(AspNetRole role, IEnumerable<AspNetRoleClaim> claims)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var insertRoleSql = @"
                    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                    VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                _connection.Execute(insertRoleSql, role, transaction);

                foreach (var claim in claims)
                {
                    claim.RoleId = role.Id;

                    var insertClaimSql = @"
                        INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                        VALUES (@RoleId, @ClaimType, @ClaimValue)";
                    _connection.Execute(insertClaimSql, claim, transaction);
                }

                transaction.Commit();
                return 1;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public async Task AssignUserToRoles_TransactionAsync(string userId, IEnumerable<string> roleIds)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var clearRolesSql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId";
                await _connection.ExecuteAsync(clearRolesSql, new { UserId = userId }, transaction);

                var assignSql = @"
                    IF NOT EXISTS (
                        SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId
                    )
                    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";

                foreach (var roleId in roleIds)
                {
                    await _connection.ExecuteAsync(assignSql, new { UserId = userId, RoleId = roleId }, transaction);
                }

                var resetAccessSql = @"
                    UPDATE AspNetUsers
                    SET AccessFailedCount = 0,
                        LockoutEnd = NULL
                    WHERE Id = @UserId";
                await _connection.ExecuteAsync(resetAccessSql, new { UserId = userId }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    public void ReplaceUserClaims_Transaction(string userId, IEnumerable<AspNetUserClaim> newClaims)
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        using (var transaction = _connection.BeginTransaction())
        {
            try
            {
                var deleteSql = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId";
                _connection.Execute(deleteSql, new { UserId = userId }, transaction);

                if (newClaims != null && newClaims.Any())
                {
                    foreach (var claim in newClaims)
                    {
                        claim.UserId = userId;
                    }

                    var insertSql = @"
                        INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                        VALUES (@UserId, @ClaimType, @ClaimValue)";
                    _connection.Execute(insertSql, newClaims, transaction);
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }

    #endregion

    #region 6. Buffered vs Non-Buffered

    public List<AspNetUser> GetUsers_Buffered()
    {
        var sql = "SELECT * FROM AspNetUsers";
        return _connection.Query<AspNetUser>(sql, buffered: true).ToList();
    }

    public IEnumerable<AspNetUser> GetUsers_NonBuffered()
    {
        var sql = "SELECT * FROM AspNetUsers";
        return _connection.Query<AspNetUser>(sql, buffered: false);
    }

    public void ProcessUsers_StreamingExample()
    {
        var sql = "SELECT * FROM AspNetUsers";

        foreach (var user in _connection.Query<AspNetUser>(sql, buffered: false))
        {
            Console.WriteLine("Procesando usuario: " + user.UserName);
        }
    }

    #endregion

    #region 7. Mapeo Dinámico (dynamic)

    public List<dynamic> GetUsersDynamic()
    {
        var sql = "SELECT Id, UserName, Email, EmailConfirmed FROM AspNetUsers WHERE EmailConfirmed = 1";
        return _connection.Query(sql).AsList();
    }

    public List<dynamic> GetUsersWithRoleCount_Dynamic()
    {
        var sql = @"
            SELECT
                u.UserName,
                u.Email,
                (SELECT COUNT(*) FROM AspNetUserRoles ur WHERE ur.UserId = u.Id) AS RoleCount
            FROM AspNetUsers u";
        return _connection.Query(sql).AsList();
    }

    public void ProcessDynamicExample()
    {
        var users = GetUsersWithRoleCount_Dynamic();
        foreach (var u in users)
        {
            Console.WriteLine(u.UserName + " - Roles: " + u.RoleCount);
        }
    }

    #endregion

    #region 8. Ejemplo práctico: Usuarios bloqueados con información extendida

    public class LockedOutUserDetail
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public int RoleCount { get; set; }
        public int ClaimCount { get; set; }
    }

    public List<LockedOutUserDetail> GetLockedOutUsers_WithAggregates()
    {
        var sql = @"
            SELECT
                u.Id AS UserId,
                u.UserName,
                u.Email,
                u.LockoutEnd,
                u.AccessFailedCount,
                (SELECT COUNT(*) FROM AspNetUserRoles ur WHERE ur.UserId = u.Id) AS RoleCount,
                (SELECT COUNT(*) FROM AspNetUserClaims c WHERE c.UserId = u.Id) AS ClaimCount
            FROM AspNetUsers u
            WHERE u.LockoutEnd IS NOT NULL
              AND u.LockoutEnd > GETUTCDATE()
            ORDER BY u.LockoutEnd DESC";

        return _connection.Query<LockedOutUserDetail>(sql).ToList();
    }

    #endregion
}
