namespace WebAppMultitenancyInfraestructure;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "dd/MM/yyyy HH:mm:ss ";
            options.SingleLine = true;
            options.IncludeScopes = false;
        });

        var configuration = builder.Configuration;
        builder.Services.Configure<AbpDefaultTenantStoreOptions>(configuration);
        builder.Services.Configure<AbpTenantResolveOptions>(options =>
        {
            options.TenantResolvers.Insert(0, new HttpHeaderTenantResolveContributor());
        });
        
        builder.Services.AddTransient<ITenantResolver, TenantResolver>();
        builder.Services.AddTransient<ITenantConfigurationProvider, TenantConfigurationProvider>();
        builder.Services.AddTransient<ITenantStore, AppSettingTenantStore>();
        builder.Services.AddTransient<ICurrentTenant, CurrentTenant>();
        
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        // builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}