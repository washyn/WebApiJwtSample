using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 4: Execute + ExecuteScalar
// =============================================
// Execute        => para INSERT, UPDATE, DELETE (retorna filas afectadas)
// ExecuteScalar  => para 1 solo valor (COUNT, MAX, SUM, SCOPE_IDENTITY)
// QuerySingle<t> => para obtener 1 fila con 1 o varias columnas
// =============================================

public class Paso4_ExecuteScalar
{
    private readonly string _conn = Consts.connString;

    // =====================================
    // Execute: INSERT
    // =====================================

    public int InsertRole(AspNetRole role)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            // Execute retorna la CANTIDAD DE FILAS afectadas
            return db.Execute(sql, role);
        }
    }

    // Insertar un Claim de usuario y OBTENER EL NUEVO ID (IDENTITY)
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

            // AspNetUserClaims.Id es IDENTITY, así que usamos SCOPE_IDENTITY()
            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            // QuerySingle<int> retorna el valor único de la columna
            return db.QuerySingle<int>(sql, claim);
        }
    }

    // Batch: insertar varios claims en un solo Execute
    public int InsertMultipleUserClaims(List<AspNetUserClaim> claims)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue)";

            // Al pasar un IEnumerable, Dapper ejecuta el comando una vez
            // POR CADA elemento. Retorna TOTAL de filas insertadas.
            return db.Execute(sql, claims);
        }
    }

    // =====================================
    // Execute: UPDATE
    // =====================================

    public int UpdateUserSecurityStamp(string userId, string newSecurityStamp)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET SecurityStamp = @SecurityStamp WHERE Id = @Id";

            var rowsAffected = db.Execute(
                sql,
                new { Id = userId, SecurityStamp = newSecurityStamp }
            );

            return rowsAffected; // 1 si el usuario existe, 0 si no
        }
    }

    // UPDATE con operación matemática
    public int IncrementarIntentosFallidos(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET AccessFailedCount = AccessFailedCount + 1 WHERE Id = @Id";
            return db.Execute(sql, new { Id = userId });
        }
    }

    // Confirmar email (booleano)
    public int ConfirmarEmail(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE Id = @Id AND EmailConfirmed = 0";
            return db.Execute(sql, new { Id = userId });
        }
    }

    // Upsert de un Token (UPDATE si existe, INSERT si no)
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

    // Asignar Rol a Usuario (evitando duplicados)
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

    // =====================================
    // Execute: DELETE
    // =====================================

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

    // Delete múltiple con IN clause (Dapper expande la lista)
    public int EliminarVariosClaims(List<int> claimIds)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "DELETE FROM AspNetUserClaims WHERE Id IN @ClaimIds";
            return db.Execute(sql, new { ClaimIds = claimIds });
        }
    }

    // =====================================
    // ExecuteScalar: Valores agregados (1 valor)
    // =====================================

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

    // Para tipos nullable (LockoutEnd puede ser NULL)
    public DateTimeOffset? ObtenerMaxLockoutEnd()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<DateTimeOffset?>(
                "SELECT MAX(LockoutEnd) FROM AspNetUsers"
            );
        }
    }

    // ISNULL para un valor por defecto
    public int ObtenerMaximoIntentosFallidos()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<int>(
                "SELECT ISNULL(MAX(AccessFailedCount), 0) FROM AspNetUsers"
            );
        }
    }

    // Obtener el nombre de usuario más largo (primer alfabéticamente)
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
