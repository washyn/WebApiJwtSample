# Independent ABP-Style Repository Pattern

I have successfully implemented an independent version of the ABP-style repository pattern within `MyLibrary`. 

## Architecture & Features

The design mimics the core abstractions of ABP but relies purely on Microsoft's Entity Framework Core, omitting tightly coupled ABP concerns like `IUnitOfWork` or multi-tenancy.

### 1. Domain Entities
Located in `Domain/Entities/`. These define the shape of an entity:
- `IEntity` & `IEntity<TKey>`: Marker interfaces.
- `Entity` & `Entity<TKey>`: Base classes that provide common implementations like equality checks based on `Id`.

### 2. Repository Interfaces
Located in `Domain/Repositories/`. They enforce separation of concerns between basic/advanced and read/write operations:
- `IReadOnlyBasicRepository<TEntity>`: Simple reads (`GetList`, `GetCount`).
- `IReadOnlyRepository<TEntity>`: Advanced reads (pagination, sorting, `IQueryable`).
- `IBasicRepository<TEntity>`: Basic CRUD modifications (`Insert`, `Update`, `Delete`).
- `IRepository<TEntity>`: The all-encompassing interface used in your application code.

### 3. EF Core Implementation
Located in `EntityFrameworkCore/EfCoreRepository.cs`. This provides the concrete implementation:
- It uses `System.Linq.Dynamic.Core` to seamlessly parse string-based property sorts for pagination.
- It includes both **synchronous** and **asynchronous** variants for every method.
- It accepts a `DbContext` via constructor injection so you can wire it up with standard .NET dependency injection.

## How to Use It

1. **Define an Entity:**
```csharp
public class User : Entity<Guid>
{
    public string Name { get; set; }
}
```

2. **Register it (Optional, but recommended for DI):**
If you inject the raw repository, you can configure it in your `Startup.cs` or `Program.cs`:
```csharp
builder.Services.AddTransient(typeof(IRepository<,>), typeof(EfCoreRepository<MyDbContext, ,>));
```

3. **Inject and Execute:**
```csharp
public class UserService 
{
    private readonly IRepository<User, Guid> _userRepository;

    public UserService(IRepository<User, Guid> userRepository) 
    {
        _userRepository = userRepository;
    }

    public async Task CreateUserAsync(string name) 
    {
        await _userRepository.InsertAsync(new User { Name = name }, autoSave: true);
    }
}
```

> [!TIP]
> The project builds successfully with 0 errors. A NuGet vulnerability warning is visible for `System.Linq.Dynamic.Core`, but I have installed the latest available patch (`1.3.14`). If your security policies forbid this, you can remove the package and update the `GetPagedList` methods to parse string sorting manually.



# Independent ABP-Style Repository Pattern

I have successfully implemented an independent version of the ABP-style repository pattern within `MyLibrary`. 

## Architecture & Features

The design mimics the core abstractions of ABP but relies purely on Microsoft's Entity Framework Core, omitting tightly coupled ABP concerns like `IUnitOfWork` or multi-tenancy.

### 1. Domain Entities
Located in `Domain/Entities/`. These define the shape of an entity:
- `IEntity` & `IEntity<TKey>`: Marker interfaces.
- `Entity` & `Entity<TKey>`: Base classes that provide common implementations like equality checks based on `Id`.

### 2. Repository Interfaces
Located in `Domain/Repositories/`. They enforce separation of concerns between basic/advanced and read/write operations:
- `IReadOnlyBasicRepository<TEntity>`: Simple reads (`GetList`, `GetCount`).
- `IReadOnlyRepository<TEntity>`: Advanced reads (pagination, sorting, `IQueryable`).
- `IBasicRepository<TEntity>`: Basic CRUD modifications (`Insert`, `Update`, `Delete`).
- `IRepository<TEntity>`: The all-encompassing interface used in your application code.

### 3. EF Core Implementation
Located in `EntityFrameworkCore/EfCoreRepository.cs`. This provides the concrete implementation:
- It uses `System.Linq.Dynamic.Core` to seamlessly parse string-based property sorts for pagination.
- It includes both **synchronous** and **asynchronous** variants for every method.
- It accepts a `DbContext` via constructor injection so you can wire it up with standard .NET dependency injection.

### 4. Application DTOs
Located in `Application/Dtos/`.
- `IEntityDto` / `EntityDto<TKey>`: Base DTOs that mirror entities.
- `PagedResultDto<T>`: A standard envelope for returning paginated lists with a `TotalCount` and `Items`.
- `PagedAndSortedResultRequestDto`: A standard input parameter for `GetListAsync` operations, holding `SkipCount`, `MaxResultCount`, and `Sorting`.

### 5. Application Services
Located in `Application/Services/`. These services orchestrate the domain repositories:
- `IReadOnlyAppService` / `ReadOnlyAppService`: Handles retrieving data and mapping it to DTOs.
- `ICrudAppService` / `CrudAppService`: Extends the read-only service to include Create, Update, and Delete operations.

### 6. Object Mapping Abstraction
Located in `Application/ObjectMapping/`.
- `IObjectMapper`: A lightweight interface to keep the library decoupled from specific mapping tools like AutoMapper. You will need to implement this interface in your main application and register it in your DI container.

## How to Use It

1. **Define an Entity and DTOs:**
```csharp
public class User : Entity<Guid> { public string Name { get; set; } }
public class UserDto : EntityDto<Guid> { public string Name { get; set; } }
public class CreateUserDto { public string Name { get; set; } }
```

2. **Create an Application Service:**
```csharp
public class UserAppService : CrudAppService<User, UserDto, Guid, PagedAndSortedResultRequestDto, CreateUserDto, CreateUserDto>
{
    public UserAppService(IRepository<User, Guid> repository) : base(repository)
    {
    }
}
```

3. **Register Services and ObjectMapper (Startup.cs/Program.cs):**
```csharp
// Register Repositories
builder.Services.AddTransient(typeof(IRepository<,>), typeof(EfCoreRepository<MyDbContext, ,>));
builder.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(EfCoreRepository<MyDbContext, ,>));

// Register your ObjectMapper implementation
builder.Services.AddSingleton<IObjectMapper, MyAutoMapperWrapper>();

// Register Application Services
builder.Services.AddTransient<UserAppService>();
```

> [!TIP]
> The project builds successfully with 0 errors. A NuGet vulnerability warning is visible for `System.Linq.Dynamic.Core`, but I have installed the latest available patch (`1.3.14`). If your security policies forbid this, you can remove the package and update the `GetPagedList` methods to parse string sorting manually.
