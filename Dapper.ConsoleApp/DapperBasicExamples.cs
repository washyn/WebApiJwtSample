using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Volo.Abp.DependencyInjection;

namespace Dapper.ConsoleApp;

public class DapperBasicExamples : ITransientDependency
{
    private readonly IDbConnection _connection;

    public DapperBasicExamples(IDbConnection connection)
    {
        _connection = connection;
    }

    #region 1. Query con parámetros (SQL Injection prevention)

    public AspNetUser GetUserById(string userId)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
        return _connection.QueryFirstOrDefault<AspNetUser>(sql, new { Id = userId });
    }

    public List<AspNetUser> GetUsersByEmailConfirmed(bool emailConfirmed)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
        return _connection.Query<AspNetUser>(sql, new { EmailConfirmed = emailConfirmed }).ToList();
    }

    public List<AspNetUser> GetUsersWithLockoutEnabled(bool lockoutEnabled, int maxFailedCount)
    {
        var sql = @"
            SELECT * FROM AspNetUsers
            WHERE LockoutEnabled = @LockoutEnabled
              AND AccessFailedCount >= @MaxFailedCount";

        var parameters = new
        {
            LockoutEnabled = lockoutEnabled,
            MaxFailedCount = maxFailedCount
        };

        return _connection.Query<AspNetUser>(sql, parameters).ToList();
    }

    public List<AspNetUser> GetUsersByEmailDomain(string emailDomain)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Email LIKE @EmailDomain";
        return _connection.Query<AspNetUser>(sql, new { EmailDomain = "%" + emailDomain + "%" }).ToList();
    }

    #endregion

    #region 2. QueryFirst, QuerySingle y sus variantes OrDefault

    public AspNetUser GetUserByUserName_QueryFirst(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        var sql = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
        return _connection.QueryFirst<AspNetUser>(sql, new { NormalizedUserName = normalizedUserName });
    }

    public AspNetUser GetUserByUserName_QueryFirstOrDefault(string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        var sql = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";
        return _connection.QueryFirstOrDefault<AspNetUser>(sql, new { NormalizedUserName = normalizedUserName });
    }

    public AspNetUser GetUserById_QuerySingle(string userId)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
        return _connection.QuerySingle<AspNetUser>(sql, new { Id = userId });
    }

    public AspNetUser GetUserById_QuerySingleOrDefault(string userId)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
        return _connection.QuerySingleOrDefault<AspNetUser>(sql, new { Id = userId });
    }

    public AspNetRole GetRoleByName_QuerySingleOrDefault(string roleName)
    {
        var normalized = roleName.ToUpperInvariant();
        var sql = "SELECT * FROM AspNetRoles WHERE NormalizedName = @NormalizedName";
        return _connection.QuerySingleOrDefault<AspNetRole>(sql, new { NormalizedName = normalized });
    }

    #endregion

    #region 3. Execute - INSERT, UPDATE, DELETE (Tablas Identity)

    public int InsertRole(AspNetRole role)
    {
        var sql = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
        return _connection.Execute(sql, role);
    }

    public int UpdateUserSecurityStamp(string userId, string newSecurityStamp)
    {
        var sql = "UPDATE AspNetUsers SET SecurityStamp = @SecurityStamp WHERE Id = @Id";
        return _connection.Execute(sql, new { Id = userId, SecurityStamp = newSecurityStamp });
    }

    public int UpdateUserLockoutEnd(string userId, DateTimeOffset? newLockoutEnd)
    {
        var sql = "UPDATE AspNetUsers SET LockoutEnd = @LockoutEnd WHERE Id = @Id";
        return _connection.Execute(sql, new { Id = userId, LockoutEnd = newLockoutEnd });
    }

    public int IncrementAccessFailedCount(string userId)
    {
        var sql = "UPDATE AspNetUsers SET AccessFailedCount = AccessFailedCount + 1 WHERE Id = @Id";
        return _connection.Execute(sql, new { Id = userId });
    }

    public int InsertUserClaim(AspNetUserClaim claim)
    {
        var sql = @"
            INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
            VALUES (@UserId, @ClaimType, @ClaimValue);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return _connection.QuerySingle<int>(sql, claim);
    }

    public int DeleteUserClaim(int claimId)
    {
        var sql = "DELETE FROM AspNetUserClaims WHERE Id = @ClaimId";
        return _connection.Execute(sql, new { ClaimId = claimId });
    }

    public int AssignRoleToUser(string userId, string roleId)
    {
        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
            INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
        return _connection.Execute(sql, new { UserId = userId, RoleId = roleId });
    }

    public int RemoveRoleFromUser(string userId, string roleId)
    {
        var sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
        return _connection.Execute(sql, new { UserId = userId, RoleId = roleId });
    }

    public int InsertMultipleUserClaims(IEnumerable<AspNetUserClaim> claims)
    {
        var sql = @"
            INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
            VALUES (@UserId, @ClaimType, @ClaimValue)";
        return _connection.Execute(sql, claims);
    }

    #endregion

    #region 4. ExecuteScalar - Obtener un solo valor

    public int GetTotalUsersCount()
    {
        var sql = "SELECT COUNT(*) FROM AspNetUsers";
        return _connection.ExecuteScalar<int>(sql);
    }

    public int GetTotalRolesCount()
    {
        var sql = "SELECT COUNT(*) FROM AspNetRoles";
        return _connection.ExecuteScalar<int>(sql);
    }

    public int GetTotalUsersWithEmailConfirmed()
    {
        var sql = "SELECT COUNT(*) FROM AspNetUsers WHERE EmailConfirmed = 1";
        return _connection.ExecuteScalar<int>(sql);
    }

    public DateTimeOffset? GetMaxLockoutEnd()
    {
        var sql = "SELECT MAX(LockoutEnd) FROM AspNetUsers";
        return _connection.ExecuteScalar<DateTimeOffset?>(sql);
    }

    public int GetMaxAccessFailedCount()
    {
        var sql = "SELECT ISNULL(MAX(AccessFailedCount), 0) FROM AspNetUsers";
        return _connection.ExecuteScalar<int>(sql);
    }

    public UserClaimsSummary GetUserClaimsSummary(string userId)
    {
        var sql = @"
            SELECT
                u.Id AS UserId,
                u.UserName AS UserName,
                (SELECT COUNT(*) FROM AspNetUserClaims c WHERE c.UserId = u.Id) AS TotalClaims,
                (SELECT COUNT(*) FROM AspNetUserRoles r WHERE r.UserId = u.Id) AS TotalRoles
            FROM AspNetUsers u
            WHERE u.Id = @UserId";

        return _connection.QuerySingle<UserClaimsSummary>(sql, new { UserId = userId });
    }

    #endregion

    #region 5. DynamicParameters (Consultas dinámicas)

    public List<AspNetUser> SearchUsers_DynamicParameters(
        string userName = null,
        string email = null,
        bool? emailConfirmed = null,
        bool? lockoutEnabled = null,
        int? minAccessFailedCount = null)
    {
        var parameters = new DynamicParameters();
        var sql = "SELECT * FROM AspNetUsers WHERE 1=1";

        if (!string.IsNullOrWhiteSpace(userName))
        {
            sql += " AND NormalizedUserName LIKE @UserName";
            parameters.Add("UserName", "%" + userName.ToUpperInvariant() + "%", DbType.String);
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            sql += " AND Email LIKE @Email";
            parameters.Add("Email", "%" + email + "%", DbType.String);
        }

        if (emailConfirmed.HasValue)
        {
            sql += " AND EmailConfirmed = @EmailConfirmed";
            parameters.Add("EmailConfirmed", emailConfirmed.Value, DbType.Boolean);
        }

        if (lockoutEnabled.HasValue)
        {
            sql += " AND LockoutEnabled = @LockoutEnabled";
            parameters.Add("LockoutEnabled", lockoutEnabled.Value, DbType.Boolean);
        }

        if (minAccessFailedCount.HasValue)
        {
            sql += " AND AccessFailedCount >= @MinFailed";
            parameters.Add("MinFailed", minAccessFailedCount.Value, DbType.Int32);
        }

        return _connection.Query<AspNetUser>(sql, parameters).ToList();
    }

    public int InsertUserLogin_WithCheck(AspNetUserLogin login)
    {
        var parameters = new DynamicParameters();
        parameters.AddDynamicParams(login);
        parameters.Add("@WasInserted", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var sql = @"
            IF NOT EXISTS (
                SELECT 1 FROM AspNetUserLogins
                WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey
            )
            BEGIN
                INSERT INTO AspNetUserLogins (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId)
                SET @WasInserted = 1
            END
            ELSE
            BEGIN
                SET @WasInserted = 0
            END";

        _connection.Execute(sql, parameters);
        return parameters.Get<int>("@WasInserted");
    }

    #endregion

    #region 6. Stored Procedures (Identity patterns)

    public List<AspNetUser> sp_GetUsersByEmailConfirmed(bool emailConfirmed)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@EmailConfirmed", emailConfirmed);

        return _connection.Query<AspNetUser>(
            "sp_GetUsersByEmailConfirmed",
            parameters,
            commandType: CommandType.StoredProcedure
        ).ToList();
    }

    public List<AspNetRole> sp_GetRolesForUser(string userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        return _connection.Query<AspNetRole>(
            "sp_GetRolesForUser",
            parameters,
            commandType: CommandType.StoredProcedure
        ).ToList();
    }

    public int sp_IncrementAccessFailed(string userId, out bool isLockedOut)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@IsLockedOut", dbType: DbType.Boolean, direction: ParameterDirection.Output);

        var result = _connection.Execute(
            "sp_IncrementAccessFailed",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        isLockedOut = parameters.Get<bool>("@IsLockedOut");
        return result;
    }

    #endregion

    #region 7. Async / Await (Identity)

    public async Task<List<AspNetUser>> GetUsersAsync()
    {
        var sql = "SELECT * FROM AspNetUsers";
        var result = await _connection.QueryAsync<AspNetUser>(sql);
        return result.ToList();
    }

    public async Task<AspNetUser> GetUserByIdAsync(string userId)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<AspNetUser>(sql, new { Id = userId });
    }

    public async Task<List<AspNetRole>> GetRolesAsync()
    {
        var sql = "SELECT * FROM AspNetRoles ORDER BY Name";
        var result = await _connection.QueryAsync<AspNetRole>(sql);
        return result.ToList();
    }

    public async Task<int> InsertRoleAsync(AspNetRole role)
    {
        var sql = @"
            INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
        return await _connection.ExecuteAsync(sql, role);
    }

    public async Task<int> UpdateUserEmailConfirmedAsync(string userId, bool confirmed)
    {
        var sql = "UPDATE AspNetUsers SET EmailConfirmed = @Confirmed WHERE Id = @Id";
        return await _connection.ExecuteAsync(sql, new { Id = userId, Confirmed = confirmed });
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        var sql = "SELECT COUNT(*) FROM AspNetUsers";
        return await _connection.ExecuteScalarAsync<int>(sql);
    }

    #endregion

    #region 8. IN Clauses (Dapper lo hace automáticamente)

    public List<AspNetUser> GetUsersByIdList(IEnumerable<string> userIds)
    {
        var sql = "SELECT * FROM AspNetUsers WHERE Id IN @UserIds";
        return _connection.Query<AspNetUser>(sql, new { UserIds = userIds }).ToList();
    }

    public List<AspNetRole> GetRolesByIdList(IEnumerable<string> roleIds)
    {
        var sql = "SELECT * FROM AspNetRoles WHERE Id IN @RoleIds";
        return _connection.Query<AspNetRole>(sql, new { RoleIds = roleIds }).ToList();
    }

    public List<AspNetUserClaim> GetClaimsByIdList(IEnumerable<int> claimIds)
    {
        var sql = "SELECT * FROM AspNetUserClaims WHERE Id IN @ClaimIds";
        return _connection.Query<AspNetUserClaim>(sql, new { ClaimIds = claimIds }).ToList();
    }

    public int DeleteMultipleClaims(IEnumerable<int> claimIds)
    {
        var sql = "DELETE FROM AspNetUserClaims WHERE Id IN @ClaimIds";
        return _connection.Execute(sql, new { ClaimIds = claimIds });
    }

    #endregion

    #region 9. Consultas específicas a tablas Identity secundarias

    public List<AspNetUserClaim> GetClaimsForUser(string userId)
    {
        var sql = "SELECT * FROM AspNetUserClaims WHERE UserId = @UserId ORDER BY ClaimType";
        return _connection.Query<AspNetUserClaim>(sql, new { UserId = userId }).ToList();
    }

    public List<AspNetRoleClaim> GetClaimsForRole(string roleId)
    {
        var sql = "SELECT * FROM AspNetRoleClaims WHERE RoleId = @RoleId ORDER BY ClaimType";
        return _connection.Query<AspNetRoleClaim>(sql, new { RoleId = roleId }).ToList();
    }

    public List<AspNetUserLogin> GetLoginsForUser(string userId)
    {
        var sql = "SELECT * FROM AspNetUserLogins WHERE UserId = @UserId";
        return _connection.Query<AspNetUserLogin>(sql, new { UserId = userId }).ToList();
    }

    public List<AspNetUserToken> GetTokensForUser(string userId)
    {
        var sql = "SELECT * FROM AspNetUserTokens WHERE UserId = @UserId";
        return _connection.Query<AspNetUserToken>(sql, new { UserId = userId }).ToList();
    }

    public int UpsertUserToken(AspNetUserToken token)
    {
        var sql = @"
            IF EXISTS (
                SELECT 1 FROM AspNetUserTokens
                WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name
            )
                UPDATE AspNetUserTokens SET Value = @Value
                WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND Name = @Name
            ELSE
                INSERT INTO AspNetUserTokens (UserId, LoginProvider, Name, Value)
                VALUES (@UserId, @LoginProvider, @Name, @Value)";

        return _connection.Execute(sql, token);
    }

    #endregion
}
