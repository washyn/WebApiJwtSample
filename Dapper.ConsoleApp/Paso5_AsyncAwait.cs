using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso5_AsyncAwait
{
    private readonly string _conn = Consts.connString;

    public async Task<List<AspNetUser>> GetUsuariosAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var result = await db.QueryAsync<AspNetUser>("SELECT * FROM AspNetUsers");
            return result.ToList();
        }
    }

    public async Task<(int Users, int Roles, int Claims)> ContarTodoEnParaleloAsync()
    {
        using (IDbConnection db = new SqlConnection(_conn))
        {
            var t1 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetUsers");
            var t2 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetRoles");
            var t3 = db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM AspNetUserClaims");

            await Task.WhenAll(t1, t2, t3);

            return (t1.Result, t2.Result, t3.Result);
        }
    }
}
