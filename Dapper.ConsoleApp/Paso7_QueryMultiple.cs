using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 7: QueryMultiple (Ahorro de viajes por red)
// =============================================
// Problema clásico (N+1 queries):
//   var users = SELECT * FROM AspNetUsers;           (1 viaje)
//   foreach (var u in users) {
//       u.Roles = SELECT * FROM AspNetUserRoles ... ; (N viajes, 1 por usuario)
//   }
//
//   => 1 + N viajes a la BD = MUY LENTO
//
// Solución QueryMultiple:
//   Ejecutas TODOS los SELECT en UNA MISMA conexión / UNA MISMA LLAMADA
//   y consumes secuencialmente los resultados con multi.Read<T>()
//
//   => 1 solo viaje a la BD = MUCHO MÁS RÁPIDO
// =============================================

public class Paso7_QueryMultiple
{
    private readonly string _conn = Consts.connString;

    // =====================================
    // Ejemplo 1: Obtener Usuarios + Roles + Counts (4 SELECT, 1 viaje)
    // =====================================
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
                // Leemos en EL MISMO ORDEN que los SELECT en SQL
                var usuarios = multi.Read<AspNetUser>().ToList();
                var roles    = multi.Read<AspNetRole>().ToList();
                var totalU   = multi.ReadSingle<int>();
                var totalR   = multi.ReadSingle<int>();

                return (usuarios, roles, totalU, totalR);
            }
        }
    }

    // =====================================
    // Ejemplo 2: Perfil COMPLETO de usuario
    //   - AspNetUsers (1 fila)
    //   - Roles asignados (N filas)
    //   - Claims (N filas)
    //   - Logins externos (N filas)
    //   - Tokens (N filas)
    // =====================================
    public (
        AspNetUser Usuario,
        List<AspNetRole> Roles,
        List<AspNetUserClaim> Claims,
        List<AspNetUserLogin> Logins,
        List<AspNetUserToken> Tokens
    ) ObtenerPerfilCompletoUsuario(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT * FROM AspNetUsers WHERE Id = @UserId;
                SELECT r.*
                FROM AspNetRoles r
                INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                WHERE ur.UserId = @UserId
                ORDER BY r.Name;
                SELECT * FROM AspNetUserClaims
                WHERE UserId = @UserId
                ORDER BY ClaimType, ClaimValue;
                SELECT * FROM AspNetUserLogins
                WHERE UserId = @UserId;
                SELECT * FROM AspNetUserTokens
                WHERE UserId = @UserId;";

            using (var multi = db.QueryMultiple(sql, new { UserId = userId }))
            {
                var usuario = multi.ReadSingleOrDefault<AspNetUser>();
                var roles   = multi.Read<AspNetRole>().ToList();
                var claims  = multi.Read<AspNetUserClaim>().ToList();
                var logins  = multi.Read<AspNetUserLogin>().ToList();
                var tokens  = multi.Read<AspNetUserToken>().ToList();

                return (usuario, roles, claims, logins, tokens);
            }
        }
    }

    // =====================================
    // Ejemplo 3: Dashboard / Panel de control resumido
    // =====================================
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

    // =====================================
    // Ejemplo 4: Rol con todos sus usuarios y claims
    // =====================================
    public (
        AspNetRole Role,
        List<AspNetUser> UsuariosEnRol,
        List<AspNetRoleClaim> ClaimsDelRol
    ) ObtenerRolConUsuariosYClaims(string roleId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT * FROM AspNetRoles WHERE Id = @RoleId;
                SELECT u.*
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId
                ORDER BY u.UserName;
                SELECT * FROM AspNetRoleClaims
                WHERE RoleId = @RoleId
                ORDER BY ClaimType, ClaimValue;";

            using (var multi = db.QueryMultiple(sql, new { RoleId = roleId }))
            {
                var rol      = multi.ReadSingleOrDefault<AspNetRole>();
                var users    = multi.Read<AspNetUser>().ToList();
                var claims   = multi.Read<AspNetRoleClaim>().ToList();

                return (rol, users, claims);
            }
        }
    }

    // =====================================
    // Ejemplo 5: Async - QueryMultipleAsync
    // =====================================
    public async Task<(
        List<AspNetUser> Usuarios,
        List<AspNetRole> Roles
    )> ObtenerUsuariosYRolesAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT * FROM AspNetUsers ORDER BY UserName;
                SELECT * FROM AspNetRoles ORDER BY Name;";

            using (var multi = await db.QueryMultipleAsync(sql))
            {
                var users = (await multi.ReadAsync<AspNetUser>()).ToList();
                var roles = (await multi.ReadAsync<AspNetRole>()).ToList();

                return (users, roles);
            }
        }
    }

    // =====================================
    // Ejemplo 6: QueryMultiple + JOIN materializado en memoria
    // =====================================
    public List<AspNetUserWithRole> ObtenerUsuariosRolesQueryMultiple()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT * FROM AspNetUsers ORDER BY UserName;
                SELECT
                    ur.UserId,
                    ur.RoleId,
                    r.Name AS RoleName
                FROM AspNetUserRoles ur
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id;";

            using (var multi = db.QueryMultiple(sql))
            {
                var users      = multi.Read<AspNetUser>().ToDictionary(u => u.Id, u => u);
                var userRoles  = multi.Read().ToList(); // dynamic

                var result = new List<AspNetUserWithRole>();

                foreach (var user in users.Values)
                {
                    var rolesForUser = userRoles.Where(ur => (string)ur.UserId == user.Id).ToList();

                    if (rolesForUser.Count == 0)
                    {
                        result.Add(new AspNetUserWithRole
                        {
                            UserId = user.Id,
                            UserName = user.UserName,
                            Email = user.Email,
                            EmailConfirmed = user.EmailConfirmed
                        });
                    }
                    else
                    {
                        foreach (var ur in rolesForUser)
                        {
                            result.Add(new AspNetUserWithRole
                            {
                                UserId = user.Id,
                                UserName = user.UserName,
                                Email = user.Email,
                                EmailConfirmed = user.EmailConfirmed,
                                RoleId = (string)ur.RoleId,
                                RoleName = (string)ur.RoleName
                            });
                        }
                    }
                }

                return result;
            }
        }
    }
}
