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

    public class RolConClaims
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public List<AspNetRoleClaim> Claims { get; set; } = new List<AspNetRoleClaim>();
    }

    public List<RolConClaims> ObtenerRolesConSusClaims()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT
                    r.Id   AS RoleId,
                    r.Name,
                    r.NormalizedName,
                    rc.Id,
                    rc.RoleId,
                    rc.ClaimType,
                    rc.ClaimValue
                FROM AspNetRoles r
                LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                ORDER BY r.Name, rc.ClaimType";

            var roleMap = new Dictionary<string, RolConClaims>();

            var result = db.Query<RolConClaims, AspNetRoleClaim, RolConClaims>(
                sql,
                (rol, claim) =>
                {
                    if (!roleMap.TryGetValue(rol.RoleId, out var currentRol))
                    {
                        currentRol = rol;
                        currentRol.Claims = new List<AspNetRoleClaim>();
                        roleMap.Add(currentRol.RoleId, currentRol);
                    }

                    if (claim != null && claim.Id != 0)
                        currentRol.Claims.Add(claim);

                    return currentRol;
                },
                splitOn: "Id"
            );

            return result.Distinct().ToList();
        }
    }

    public class UsuarioConClaims
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<AspNetUserClaim> Claims { get; set; } = new List<AspNetUserClaim>();
    }

    public List<UsuarioConClaims> ObtenerUsuariosConClaims()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT
                    u.Id   AS UserId,
                    u.UserName,
                    u.Email,
                    c.Id,
                    c.UserId,
                    c.ClaimType,
                    c.ClaimValue
                FROM AspNetUsers u
                LEFT JOIN AspNetUserClaims c ON u.Id = c.UserId
                ORDER BY u.UserName, c.ClaimType";

            var userMap = new Dictionary<string, UsuarioConClaims>();

            var result = db.Query<UsuarioConClaims, AspNetUserClaim, UsuarioConClaims>(
                sql,
                (user, claim) =>
                {
                    if (!userMap.TryGetValue(user.UserId, out var current))
                    {
                        current = user;
                        current.Claims = new List<AspNetUserClaim>();
                        userMap.Add(current.UserId, current);
                    }

                    if (claim != null && claim.Id != 0)
                        current.Claims.Add(claim);

                    return current;
                },
                splitOn: "Id"
            );

            return result.Distinct().ToList();
        }
    }

    public class UsuarioFull
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RolConClaimsLite> Roles { get; set; } = new List<RolConClaimsLite>();
    }

    public class RolConClaimsLite
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AspNetRoleClaim> RoleClaims { get; set; } = new List<AspNetRoleClaim>();
    }

    public List<UsuarioFull> ObtenerUsuarios3Niveles()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT
                    u.Id   AS UserId,
                    u.UserName,
                    r.Id   AS RoleId,
                    r.Name AS RoleName,
                    rc.Id,
                    rc.RoleId,
                    rc.ClaimType,
                    rc.ClaimValue
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetRoles      r  ON r.Id = ur.RoleId
                LEFT JOIN AspNetRoleClaims rc ON r.Id = rc.RoleId
                ORDER BY u.UserName, r.Name, rc.ClaimType";

            var usersMap = new Dictionary<string, UsuarioFull>();
            var rolesMap = new Dictionary<string, RolConClaimsLite>();

            db.Query<UsuarioFull, RolConClaimsLite, AspNetRoleClaim, UsuarioFull>(
                sql,
                (user, rol, claim) =>
                {
                    if (!usersMap.TryGetValue(user.UserId, out var currentUser))
                    {
                        currentUser = user;
                        currentUser.Roles = new List<RolConClaimsLite>();
                        usersMap.Add(currentUser.UserId, currentUser);
                    }

                    if (rol != null && rol.RoleId != null)
                    {
                        var roleKey = currentUser.UserId + "|" + rol.RoleId;
                        if (!rolesMap.TryGetValue(roleKey, out var currentRole))
                        {
                            currentRole = rol;
                            currentRole.RoleClaims = new List<AspNetRoleClaim>();
                            rolesMap.Add(roleKey, currentRole);
                            currentUser.Roles.Add(currentRole);
                        }

                        if (claim != null && claim.Id != 0)
                        {
                            currentRole.RoleClaims.Add(claim);
                        }
                    }

                    return currentUser;
                },
                splitOn: "RoleId,Id"
            ).AsList();

            return usersMap.Values.ToList();
        }
    }

    public class UsuarioClaimSimple
    {
        public AspNetUser User { get; set; }
        public AspNetUserClaim Claim { get; set; }
    }

    public List<UsuarioClaimSimple> ObtenerClaimsConDetalleUsuario()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                SELECT TOP 50
                    u.*,
                    c.*
                FROM AspNetUserClaims c
                INNER JOIN AspNetUsers u ON u.Id = c.UserId
                ORDER BY c.Id";

            return db.Query<AspNetUser, AspNetUserClaim, UsuarioClaimSimple>(
                sql,
                (user, claim) => new UsuarioClaimSimple
                {
                    User  = user,
                    Claim = claim
                },
                splitOn: "Id"
            ).ToList();
        }
    }
}
