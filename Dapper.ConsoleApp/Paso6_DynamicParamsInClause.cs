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
        bool? emailConfirmed = null,
        bool? lockoutEnabled = null,
        int? minAccessFailedCount = null)
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

            sql += " ORDER BY UserName";

            return db.Query<AspNetUser>(sql, parameters).ToList();
        }
    }

    public int InsertarLoginExterno(AspNetUserLogin login)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var parameters = new DynamicParameters();
            parameters.AddDynamicParams(login);

            parameters.Add(
                "@FueInsertado",
                dbType: DbType.Int32,
                direction: ParameterDirection.Output
            );

            var sql = @"
                IF NOT EXISTS (
                    SELECT 1 FROM AspNetUserLogins
                    WHERE LoginProvider = @LoginProvider
                      AND ProviderKey = @ProviderKey
                )
                BEGIN
                    INSERT INTO AspNetUserLogins
                        (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                    VALUES
                        (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId)
                    SET @FueInsertado = 1
                END
                ELSE
                BEGIN
                    SET @FueInsertado = 0
                END";

            db.Execute(sql, parameters);

            return parameters.Get<int>("@FueInsertado");
        }
    }

    public int Sp_ContarUsuariosPorRol(string roleId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", roleId);
            parameters.Add("@Total", dbType: DbType.Int32, direction: ParameterDirection.Output);

            db.Execute(
                "sp_ContarUsuariosPorRol",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@Total");
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

    public List<AspNetRole> ObtenerRolesPorIds(List<string> roleIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.Query<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE Id IN @RoleIds ORDER BY Name",
                new { RoleIds = roleIds }
            ).ToList();
        }
    }

    public List<AspNetUserClaim> ObtenerClaimsPorIds(List<int> claimIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.Query<AspNetUserClaim>(
                "SELECT * FROM AspNetUserClaims WHERE Id IN @ClaimIds ORDER BY Id",
                new { ClaimIds = claimIds }
            ).ToList();
        }
    }

    public int EliminarVariosClaims(List<int> claimIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.Execute(
                "DELETE FROM AspNetUserClaims WHERE Id IN @ClaimIds",
                new { ClaimIds = claimIds }
            );
        }
    }

    public int QuitarUsuariosDeUnRol(string roleId, List<string> userIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                DELETE FROM AspNetUserRoles
                WHERE RoleId = @RoleId
                  AND UserId IN @UserIds";

            return db.Execute(sql, new { RoleId = roleId, UserIds = userIds });
        }
    }

    public int ConfirmarEmailDeUsuarios(List<string> userIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                UPDATE AspNetUsers
                SET EmailConfirmed = 1
                WHERE Id IN @UserIds
                  AND EmailConfirmed = 0";

            return db.Execute(sql, new { UserIds = userIds });
        }
    }

    public List<AspNetUser> BusquedaCombinada(
        List<string> roleIds = null,
        bool? emailConfirmed = null,
        string emailDomain = null)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var parameters = new DynamicParameters();
            var sql = @"
                SELECT DISTINCT u.*
                FROM AspNetUsers u";

            if (roleIds != null && roleIds.Count > 0)
            {
                sql += " INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId ";
                sql += " WHERE ur.RoleId IN @RoleIds ";
                parameters.Add("RoleIds", roleIds);
            }
            else
            {
                sql += " WHERE 1=1 ";
            }

            if (emailConfirmed.HasValue)
            {
                sql += " AND u.EmailConfirmed = @EmailConfirmed";
                parameters.Add("EmailConfirmed", emailConfirmed.Value);
            }

            if (!string.IsNullOrWhiteSpace(emailDomain))
            {
                sql += " AND u.Email LIKE @Domain";
                parameters.Add("Domain", "%" + emailDomain + "%");
            }

            sql += " ORDER BY u.UserName";

            return db.Query<AspNetUser>(sql, parameters).ToList();
        }
    }
}
