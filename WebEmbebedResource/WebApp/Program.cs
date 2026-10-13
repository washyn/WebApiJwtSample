// using Theme;
//
// using Volo.Abp.Modularity;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Services.AddApplication<WebModule>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

// [DependsOn(typeof(ThemeModule))]
// public class WebModule : AbpModule
// {
// }