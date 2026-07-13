using System.Globalization;
using System.Reflection;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Aspects;
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Linq;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.Validation;
using Volo.Abp.DependencyInjection;

namespace App.Api.Controllers;

[ApiController]
[Route("api/manifest")]
public class ManifestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ManifestProvider _manifestProvider;
    private readonly ILogger<ManifestController> _logger;

    public ManifestController(IConfiguration configuration, IWebHostEnvironment env,
        ManifestProvider manifestProvider,
        ILogger<ManifestController> logger)
    {
        _configuration = configuration;
        _env = env;
        _manifestProvider = manifestProvider;
        _logger = logger;
    }

    [HttpGet]
    public ManifestViewModel Get()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var esPeCulture = new CultureInfo("es-PE");
        var buildDate = System.IO.File.GetLastWriteTimeUtc(assembly.Location);
        var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var result = new ManifestViewModel()
        {
            Application = assembly.GetName().Name,
            Version = info?.InformationalVersion,
            Environment = _env.EnvironmentName,
            BuildDate = buildDate.ToString(esPeCulture), // this provides format and not time zone
        };
        return result;
    }

    [Route("v2")]
    [HttpGet]
    public ManifestViewModel GetManifest()
    {
        return _manifestProvider.GetManifest();
    }
}

public class ManifestViewModel
{
    public string Application { get; set; }
    public string Version { get; set; }
    public string Environment { get; set; }
    public string BuildDate { get; set; }
}

public class ManifestProvider : ITransientDependency
{
    protected IHostEnvironment _hostEnvironment => LazyServiceProvider.GetRequiredService<IHostEnvironment>();

    public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;

    // protected Type? ObjectMapperContext { get; set; } // this is an ref to module assembly

    // protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetService<IObjectMapper>(provider =>
    //     ObjectMapperContext == null
    //         ? provider.GetRequiredService<IObjectMapper>()
    //         : (IObjectMapper)provider.GetRequiredService(typeof(IObjectMapper<>).MakeGenericType(ObjectMapperContext)));

    public ManifestProvider()
    {
        // _hostEnvironment = hostEnvironment;
    }

    public ManifestViewModel GetManifest()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var esPeCulture = new CultureInfo("es-PE");
        var buildDate = System.IO.File.GetLastWriteTimeUtc(assembly.Location);
        var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        var result = new ManifestViewModel()
        {
            Application = assembly.GetName().Name,
            Version = info?.InformationalVersion,
            Environment = _hostEnvironment.EnvironmentName,
            BuildDate = buildDate.ToString(esPeCulture), // this provides format and not time zone
        };
        return result;
    }
}
