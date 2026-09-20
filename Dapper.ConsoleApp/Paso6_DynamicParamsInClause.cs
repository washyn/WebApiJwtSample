using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso6_DynamicParamsInClause
{
    private readonly string _conn = Consts.connString;

    public List<AspNetUser> BuscarUsuarios(
        string userName = null,
        string email = null,
        bool? emailConfirmed = null)
    {
        using (IDbConnection db = new SqlConnection(_conn))
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

            sql += " ORDER BY UserName";

            return db.Query<AspNetUser>(sql, parameters).ToList();
        }
    }

    public List<AspNetUser> ObtenerUsuariosPorIds(List<string> userIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id IN @UserIds ORDER BY UserName";
            return db.Query<AspNetUser>(sql, new { UserIds = userIds }).ToList();
        }
    }
}
