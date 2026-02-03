namespace WebhookSystem.NET9;

public class Extensions
{
}

public static class CongfigurationExtensions
{
    public static string GetSqlite(this IConfiguration configuration)
    {
        return configuration.GetConnectionString("Default") ??
               throw new InvalidOperationException("Sqlite connection string not found");
    }
}