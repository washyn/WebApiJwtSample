using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Washyn.BookStore.Entities.Books;

namespace Washyn.BookStore.Data
{
  public class BookStoreDbContext : AbpDbContext<BookStoreDbContext>
  {
    public DbSet<Book> Books { get; set; }

    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      /* Include modules to your migration db context */

      builder.Entity<Book>(b =>
      {
        b.ToTable(DbTablePrefix + "Books",
          DbSchema);
        b.ConfigureByConvention(); //auto configure for the base class props
        b.Property(x => x.Name).IsRequired().HasMaxLength(128);
      });

      /* Configure your own entities here */
    }
  }
}