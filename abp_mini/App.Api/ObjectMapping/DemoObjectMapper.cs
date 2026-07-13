using App.Api.Dtos;
using App.Api.Entities;

using Library.Application.ObjectMapping;

namespace App.Api.ObjectMapping;

public class DemoObjectMapper : IObjectMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source == null) return default!;

        if (source is TodoItem entity && typeof(TDestination) == typeof(TodoItemDto))
        {
            return (TDestination)(object)new TodoItemDto
            {
                Id = entity.Id, Title = entity.Title, IsCompleted = entity.IsCompleted
            };
        }

        if (source is Category category && typeof(TDestination) == typeof(CategoryDto))
        {
            return (TDestination)(object)new CategoryDto { Id = category.Id, Name = category.Name };
        }

        if (source is Book book && typeof(TDestination) == typeof(BookDto))
        {
            return (TDestination)(object)new BookDto
            {
                Id = book.Id, Title = book.Title, Author = book.Author, PublishDate = book.PublishDate
            };
        }

        if (source is CreateUpdateTodoItemDto createDto && typeof(TDestination) == typeof(TodoItem))
        {
            return (TDestination)(object)new TodoItem { Title = createDto.Title, IsCompleted = createDto.IsCompleted };
        }

        if (source is CreateUpdateBookDto createBookDto && typeof(TDestination) == typeof(Book))
        {
            return (TDestination)(object)new Book
            {
                Title = createBookDto.Title, Author = createBookDto.Author, PublishDate = createBookDto.PublishDate
            };
        }

        throw new NotImplementedException(
            $"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported by DemoObjectMapper.");
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null || destination == null) return destination;

        if (source is CreateUpdateTodoItemDto updateDto && destination is TodoItem entity)
        {
            entity.Title = updateDto.Title;
            entity.IsCompleted = updateDto.IsCompleted;
            return destination;
        }

        if (source is CreateUpdateBookDto updateBookDto && destination is Book bookEntity)
        {
            bookEntity.Title = updateBookDto.Title;
            bookEntity.Author = updateBookDto.Author;
            bookEntity.PublishDate = updateBookDto.PublishDate;
            return destination;
        }

        throw new NotImplementedException(
            $"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported by DemoObjectMapper.");
    }
}