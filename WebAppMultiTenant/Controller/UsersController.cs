using Microsoft.AspNetCore.Mvc;

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
    private readonly ITenantResolver _tenantResolver;

    public UsersRepository(ITenantStore tenantStore, ITenantResolver tenantResolver)
    {
        _tenantStore = tenantStore;
        _tenantResolver = tenantResolver;
    }

    public List<UserDto> GetUsers()
    {
        var tenant = _tenantResolver.ResolveTenantName();
        if (tenant == null)
        {
            throw new Exception("Tenant not found");
        }

        var tenantData = _tenantStore.GetTenant(tenant);
        if (tenantData == null)
        {
            throw new Exception("Tenant data not found");
        }

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

        return tenantsData[tenantData.Name];
    }
}