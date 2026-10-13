using Microsoft.Extensions.FileProviders;

using Theme;

using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication<WebModule>();
builder.Services.AddControllersWithViews();

var app = builder.Build();
// var virtualFileProvider = app.Services.GetRequiredService<IVirtualFileProvider>();

app.UseStaticFiles();

app.InitializeApplication();

// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new CompositeFileProvider(
//         app.Environment.WebRootFileProvider,
//         virtualFileProvider)
// });

app.UseRouting();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


[DependsOn(typeof(AbpAspNetCoreMvcModule), typeof(ThemeModule))]
public class WebModule : AbpModule
{
}
// TODO: fix for publish wwwroot files y ebembed are equal