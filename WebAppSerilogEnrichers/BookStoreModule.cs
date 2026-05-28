using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Washyn.BookStore.Data;
using Washyn.BookStore.Menus;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Uow;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AutoMapper;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;

namespace Washyn.BookStore
{
    [DependsOn(
        // ABP Framework packages
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule),

        // lepton-theme
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
    public class BookStoreModule : AbpModule
    {
        /* Single point to enable/disable multi-tenancy */
        public const bool IsMultiTenant = true;

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureMultiTenancy();
            ConfigureUrls(configuration);
            ConfigureBundles();
            ConfigureSwagger(context.Services);
            ConfigureAutoApiControllers();
            ConfigureVirtualFiles(hostingEnvironment);
            ConfigureLocalization();
            ConfigureNavigationServices();
            ConfigureEfCore(context);
            ConfigureAutoMapper(context);
        }

        private void ConfigureAutoMapper(ServiceConfigurationContext context)
        {
            context.Services.AddAutoMapperObjectMapper<BookStoreModule>();
            Configure<AbpAutoMapperOptions>(options =>
            {
                /* Uncomment `validate: true` if you want to enable the Configuration Validation feature.
                 * See AutoMapper's documentation to learn what it is:
                 * https://docs.automapper.org/en/stable/Configuration-validation.html
                 */
                options.AddMaps<BookStoreModule>( /* validate: true */);
            });
        }

        private void ConfigureMultiTenancy()
        {
            Configure<AbpMultiTenancyOptions>(options => { options.IsEnabled = IsMultiTenant; });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            });
        }

        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                // options.StyleBundles.Configure(
                //     BasicThemeBundles.Styles.Global,
                //     bundle => { bundle.AddFiles("/global-styles.css"); }
                // );

                // options.ScriptBundles.Configure(
                //     BasicThemeBundles.Scripts.Global,
                //     bundle => { bundle.AddFiles("/global-scripts.js"); }
                // );
            });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("ar", "ar", "Arabic"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "Chinese (Simplified)"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "Chinese (Traditional)"));
                options.Languages.Add(new LanguageInfo("cs", "cs", "Czech"));
                options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (United Kingdom)"));
                options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
                options.Languages.Add(new LanguageInfo("fr", "fr", "French"));
                options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "German (Germany)"));
                options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi "));
                options.Languages.Add(new LanguageInfo("hu", "hu", "Hungarian"));
                options.Languages.Add(new LanguageInfo("is", "is", "Icelandic"));
                options.Languages.Add(new LanguageInfo("it", "it", "Italian"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Portuguese (Brazil)"));
                options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Romanian (Romania)"));
                options.Languages.Add(new LanguageInfo("ru", "ru", "Russian"));
                options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
                options.Languages.Add(new LanguageInfo("es", "es", "Spanish"));
                options.Languages.Add(new LanguageInfo("sv", "sv", "Swedish"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Turkish"));
            });
        }

        private void ConfigureVirtualFiles(IWebHostEnvironment hostingEnvironment)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<BookStoreModule>();
                if (hostingEnvironment.IsDevelopment())
                {
                    /* Using physical files in development, so we don't need to recompile on changes */
                    options.FileSets.ReplaceEmbeddedByPhysical<BookStoreModule>(hostingEnvironment.ContentRootPath);
                }
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(BookStoreModule).Assembly);
            });
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        private void ConfigureNavigationServices()
        {
            Configure<AbpNavigationOptions>(options =>
            {
                options.MenuContributors.Add(new BookStoreMenuContributor());
            });
        }

        private void ConfigureEfCore(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<BookStoreDbContext>(options =>
            {
                /* You can remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots
                 * Documentation: https://docs.abp.io/en/abp/latest/Entity-Framework-Core#add-default-repositories
                 */
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(configurationContext => { configurationContext.UseSqlite(); });
            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();
            app.UseVirtualFiles();
            app.UseRouting();
            app.UseStaticFiles();
            // app.UseAbpSecurityHeaders();
            app.UseAuthentication();

            if (IsMultiTenant)
            {
                app.UseMultiTenancy();
            }

            app.UseUnitOfWork();
            // app.UseDynamicClaims();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseAbpSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStore API"); });

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseSerilogRequestLogging();
            app.UseConfiguredEndpoints();
        }
    }
}