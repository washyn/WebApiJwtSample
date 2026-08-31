using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace WebAppClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var sharedKeysPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "SharedKeys");
        sharedKeysPath = Path.GetFullPath(sharedKeysPath);
        if (!Directory.Exists(sharedKeysPath))
        {
            Directory.CreateDirectory(sharedKeysPath);
        }

        builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(sharedKeysPath))
            .SetApplicationName("SingleSignOnSharedApp");

        builder.Services.AddAuthentication("Identity.Application")
            .AddCookie("Identity.Application", options =>
            {
                options.Cookie.Name = ".SingleSignOn.SharedCookie";
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.LoginPath = "/Account/SsoLogin";
                options.LogoutPath = "/Account/SsoLogout";
                options.ReturnUrlParameter = "returnUrl";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddRazorPages();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
