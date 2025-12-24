using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebAppMultiTenant.Controller;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UsersRepository _usersStore;

    public UsersController(UsersRepository usersStore)
    {
        _usersStore = usersStore;
    }

    [HttpGet]
    public List<UserDto> GetList()
    {
        return _usersStore.GetUsers();
    }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UsersRepository
{
    private readonly ITenantStore _tenantStore;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UsersRepository> _logger;

    public UsersRepository(ITenantStore tenantStore, IHttpContextAccessor httpContextAccessor,
        ILogger<UsersRepository> logger)
    {
        _tenantStore = tenantStore;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public List<UserDto> GetUsers()
    {
        var tenantInfo = _httpContextAccessor.HttpContext?.Items[TenantMiddleware.TenantItemsKey] as TenantInfo;
        if (tenantInfo is null)
        {
            throw new Exception("Tenant no disponible en el contexto");
        }

        _logger.LogInformation("Obteniendo usuarios para {Tenant}", tenantInfo.Name);

        var dataTenantA = new List<UserDto>()
        {
            new UserDto()
            {
                Id = Guid.NewGuid(), Name = "User 1"
            },
            new UserDto()
            {
                Id = Guid.NewGuid(), Name = "User 2"
            },
            new UserDto()
            {
                Id = Guid.NewGuid(),
                Name = "User 3"
            }
        };

        var dataTenantB = new List<UserDto>()
        {
            new UserDto()
            {
                Id = Guid.NewGuid(), Name = "User 4"
            },
            new UserDto()
            {
                Id = Guid.NewGuid(), Name = "User 5"
            },
            new UserDto()
            {
                Id = Guid.NewGuid(),
                Name = "User 6"
            }
        };

        var tenantsData = new Dictionary<string, List<UserDto>>()
        {
            { "tenantA", dataTenantA },
            { "tenantB", dataTenantB }
        };

        return tenantsData[tenantInfo.Name];
    }
}