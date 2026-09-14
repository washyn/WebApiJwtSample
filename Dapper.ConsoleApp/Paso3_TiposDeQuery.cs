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

    public AspNetUser QuerySingle_UsuarioPorId(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "SELECT * FROM AspNetUsers WHERE Id = @Id";
            return db.QuerySingle<AspNetUser>(sql, new { Id = userId });
        }
    }
}
