using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Dapper;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
// using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Acme.BookStore
{
    [DependsOn(
        typeof(BookStoreDomainModule),
        // typeof(AbpEntityFrameworkCoreSqlServerModule)
        typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
    [DependsOn(typeof(AbpDapperModule))]
    public class BookStoreDapperModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<BookStoreDbContext>(options =>
            {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                /* The main point to change your DBMS.
                 * See also BookStoreMigrationsDbContextFactory for EF Core tooling. */
                options.UseSqlite();
            });
            
            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });
        }
    }
}
