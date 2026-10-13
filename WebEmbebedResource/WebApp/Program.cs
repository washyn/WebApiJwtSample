using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

// Expone los recursos embebidos del proyecto Theme en /Theming.
var themeFileProvider = new ManifestEmbeddedFileProvider(typeof(Theme.Theme).Assembly, "Theming");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = themeFileProvider,
    RequestPath = "/Theming"
});

app.UseRouting();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
