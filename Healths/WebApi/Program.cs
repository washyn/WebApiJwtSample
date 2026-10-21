using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using WebApiLogCollector.Data;

namespace WebApiLogCollector;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=healths.db";

        builder.Services.AddControllers();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
        builder.Services
            .AddHealthChecks()
            .AddCheck<DbHc>("sqlite");

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapControllers();

        app.Run();
    }
}
