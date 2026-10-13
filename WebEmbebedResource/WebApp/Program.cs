using Theme;

// using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplication<WebModule>();
        // builder.Services.AddControllersWithViews()
        //     .AddApplicationPart(typeof(ThemeModule).Assembly);

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseStaticFiles();
        // app.InitializeApplication();
        app.UseRouting();
        app.MapControllers();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
// [DependsOn(typeof(AbpAspNetCoreMvcModule))]
[DependsOn(typeof(ThemeModule))]
public class WebModule : AbpModule
{
}