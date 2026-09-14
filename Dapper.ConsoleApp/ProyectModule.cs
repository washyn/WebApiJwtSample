using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Dapper.ConsoleApp;

[DependsOn(
    typeof(AbpAutofacModule)
)]
public class ProjectModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        // Register IDbConnection as Transient
        context.Services.AddTransient<IDbConnection>((sp) =>
        {
            return new SqlConnection(configuration.GetConnectionString(
                "DefaultConnection"));
        });
    }
}
// NOTE: can be simplifi console app