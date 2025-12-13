using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;

namespace ConsoleAbpSetting
{
    // [DependsOn(typeof(Volo.Abp.Auditing.AbpAuditingModule))]
    // [DependsOn(typeof(Volo.Abp.AutoMapper.AbpAutoMapperModule))]
    // [DependsOn(typeof(Volo.Abp.Application.AbpDddApplicationModule))]
    // [DependsOn(typeof(Volo.Abp.Domain.AbpDddDomainModule))]
    // [DependsOn(typeof(Volo.Abp.Json.Newtonsoft.AbpJsonNewtonsoftModule))]
    // [DependsOn(typeof(Volo.Abp.Timing.AbpTimingModule))]
    // [DependsOn(typeof(Volo.Abp.Data.AbpDataModule))]
    // [DependsOn(typeof(Volo.Abp.VirtualFileSystem.AbpVirtualFileSystemModule))]

    // [DependsOn(typeof(ExtraModule))]
    // [DependsOn(typeof(AbpAutofacModule))]
    // [DependsOn(typeof(AbpSecurityModule))]
    // [DependsOn(typeof(AbpThreadingModule))]
    // [DependsOn(typeof(AbpGuidsModule))]
    // [DependsOn(typeof(AbpMinifyModule))]
    // [DependsOn(typeof(AbpSpecificationsModule))]
    // [DependsOn(typeof(AbpSerializationModule))]
    // [DependsOn(typeof(AbpVirtualFileSystemModule))]
    
    [DependsOn(typeof(AbpSettingsModule))]
    public class ExtraModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MainService>();
        }

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpSettingOptions>(options =>
            {
                options.ValueProviders.Clear();
                options.ValueProviders.Add<DefaultValueSettingValueProvider>();
                options.ValueProviders.Add<ConfigurationSettingValueProvider>();
            });
        }
    }

    public class MainService : IHostedService
    {
        private readonly ISettingProvider _settingProvider;
        private readonly ILogger<MainService> _logger;
        private readonly IOptions<AbpSettingOptions> _options;
        private readonly ICollection<ISettingValueProvider> _providers;

        public MainService(ISettingProvider settingProvider, ILogger<MainService> logger, IOptions<AbpSettingOptions> options,
            ICollection<ISettingValueProvider> providers)
        {
            _settingProvider = settingProvider;
            _logger = logger;
            _options = options;
            _providers = providers;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = await _settingProvider.GetOrNullAsync(AppLogoSettings.AppLogoPicture);
            _logger.LogInformation($"AppLogoPicture value: {conf}");
            _logger.LogInformation($"Setting providers count: {_options.Value.ValueProviders.Count}");
            foreach (var valueProvider in _options.Value.ValueProviders)
            {
                _logger.LogInformation($"- Value provider: {valueProvider.FullName}");
            }
            _logger.LogInformation($"Definition providers count: {_options.Value.DefinitionProviders.Count}");
            foreach (var definitionProvider in _options.Value.DefinitionProviders)
            {
                _logger.LogInformation($"- Definition provider: {definitionProvider.FullName}");
            }
            // delted providers
            _logger.LogInformation($"Deleted providers count: {_options.Value.DeletedSettings.Count}");
            foreach (var provider in _options.Value.DeletedSettings)
            {
                _logger.LogInformation($"Deleted provider: {provider}");
            }
            
            // providers
            _logger.LogInformation($"Providers count: {_providers.Count}");
            foreach (var provider in _providers)
            {
                _logger.LogInformation($"- Provider: {provider.Name}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
    public class AppLogoSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            context.Add(new SettingDefinition(AppLogoSettings.AppLogoPicture, Guid.NewGuid().ToString()));
        }
    }

    public static class AppLogoSettings
    {
        private const string Prefix = "AppLogoSettings";

        //Add your own setting names here. Example:
        public const string AppLogoPicture = Prefix + ".AppLogoPicture";
    }
}