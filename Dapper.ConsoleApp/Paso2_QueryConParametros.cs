using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso2_QueryConParametros
{
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
}
