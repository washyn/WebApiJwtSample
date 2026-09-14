using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso2_QueryConParametros
{
    public List<AspNetUser> GetUsersByEmailConfirmed(bool emailConfirmed)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
            return db.Query<AspNetUser>(sql, new { EmailConfirmed = emailConfirmed }).ToList();
        }
    }

    public AspNetUser GetUserById(string userId)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.QueryFirstOrDefault<AspNetUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }
    }

    public List<AspNetUser> GetUsersConVariosFiltros(
        bool lockoutEnabled,
        int minFailedAttempts,
        string emailLike)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            var sql = @"
                SELECT * FROM AspNetUsers
                WHERE LockoutEnabled = @LockoutEnabled
                  AND AccessFailedCount >= @MinFailed
                  AND Email LIKE @EmailLike";

            var parameters = new
            {
                LockoutEnabled = lockoutEnabled,
                MinFailed = minFailedAttempts,
                EmailLike = "%" + emailLike + "%"
            };

            return db.Query<AspNetUser>(sql, parameters).ToList();
        }
    }

    public List<AspNetRole> GetRolesByNameStart(string prefix)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.Query<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName LIKE @Prefix",
                new { Prefix = prefix.ToUpperInvariant() + "%" }
            ).ToList();
        }
    }

    public int GetClaimsCountForUser(string userId)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM AspNetUserClaims WHERE UserId = @UserId",
                new { UserId = userId }
            );
        }
    }

    public List<AspNetUser> GetUsersByEmail_Unsafe(string email)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            var sqlUnsafe = "SELECT * FROM AspNetUsers WHERE Email = '" + email + "'";
            return db.Query<AspNetUser>(sqlUnsafe).ToList();
        }
    }
}
