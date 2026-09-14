using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 9: Transacciones
// =============================================
// ¿Qué es una transacción?
// Un conjunto de operaciones SQL que deben ejecutarse de forma ATÓMICA:
//   - TODAS se aplican (COMMIT)
//   - O NINGUNA se aplica (ROLLBACK)
//
// Reglas con Dapper + Transaction:
//   1) Abre la conexión ANTES de crear la transacción
//      (connection.Open())
//   2) Pasa la transacción EN TODOS los Execute/Query
//      (db.Execute(sql, params, transaction))
//   3) Llama transaction.Commit() al final si todo OK
//   4) Si hay excepción, transaction.Rollback()
//   5) El bloque using asegura Rollback() automático si no se hizo Commit
//
// Dapper NO gestiona la transacción por ti; debes pasarla explícitamente.
// =============================================

public class Paso9_Transacciones
{
    private readonly string _conn = Consts.connString;

    // =====================================
    // Ejemplo 1: Crear Rol + N Claims (todo o nada)
    // =====================================
    // Se inserta el Rol. Se insertan N AspNetRoleClaims.
    // Si CUALQUIER claim falla => NO se crea nada.
    public int CrearRolConClaims(AspNetRole rol, IEnumerable<AspNetRoleClaim> claimsDelRol)
    {
        using (var db = new SqlConnection(_conn))
        {
            // 🔴 IMPORTANTE: Abrir conexión ANTES de BeginTransaction
            db.Open();

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    // 1) Insertar Rol
                    var insertRoleSql = @"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                    db.Execute(insertRoleSql, rol, transaction: transaction);

                    // 2) Insertar cada Claim del Rol
                    foreach (var claim in claimsDelRol)
                    {
                        claim.RoleId = rol.Id;

                        var insertClaimSql = @"
                            INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                            VALUES (@RoleId, @ClaimType, @ClaimValue)";

                        db.Execute(insertClaimSql, claim, transaction: transaction);
                    }

                    // 3) Si TODO fue OK: COMMIT
                    transaction.Commit();
                    return 1;
                }
                catch (Exception)
                {
                    // 4) Si ALGO falla: ROLLBACK
                    // (También se hace automáticamente al salir del using
                    //  sin haber llamado a Commit(), pero es explícito y bueno)
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    // =====================================
    // Ejemplo 2: Reemplazar Roles de un Usuario
    // =====================================
    // Paso A: Borrar TODOS los roles actuales del usuario
    // Paso B: Insertar los nuevos roles
    // Si B falla => A debe deshacerse (recuperamos los roles anteriores)
    public int ReemplazarRolesDeUsuario(string userId, List<string> nuevosRoleIds)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();

            using (var tran = db.BeginTransaction())
            {
                try
                {
                    // A) Eliminar roles actuales
                    var deleteSql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId";
                    db.Execute(deleteSql, new { UserId = userId }, transaction: tran);

                    // B) Insertar los nuevos (evitando duplicados)
                    var insertSql = @"
                        IF NOT EXISTS (
                            SELECT 1 FROM AspNetUserRoles
                            WHERE UserId = @UserId AND RoleId = @RoleId
                        )
                        INSERT INTO AspNetUserRoles (UserId, RoleId)
                        VALUES (@UserId, @RoleId)";

                    foreach (var roleId in nuevosRoleIds)
                    {
                        db.Execute(
                            insertSql,
                            new { UserId = userId, RoleId = roleId },
                            transaction: tran
                        );
                    }

                    // C) Bonus: reseteamos intentos fallidos y lockout
                    var updateSql = @"
                        UPDATE AspNetUsers
                        SET AccessFailedCount = 0,
                            LockoutEnd = NULL
                        WHERE Id = @UserId";
                    db.Execute(updateSql, new { UserId = userId }, transaction: tran);

                    tran.Commit();
                    return nuevosRoleIds.Count;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    // =====================================
    // Ejemplo 3: Reemplazar Claims de Usuario
    // =====================================
    public void ReemplazarClaimsDeUsuario(string userId, List<AspNetUserClaim> nuevosClaims)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    // 1) Eliminar todos los claims actuales del usuario
                    db.Execute(
                        "DELETE FROM AspNetUserClaims WHERE UserId = @UserId",
                        new { UserId = userId },
                        transaction: tran
                    );

                    // 2) Insertar los nuevos claims
                    if (nuevosClaims != null && nuevosClaims.Count > 0)
                    {
                        foreach (var c in nuevosClaims)
                            c.UserId = userId;

                        var insert = @"
                            INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
                            VALUES (@UserId, @ClaimType, @ClaimValue)";

                        db.Execute(insert, nuevosClaims, transaction: tran);
                    }

                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    // =====================================
    // Ejemplo 4: Transferencia de Rol entre usuarios
    // =====================================
    // Quitar un Rol al Usuario A y asignarlo al Usuario B.
    // Ambas operaciones deben ocurrir juntas.
    public int TransferirRol(string sourceUserId, string targetUserId, string roleId)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    // Quitar rol al source
                    var del = db.Execute(
                        "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                        new { UserId = sourceUserId, RoleId = roleId },
                        transaction: tran
                    );
                    if (del == 0)
                    {
                        // Source no tenía el rol => abortamos
                        throw new InvalidOperationException(
                            "El usuario origen no tiene asignado el rol especificado."
                        );
                    }

                    // Asignar rol al target
                    var ins = db.Execute(@"
                        IF NOT EXISTS (
                            SELECT 1 FROM AspNetUserRoles
                            WHERE UserId = @UserId AND RoleId = @RoleId
                        )
                        INSERT INTO AspNetUserRoles (UserId, RoleId)
                        VALUES (@UserId, @RoleId)",
                        new { UserId = targetUserId, RoleId = roleId },
                        transaction: tran
                    );

                    tran.Commit();
                    return del + ins;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    // =====================================
    // Ejemplo 5: Transacción + Nivel de Aislamiento
    // =====================================
    // Niveles de aislamiento comunes:
    //   - ReadCommitted  (default SQL Server)
    //   - ReadUncommitted (lectura sucia, sin bloqueos)
    //   - RepeatableRead
    //   - Serializable (más restrictivo, peor rendimiento)
    //   - Snapshot
    public int CrearRolConClaims_Snapshot(
        AspNetRole rol, List<AspNetRoleClaim> claims)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();

            // Especificamos IsolationLevel
            using (var tran = db.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    var sqlRole = @"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
                    db.Execute(sqlRole, rol, tran);

                    foreach (var c in claims)
                    {
                        c.RoleId = rol.Id;
                        db.Execute(@"
                            INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                            VALUES (@RoleId, @ClaimType, @ClaimValue)",
                            c, tran);
                    }

                    tran.Commit();
                    return 1;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    // =====================================
    // Ejemplo 6: Async / Transaction (versión asíncrona)
    // =====================================
    public async Task<int> ReemplazarRolesAsync(string userId, List<string> roleIds)
    {
        using (var db = new SqlConnection(_conn))
        {
            await db.OpenAsync();

            using (var tran = db.BeginTransaction())
            {
                try
                {
                    await db.ExecuteAsync(
                        "DELETE FROM AspNetUserRoles WHERE UserId = @UserId",
                        new { UserId = userId },
                        transaction: tran
                    );

                    foreach (var r in roleIds)
                    {
                        await db.ExecuteAsync(@"
                            IF NOT EXISTS (
                                SELECT 1 FROM AspNetUserRoles
                                WHERE UserId = @UserId AND RoleId = @RoleId
                            )
                            INSERT INTO AspNetUserRoles (UserId, RoleId)
                            VALUES (@UserId, @RoleId)",
                            new { UserId = userId, RoleId = r },
                            transaction: tran
                        );
                    }

                    tran.Commit();
                    return roleIds.Count;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}
