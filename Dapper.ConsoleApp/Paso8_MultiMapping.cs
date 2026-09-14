using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso8_MultiMapping
{
    private readonly string _conn = Consts.connString;

    public List<AspNetUserWithRole> ObtenerUsuariosConRoles_Simple()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT
                    u.Id   AS UserId,
                    u.UserName,
                    u.Email,
                    u.EmailConfirmed,
                    r.Id   AS RoleId,
                    r.Name AS RoleName
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles      r  ON r.Id = ur.RoleId
                ORDER BY u.UserName, r.Name";

            return db.Query<AspNetUserWithRole>(sql).ToList();
        }
    }

    public class UsuarioConRoles
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
    }

    public List<UsuarioConRoles> ObtenerUsuariosConRoles_Diccionario()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT
                    u.Id   AS UserId,
                    u.UserName,
                    u.Email,
                    r.Id,
                    r.Name,
                    r.NormalizedName,
                    r.ConcurrencyStamp
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetRoles      r  ON r.Id = ur.RoleId
                ORDER BY u.UserName, r.Name";

            var userMap = new Dictionary<string, UsuarioConRoles>();

            var result = db.Query<UsuarioConRoles, AspNetRole, UsuarioConRoles>(
                sql,
                (user, role) =>
                {
                    if (!userMap.TryGetValue(user.UserId, out var currentUser))
                    {
                        currentUser = user;
                        currentUser.Roles = new List<AspNetRole>();
                        userMap.Add(currentUser.UserId, currentUser);
                    }

                    if (role != null && role.Id != null)
                    {
                        if (!currentUser.Roles.Any(r => r.Id == role.Id))
                            currentUser.Roles.Add(role);
                    }

                    return currentUser;
                },
                splitOn: "Id"
            );

            return result.Distinct().ToList();
        }
    }
}
