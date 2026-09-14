using System.Collections.Generic;
using System.Data;
using System.Linq;

using Dapper;

using Microsoft.Data.SqlClient;

namespace Dapper.ConsoleApp;

// =============================================
// PASO 1: Old Way vs New Way (Reducción ~80% de código)
// =============================================
// Este paso muestra la diferencia entre hacer acceso a datos
// de forma manual (ADO.NET puro) contra usar Dapper.
//
// Para AspNetUsers (12 columnas):
//   Old Way: ~45 líneas (mientras más columnas, más código)
//   New Way: ~3  líneas
// =============================================

public class Paso1_OldVsNewWay
{
    // =====================================
    // FORMA ANTIGUA (ADO.NET puro)
    // =====================================
    // Problemas:
    // - Mucho código boilerplate
    // - Mapeo manual propiedad por propiedad
    // - Fácil olvidar manejar DBNull
    // - Usar índices numéricos (GetOrdinal) es frágil ante cambios
    // - Fácil cometer errores de tipo
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

    // =====================================
    // FORMA NUEVA (Dapper)
    // =====================================
    // Ventajas:
    // - Dapper se encarga del mapeo columna -> propiedad
    // - Maneja DBNull -> null automáticamente
    // - Compara nombres de columna case-insensitive
    // - Abre/cierra conexión automáticamente
    public List<AspNetUser> GetUsers_NewWay()
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.Query<AspNetUser>("SELECT * FROM AspNetUsers").ToList();
        }
    }

    // Lo mismo para un solo registro:
    public AspNetUser GetUserById_OldWay(string userId)
    {
        AspNetUser user = null;
        using (var connection = new SqlConnection(Consts.connString))
        {
            connection.Open();
            using (var command = new SqlCommand("SELECT * FROM AspNetUsers WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", userId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new AspNetUser
                        {
                            Id = reader.GetString(reader.GetOrdinal("Id")),
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                            EmailConfirmed = reader.GetBoolean(reader.GetOrdinal("EmailConfirmed"))
                        };
                    }
                }
            }
        }
        return user;
    }

    public AspNetUser GetUserById_NewWay(string userId)
    {
        using (IDbConnection db = new SqlConnection(Consts.connString))
        {
            return db.QueryFirstOrDefault<AspNetUser>(
                "SELECT * FROM AspNetUsers WHERE Id = @Id",
                new { Id = userId }
            );
        }
    }
}
