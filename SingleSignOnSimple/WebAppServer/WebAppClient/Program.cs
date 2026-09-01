using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace WebAppClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var serverBaseUrl = builder.Configuration["SSOServerBaseUrl"] ?? "https://localhost:7053";

        var sharedKeysPath = Path.Combine(
            Directory.GetParent(builder.Environment.ContentRootPath)!.FullName,
            "Shared-Keys");

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
            .SetApplicationName("SingleSignOnSharedApp");

        builder.Services.AddControllersWithViews();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = ".SingleSignOn.SharedCookie";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.Lax;

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        var redirectUri = $"{serverBaseUrl}/Account/Auth?returnUrl={Uri.EscapeDataString(context.RedirectUri)}";
                        context.Response.Redirect(redirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToLogout = context =>
                    {
                        var redirectUri = $"{serverBaseUrl}/Account/Logout?returnUrl={Uri.EscapeDataString(context.RedirectUri)}";
                        context.Response.Redirect(redirectUri);
                        return Task.CompletedTask;
                    },
                    OnValidatePrincipal = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
