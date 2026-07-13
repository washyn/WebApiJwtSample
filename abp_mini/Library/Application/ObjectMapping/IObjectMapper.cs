namespace Library.Application.ObjectMapping;

public interface IObjectMapper
{
    TDestination Map<TSource, TDestination>(TSource source);
    
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
}

public interface IObjectMapper<TContext> : IObjectMapper
{
}
