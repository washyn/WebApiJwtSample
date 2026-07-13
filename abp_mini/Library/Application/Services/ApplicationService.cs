using Library.Application.ObjectMapping;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.DependencyInjection;

namespace Library.Application.Services;

public abstract class ApplicationService : IApplicationService
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; } = default!;
    protected Type? ObjectMapperContext { get; set; }

    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetService<IObjectMapper>(provider =>
        ObjectMapperContext == null
            ? provider.GetRequiredService<IObjectMapper>()
            : (IObjectMapper)provider.GetRequiredService(typeof(IObjectMapper<>).MakeGenericType(ObjectMapperContext)));

    // Shoud be remove...
    protected virtual TDestination MapTo<TDestination>(object source)
    {
        return ObjectMapper.Map<object, TDestination>(source);
    }
}
