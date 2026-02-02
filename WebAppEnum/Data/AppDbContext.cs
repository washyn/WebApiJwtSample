using Microsoft.EntityFrameworkCore;
using WebAppEnum.Ebooks;

namespace WebAppEnum.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ebook> Ebooks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ebook>()
            .Property(p => p.Type)
            .HasConversion<string>(); // 🔥 aquí pasa la magia

        // foreach (var entity in modelBuilder.Model.GetEntityTypes())
        // {
        //     foreach (var property in entity.GetProperties())
        //     {
        //         if (property.ClrType.IsEnum)
        //         {
        //             property.SetValueConverter(
        //                 new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToStringConverter<object>());
        //         }
        //     }
        // }
    }
}
