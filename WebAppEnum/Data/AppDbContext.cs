using Microsoft.EntityFrameworkCore;
using WebAppEnum.Ebooks;

namespace WebAppEnum.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ebook> Ebooks { get; set; }
}
