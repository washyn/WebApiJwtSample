using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso4_ExecuteScalar
{
    private readonly string _conn = Consts.connString;

    public int InsertRole(AspNetRole role)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            return db.Execute(sql, role);
        }
    }

    public int InsertUserClaimAndGetId(string userId, string claimType, string claimValue)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var claim = new AspNetUserClaim
            {
                UserId = userId,
                ClaimType = claimType,
                ClaimValue = claimValue
            };

            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return db.QuerySingle<int>(sql, claim);
        }
    }

    public int InsertMultipleUserClaims(List<AspNetUserClaim> claims)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue)";

            return db.Execute(sql, claims);
        }
    }

    public int UpdateUserSecurityStamp(string userId, string newSecurityStamp)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET SecurityStamp = @SecurityStamp WHERE Id = @Id";

            var rowsAffected = db.Execute(
                sql,
                new { Id = userId, SecurityStamp = newSecurityStamp }
            );

            return rowsAffected;
        }
    }

    public int IncrementarIntentosFallidos(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET AccessFailedCount = AccessFailedCount + 1 WHERE Id = @Id";
            return db.Execute(sql, new { Id = userId });
        }
    }

    public int ConfirmarEmail(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE Id = @Id AND EmailConfirmed = 0";
            return db.Execute(sql, new { Id = userId });
        }
    }

    public int UpsertUserToken(AspNetUserToken token)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                IF EXISTS (
                    SELECT 1 FROM AspNetUserTokens
                    WHERE UserId = @UserId
                      AND LoginProvider = @LoginProvider
                      AND Name = @Name
                )
                    UPDATE AspNetUserTokens SET Value = @Value
                    WHERE UserId = @UserId
                      AND LoginProvider = @LoginProvider
                      AND Name = @Name
                ELSE
                    INSERT INTO AspNetUserTokens (UserId, LoginProvider, Name, Value)
                    VALUES (@UserId, @LoginProvider, @Name, @Value)";

            return db.Execute(sql, token);
        }
    }

    public int AsignarRolAUsuario(string userId, string roleId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                IF NOT EXISTS (
                    SELECT 1 FROM AspNetUserRoles
                    WHERE UserId = @UserId AND RoleId = @RoleId
                )
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                VALUES (@UserId, @RoleId)";

            return db.Execute(sql, new { UserId = userId, RoleId = roleId });
        }
    }

    public int DeleteUserClaim(int claimId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "DELETE FROM AspNetUserClaims WHERE Id = @ClaimId";
            return db.Execute(sql, new { ClaimId = claimId });
        }
    }

    public int QuitarRolAUsuario(string userId, string roleId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            return db.Execute(sql, new { UserId = userId, RoleId = roleId });
        }
    }

    public int EliminarVariosClaims(List<int> claimIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "DELETE FROM AspNetUserClaims WHERE Id IN @ClaimIds";
            return db.Execute(sql, new { ClaimIds = claimIds });
        }
    }

    public int ContarUsuariosTotales()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM AspNetUsers");
        }
    }

    public int ContarRolesTotales()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM AspNetRoles");
        }
    }

    public int ContarUsuariosConEmailConfirmado()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT COUNT(*) FROM AspNetUsers WHERE EmailConfirmed = 1";
            return db.ExecuteScalar<int>(sql);
        }
    }

    public DateTimeOffset? ObtenerMaxLockoutEnd()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<DateTimeOffset?>(
                "SELECT MAX(LockoutEnd) FROM AspNetUsers"
            );
        }
    }

    public int ObtenerMaximoIntentosFallidos()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<int>(
                "SELECT ISNULL(MAX(AccessFailedCount), 0) FROM AspNetUsers"
            );
        }
    }

    public string ObtenerPrimerUserName()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<string>(
                "SELECT TOP 1 UserName FROM AspNetUsers ORDER BY UserName"
            );
        }
    }
}
