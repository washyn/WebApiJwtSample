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
}
