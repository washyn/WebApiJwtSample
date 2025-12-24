
using Microsoft.Extensions.Logging.Console;
using WebAppMultiTenant.Controller;

namespace WebAppMultiTenant;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "HH:mm:ss ";
            options.SingleLine = true;
            options.IncludeScopes = false;
        });


        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<UsersRepository>();

        builder.Services.AddTransient<ITenantResolver, HeaderTenantResolver>();
        builder.Services.AddTransient<ITenantStore, TenantStoreInMemory>();
        builder.Services.AddTransient<ICurrentTenant, CurrentTenant>();

        // SQLite + Dapper per tenant
        builder.Services.AddSingleton<ITenantDatabaseInitializer, TenantDatabaseInitializer>();
        builder.Services.AddTransient<ITenantDbConnectionFactory, TenantDbConnectionFactory>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseMiddleware<TenantMiddleware>();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
