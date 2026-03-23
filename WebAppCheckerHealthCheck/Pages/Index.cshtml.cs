using System.Collections;
using System.Globalization;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Volo.Abp.DependencyInjection;

namespace WebAppChecker.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppInfoProvider _appInfoProvider;

    public AppInfo Info { get; set; }

    public IndexModel(ILogger<IndexModel> logger,
        AppInfoProvider appInfoProvider)
    {
        _logger = logger;
        _appInfoProvider = appInfoProvider;
    }

    public IActionResult OnGet()
    {
        Info = _appInfoProvider.GetAppInfo();
        _logger.LogInformation("Visited index page...");
        var vars = Environment.GetEnvironmentVariables();
        _logger.LogInformation("All values: {@IDictionary}", vars);
        foreach (DictionaryEntry env in Environment.GetEnvironmentVariables())
        {
            _logger.LogInformation($"{env.Key} = {env.Value}");
        }

        MetricsDefinitions.RequestCounter.Add(1);
        // return Redirect("/health-ui");
        return Page();
    }
}

public class AppInfo
{
    public string Version { get; set; }
    public string BuildTime { get; set; }
    public string Environment { get; set; }
}

public class AppInfoProvider : ITransientDependency
{
    private readonly IWebHostEnvironment _environment;

    public AppInfoProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public AppInfo GetAppInfo()
    {
        var culture = CultureInfo.CurrentCulture;
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "N/A";
        var buildTime = GetBuildDate(Assembly.GetEntryAssembly());
        return new AppInfo
        {
            Version = version, BuildTime = buildTime.ToString(culture), Environment = _environment.EnvironmentName
        };
    }

    private static DateTime GetBuildDate(Assembly? assembly)
    {
        if (assembly == null) return DateTime.MinValue;
        var filePath = assembly.Location;
        return File.GetLastWriteTime(filePath); // Fecha de compilación aproximada
    }
}