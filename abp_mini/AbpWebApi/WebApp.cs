using App.Api.Data;

using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Modularity;

[DependsOn(typeof(AbpAspNetCoreMvcModule))]
public class WebApp : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddDbContext<AppDbContext>(a =>
        {
            a.UseInMemoryDatabase("AppDbContext");
        });
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddSwaggerGen();
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(WebApp).Assembly);
        });
        context.Services.AddTransient(typeof(IRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IBasicRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(AppDbContextRepository<,>));

        Configure<AbpAntiForgeryOptions>(options => { options.AutoValidate = false; });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseConfiguredEndpoints();
    }
}