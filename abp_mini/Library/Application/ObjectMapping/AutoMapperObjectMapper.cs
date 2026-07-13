using AutoMapper;

namespace Library.Application.ObjectMapping;

public class AutoMapperObjectMapper : IObjectMapper
{
    private readonly IMapper _mapper;

    public AutoMapperObjectMapper(IMapper mapper)
    {
        _mapper = mapper;
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null)
        {
            return default!;
        }

        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null || destination == null)
        {
            return destination;
        }

        return _mapper.Map(source, destination);
    }
}

public class AutoMapperObjectMapper<TContext> : AutoMapperObjectMapper, IObjectMapper<TContext>
{
    public AutoMapperObjectMapper(IMapper mapper)
        : base(mapper)
    {
    }
}
