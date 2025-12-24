using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;

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
    public string Id { get; set; }
    public string Name { get; set; }
}

public class UsersRepository
{
    private readonly ITenantDbConnectionFactory _connectionFactory;

    public UsersRepository(ITenantDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public List<UserDto> GetUsers()
    {
        using var conn = _connectionFactory.Create();
        var users = conn.Query<UserDto>("SELECT Id as Id, Name as Name FROM Users");
        return users.ToList();
    }
}