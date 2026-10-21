using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WebAppChecker;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        
        builder.Services.AddHealthChecks()
            .AddCheck<ExternalServerHealthCheck>(nameof(ExternalServerHealthCheck))
            .AddCheck<ExternalServerHealthCheckGoogle>(nameof(ExternalServerHealthCheckGoogle))
            ;

        builder.Services
            .AddHealthChecksUI()
            .AddInMemoryStorage()
            // .AddSqliteStorage($"Data Source=healthchecks.db")
            ;
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        
        app.UseRouting();

        app
            // Standard AspNet Healthcheck
            // .UseHealthChecks("/health", new HealthCheckOptions
            // {
            //     Predicate = _ => true
            // })
            .UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        app.UseHealthChecksUI(options => options.UIPath = "/health-ui");
        app.UseAuthorization();
        app.MapRazorPages();
        app.Run();
    }
}
