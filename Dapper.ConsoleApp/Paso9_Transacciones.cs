using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso9_Transacciones
{
    private readonly string _conn = Consts.connString;

    public int CrearRolConClaims(AspNetRole rol, IEnumerable<AspNetRoleClaim> claimsDelRol)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();

            using (var transaction = db.BeginTransaction())
            {
                try
                {
                    var insertRoleSql = @"
                        INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                        VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";

                    db.Execute(insertRoleSql, rol, transaction: transaction);

                    foreach (var claim in claimsDelRol)
                    {
                        claim.RoleId = rol.Id;

                        var insertClaimSql = @"
                            INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                            VALUES (@RoleId, @ClaimType, @ClaimValue)";

                        db.Execute(insertClaimSql, claim, transaction: transaction);
                    }

                    transaction.Commit();
                    return 1;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    public int ReemplazarRolesDeUsuario(string userId, List<string> nuevosRoleIds)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();

            using (var tran = db.BeginTransaction())
            {
                try
                {
                    var deleteSql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId";
                    db.Execute(deleteSql, new { UserId = userId }, transaction: tran);

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

    public void ReemplazarClaimsDeUsuario(string userId, List<AspNetUserClaim> nuevosClaims)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    db.Execute(
                        "DELETE FROM AspNetUserClaims WHERE UserId = @UserId",
                        new { UserId = userId },
                        transaction: tran
                    );

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

    public int TransferirRol(string sourceUserId, string targetUserId, string roleId)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();
            using (var tran = db.BeginTransaction())
            {
                try
                {
                    var del = db.Execute(
                        "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId",
                        new { UserId = sourceUserId, RoleId = roleId },
                        transaction: tran
                    );
                    if (del == 0)
                    {
                        throw new InvalidOperationException(
                            "El usuario origen no tiene asignado el rol especificado."
                        );
                    }

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

    public int CrearRolConClaims_Snapshot(
        AspNetRole rol, List<AspNetRoleClaim> claims)
    {
        using (var db = new SqlConnection(_conn))
        {
            db.Open();

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
