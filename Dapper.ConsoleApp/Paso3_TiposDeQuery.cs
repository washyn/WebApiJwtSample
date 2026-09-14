using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

//                     | 0 resultados | 1 resultado  | +1 resultados
//   ------------------+---------------+--------------+---------------
//   QueryFirst        |   EXCEPCION   | PRIMERO      | PRIMERO
//   QueryFirstOrDefault| null/default | PRIMERO      | PRIMERO
//   QuerySingle       |   EXCEPCION   | UNICO        | EXCEPCION
//   QuerySingleOrDefault| null/default| UNICO        | EXCEPCION

public class Paso3_TiposDeQuery
{
    private readonly string _conn = Consts.connString;

    public AspNetUser QueryFirst_UsuarioConEmailConfirmado()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT TOP 1 * FROM AspNetUsers WHERE EmailConfirmed = 1 ORDER BY UserName";
            return db.QueryFirst<AspNetUser>(sql);
        }
    }

    public AspNetRole QueryFirst_RolCualquiera()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QueryFirst<AspNetRole>("SELECT TOP 1 * FROM AspNetRoles ORDER BY Name");
        }
    }

    public AspNetUser QueryFirstOrDefault_UsuarioPorUserName(string userName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var normalized = userName.ToUpperInvariant();
            var sql = "SELECT * FROM AspNetUsers WHERE NormalizedUserName = @NormalizedUserName";

            return db.QueryFirstOrDefault<AspNetUser>(
                sql,
                new { NormalizedUserName = normalized }
            );
        }
    }

    public AspNetUserClaim QueryFirstOrDefault_CualquierClaimDeUsuario(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QueryFirstOrDefault<AspNetUserClaim>(
                "SELECT * FROM AspNetUserClaims WHERE UserId = @UserId ORDER BY Id",
                new { UserId = userId }
            );
        }
    }

    public AspNetUser QuerySingle_UsuarioPorId(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return db.QuerySingle<AspNetUser>(sql, new { Id = userId });
        }
    }

    public AspNetRole QuerySingle_RolPorNormalizedName(string normalizedRoleName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QuerySingle<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE NormalizedName = @Name",
                new { Name = normalizedRoleName.ToUpperInvariant() }
            );
        }
    }

    public AspNetUser QuerySingleOrDefault_UsuarioPorId(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return db.QuerySingleOrDefault<AspNetUser>(sql, new { Id = userId });
        }
    }

    public AspNetRole QuerySingleOrDefault_RolPorNombre(string roleName)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.QuerySingleOrDefault<AspNetRole>(
                "SELECT * FROM AspNetRoles WHERE Name = @Name",
                new { Name = roleName }
            );
        }
    }

    public List<AspNetUser> Ejemplo_ReglasDeUso()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.Query<AspNetUser>("SELECT * FROM AspNetUsers").ToList();
        }
    }
}
