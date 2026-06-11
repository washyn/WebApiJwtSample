using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.ObjectMapping;

namespace App.Api.ObjectMapping;

public class DemoObjectMapper : IObjectMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null) return default!;


        if (source is Book book && typeof(TDestination) == typeof(BookDto))
        {
            return (TDestination)(object)new BookDto
            {
                Id = book.Id, Title = book.Title, Author = book.Author, PublishDate = book.PublishDate
            };
        }


        throw new NotImplementedException(
            $"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported by DemoObjectMapper.");
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null || destination == null) return destination;


        throw new NotImplementedException(
            $"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported by DemoObjectMapper.");
    }
}
