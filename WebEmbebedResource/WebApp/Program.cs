using Theme;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication<WebModule>();
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(ThemeModule).Assembly);

var app = builder.Build();

app.UseStaticFiles();
// app.InitializeApplication();
app.UseRouting();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// [DependsOn(typeof(AbpAspNetCoreMvcModule))]
[DependsOn(typeof(ThemeModule))]
public class WebModule : AbpModule
{
}
