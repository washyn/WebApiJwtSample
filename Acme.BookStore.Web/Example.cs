using System.ComponentModel.DataAnnotations;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Acme.BookStore.Web;

public class CatalogAppService : ApplicationService
{
    private readonly ExampleDbContext _dbContext;

    public CatalogAppService(ExampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CatalogDto>> GetListAsync()
    {
        var catalogs = await _dbContext.Catalogs.ToListAsync();
        return ObjectMapper.Map<List<Catalog>, List<CatalogDto>>(catalogs);
    }
}

public class ExampleDbContext : DbContext
{
    public DbSet<Catalog> Catalogs { get; set; }
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options) { }
}

public class Catalog
{
    [Key] public int Id { get; set; }
    public string Name { get; set; }
}

public class CatalogDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Catalog, CatalogDto>().ReverseMap();
    }
}

public class CustomAppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddDbContext<ExampleDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")));
    }
}

// public class AppRepository : EfCoreRepository<ExampleDbContext, Catalog>
// {
// }
