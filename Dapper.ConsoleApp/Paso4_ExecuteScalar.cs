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

    public int ConfirmarEmail(string userId)
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var sql = "UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE Id = @Id AND EmailConfirmed = 0";
            return db.Execute(sql, new { Id = userId });
        }
    }

    public int ContarUsuariosTotales()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            return db.ExecuteScalar<int>("SELECT COUNT(*) FROM AspNetUsers");
        }
    }
}
