using MyLibrary.Application.ObjectMapping;

namespace MyLibrary.Application.Services;

public abstract class ApplicationService : IApplicationService
{
    public IObjectMapper ObjectMapper { get; set; } = default!;

    protected virtual TDestination MapTo<TDestination>(object source)
    {
        return ObjectMapper.Map<object, TDestination>(source);
    }
}
