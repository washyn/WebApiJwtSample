using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Acme.BookStore.Web.Data;

public class WebDbContext : AbpDbContext<WebDbContext>
{
    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        /* Configure your own entities here */
    }
}
