using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 5: Async / Await
// =============================================
// Dapper expone versiones asíncronas de TODOS sus métodos:
//   QueryAsync            => Query
//   QueryFirstAsync       => QueryFirst
//   QueryFirstOrDefaultAsync  => QueryFirstOrDefault
//   QuerySingleAsync      => QuerySingle
//   QuerySingleOrDefaultAsync => QuerySingleOrDefault
//   QueryMultipleAsync    => QueryMultiple
//   ExecuteAsync          => Execute
//   ExecuteScalarAsync    => ExecuteScalar
//   ExecuteReaderAsync    => ExecuteReader
//
// ¿Por qué Async?
// - En apps web: no bloquea el hilo mientras espera la BD
// - Mejor escalabilidad (más requests concurrentes)
// - UI apps: no congela la interfaz
//
// Regla: usar await con cada método *Async.
// =============================================

public class Paso5_AsyncAwait
{
    private readonly string _conn = Consts.connString;

    // =====================================
    // QueryAsync: Lista de usuarios
    // =====================================
    public async Task<List<AspNetUser>> GetUsuariosAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            // await libera el hilo mientras espera a la BD
            var result = await db.QueryAsync<AspNetUser>("SELECT * FROM AspNetUsers");
            return result.ToList();
        }
    }

    public async Task<List<AspNetRole>> GetRolesAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetRoles ORDER BY Name";
            var roles = await db.QueryAsync<AspNetRole>(sql);
            return roles.ToList();
        }
    }

    // =====================================
    // QueryFirstOrDefaultAsync
    // =====================================
    public async Task<AspNetUser> GetUsuarioPorIdAsync(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.QueryFirstOrDefaultAsync<AspNetUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }
    }

    public async Task<AspNetRole> GetRolPorNombreAsync(string roleName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var normalized = roleName.ToUpperInvariant();
            return await db.QueryFirstOrDefaultAsync<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @Name",
                new { Name = normalized }
            );
        }
    }

    // =====================================
    // QuerySingleOrDefaultAsync
    // =====================================
    public async Task<AspNetUserClaim> GetClaimPorIdAsync(int claimId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.QuerySingleOrDefaultAsync<AspNetUserClaim>(
                "SELECT * FROM AspNetUserClaims WHERE Id = @ClaimId",
                new { ClaimId = claimId }
            );
        }
    }

    // =====================================
    // ExecuteAsync (INSERT / UPDATE / DELETE)
    // =====================================

    public async Task<int> InsertarRolAsync(AspNetRole role)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

            return await db.ExecuteAsync(sql, role);
        }
    }

    public async Task<int> ActualizarSecurityStampAsync(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                UPDATE AspNetUsers
                SET SecurityStamp = @NewStamp
                WHERE Id = @Id";

            return await db.ExecuteAsync(
                sql,
                new { Id = userId, NewStamp = Guid.NewGuid().ToString() }
            );
        }
    }

    public async Task<int> ConfirmarEmailAsync(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE Id = @Id";
            return await db.ExecuteAsync(sql, new { Id = userId });
        }
    }

    public async Task<int> EliminarClaimAsync(int claimId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.ExecuteAsync(
                "DELETE FROM AspNetUserClaims WHERE Id = @ClaimId",
                new { ClaimId = claimId }
            );
        }
    }

    // Batch async (múltiples claims)
    public async Task<int> InsertarVariosClaimsAsync(List<AspNetUserClaim> claims)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue)";

            return await db.ExecuteAsync(sql, claims);
        }
    }

    // =====================================
    // ExecuteScalarAsync
    // =====================================

    public async Task<int> ContarUsuariosAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetUsers");
        }
    }

    public async Task<int> ContarRolesAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetRoles");
        }
    }

    public async Task<DateTimeOffset?> ObtenerMaxLockoutEndAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return await db.ExecuteScalarAsync<DateTimeOffset?>(
                "SELECT MAX(LockoutEnd) FROM AspNetUsers"
            );
        }
    }

    // =====================================
    // ExecuteScalarAsync con INSERT + SCOPE_IDENTITY
    // =====================================
    public async Task<int> InsertarClaimYRetornarIdAsync(AspNetUserClaim claim)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = @"
                INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                VALUES (@UserId, @ClaimType, @ClaimValue);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await db.QuerySingleAsync<int>(sql, claim);
        }
    }

    // =====================================
    // Ejemplo: Llamadas múltiples en paralelo (WhenAll)
    // =====================================
    public async Task<(int Users, int Roles, int Claims)> ContarTodoEnParaleloAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            // Lanzamos todas las tareas a la vez
            var t1 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetUsers");
            var t2 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetRoles");
            var t3 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetUserClaims");

            // Esperamos que TODAS terminen
            await Task.WhenAll(t1, t2, t3);

            return (t1.Result, t2.Result, t3.Result);
        }
    }
}
