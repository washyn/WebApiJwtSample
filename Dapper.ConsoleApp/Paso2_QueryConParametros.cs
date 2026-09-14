using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 2: Query con Parámetros (SQL Injection)
// =============================================
// ¿Por qué no concatenar strings?
// =============================================
// Si concatenas strings para armar tu query:
//
//   var sql = "SELECT * FROM AspNetUsers WHERE Id = '" + userId + "'";
//
// Y alguien envía como userId:
//
//   '; DROP TABLE AspNetUsers; --
//
// El resultado ejecuta:
//   SELECT * FROM AspNetUsers WHERE Id = ''; DROP TABLE AspNetUsers; --'
//
// => La tabla AspNetUsers se elimina.
//
// Dapper usa SQL Parameters (prefijo @). Los valores NUNCA se interpretan
// como SQL, incluso si contienen comillas o comandos maliciosos.
// =============================================

public class Paso2_QueryConParametros
{
    // =====================================
    // ✅ BUENO: Uso de parámetros
    // =====================================
    public List<AspNetUser> GetUsersByEmailConfirmed(bool emailConfirmed)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";

            // El objeto anónimo new { ... } define los valores de los parámetros
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

    // Parámetros con Roles
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

    // =====================================
    // ❌ MALO: Concatenación de strings
    // =====================================
    // Solo para fines de demostración - ¡NUNCA USES ESTO EN PRODUCCIÓN!
    public List<AspNetUser> GetUsersByEmail_Unsafe(string email)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            // 🚨 VULNERABLE A SQL INJECTION 🚨
            var sqlUnsafe = "SELECT * FROM AspNetUsers WHERE Email = '" + email + "'";
            return db.Query<AspNetUser>(sqlUnsafe).ToList();
        }
    }
}
