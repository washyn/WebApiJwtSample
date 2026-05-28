using AutoMapper;
using Washyn.BookStore.Entities.Books;
using Washyn.BookStore.Services.Dtos.Books;

namespace Washyn.BookStore.ObjectMapping
{
    public class BookStoreWebAutomaper : Profile
    {
        public BookStoreWebAutomaper()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<CreateUpdateBookDto, Book>().ReverseMap();
            CreateMap<BookDto, CreateUpdateBookDto>().ReverseMap();
        }
    }
}