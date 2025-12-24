using System.Data;
using Dapper;

namespace WebAppMultiTenant;

public interface ITenantDatabaseInitializer
{
    void EnsureCreated(string tenantName, IDbConnection connection);
}

public class TenantDatabaseInitializer : ITenantDatabaseInitializer
{
    public void EnsureCreated(string tenantName, IDbConnection connection)
    {
        connection.Execute("CREATE TABLE IF NOT EXISTS Users (Id TEXT PRIMARY KEY, Name TEXT NOT NULL)");

        var count = connection.ExecuteScalar<int>("SELECT COUNT(1) FROM Users");
        if (count == 0)
        {
            var names = tenantName switch
            {
                "tenantA" => new[] { "User 1", "User 2", "User 3" },
                "tenantB" => new[] { "User 4", "User 5", "User 6" },
                _ => new[] { "User A", "User B" }
            };

            foreach (var name in names)
            {
                connection.Execute("INSERT INTO Users (Id, Name) VALUES (@Id, @Name)",
                    new { Id = Guid.NewGuid().ToString(), Name = name });
            }
        }
    }
}