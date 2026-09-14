using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

public class Paso1_OldVsNewWay
{
    public List<AspNetUser> GetUsers_OldWay()
    {
        var sql = "SELECT * FROM AspNetUsers";
        var products = new List<AspNetUser>();

        var connection = new SqlConnection(Consts.connString);
        connection.Open();
        using (var command = new SqlCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new AspNetUser
                    {
                        Id = reader.GetString(reader.GetOrdinal("Id")),
                        UserName = reader.GetString(reader.GetOrdinal("UserName")),
                        NormalizedUserName = reader.GetString(reader.GetOrdinal("NormalizedUserName")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        NormalizedEmail = reader.GetString(reader.GetOrdinal("NormalizedEmail")),
                        EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed")),
                        PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                        SecurityStamp = reader.GetString(reader.GetOrdinal("SecurityStamp")),
                        ConcurrencyStamp = reader.GetString(reader.GetOrdinal("ConcurrencyStamp")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        PhoneNumberConfirmed = reader.GetBoolean(reader.GetOrdinal("PhoneNumberConfirmed")),
                        TwoFactorEnabled = reader.GetBoolean(reader.GetOrdinal("TwoFactorEnabled")),
                        LockoutEnd = reader.IsDBNull(reader.GetOrdinal("LockoutEnd")) ? null : reader.GetDateTimeOffset(reader.GetOrdinal("LockoutEnd")),
                        LockoutEnabled = reader.GetBoolean(reader.GetOrdinal("LockoutEnabled")),
                        AccessFailedCount = reader.GetInt32(reader.GetOrdinal("AccessFailedCount")),
                    };

                    products.Add(user);
                }
            }
        }

        connection.Close();
        return products;
    }

    public List<AspNetUser> GetUsers_NewWay()
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.Query<AspNetUser>("SELECT * FROM AspNetUsers").ToList();
        }
    }
}
