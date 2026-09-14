using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso7_QueryMultiple
{
    private readonly string _conn = Consts.connString;

    public (
        List<AspNetUser> Usuarios,
        List<AspNetRole> Roles,
        int TotalUsuarios,
        int TotalRoles
    ) ObtenerUsuariosYRolesEnUnViaje()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT * FROM AspNetUsers ORDER BY UserName;
                SELECT * FROM AspNetRoles ORDER BY Name;
                SELECT COUNT(*) FROM AspNetUsers;
                SELECT COUNT(*) FROM AspNetRoles;";

            using (var multi = db.QueryMultiple(sql))
            {
                var usuarios = multi.Read<AspNetUser>().ToList();
                var roles    = multi.Read<AspNetRole>().ToList();
                var totalU   = multi.ReadSingle<int>();
                var totalR   = multi.ReadSingle<int>();

                return (usuarios, roles, totalU, totalR);
            }
        }
    }

    public class IdentityDashboard
    {
        public int TotalUsers { get; set; }
        public int UsersEmailConfirmed { get; set; }
        public int UsersLockedOut { get; set; }
        public int TotalRoles { get; set; }
        public int TotalClaims { get; set; }
        public int UsersWithExternalLogin { get; set; }
    }

    public IdentityDashboard ObtenerDashboard()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT COUNT(*) FROM AspNetUsers;
                SELECT COUNT(*) FROM AspNetUsers WHERE EmailConfirmed = 1;
                SELECT COUNT(*) FROM AspNetUsers
                    WHERE LockoutEnd IS NOT NULL AND LockoutEnd > GETUTCDATE();
                SELECT COUNT(*) FROM AspNetRoles;
                SELECT COUNT(*) FROM AspNetUserClaims;
                SELECT COUNT(DISTINCT UserId) FROM AspNetUserLogins;";

            using (var multi = db.QueryMultiple(sql))
            {
                return new IdentityDashboard
                {
                    TotalUsers             = multi.ReadSingle<int>(),
                    UsersEmailConfirmed    = multi.ReadSingle<int>(),
                    UsersLockedOut         = multi.ReadSingle<int>(),
                    TotalRoles             = multi.ReadSingle<int>(),
                    TotalClaims            = multi.ReadSingle<int>(),
                    UsersWithExternalLogin = multi.ReadSingle<int>()
                };
            }
        }
    }
}
