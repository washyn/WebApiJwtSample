using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Washyn.BookStore.Entities.Books;
using Washyn.BookStore.Services.Dtos.Books;

namespace Washyn.BookStore.Services.Books
{
    // [Authorize()]
    public class BookAppService :
        CrudAppService<Book, BookDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateBookDto>, IBookAppService
    {
        private readonly IRepository<Book, Guid> _repository;

        public BookAppService(IRepository<Book, Guid> repository) : base(repository)
        {
            _repository = repository;
        }
    }

    public interface IBookAppService :
        ICrudAppService< //Defines CRUD methods
            BookDto, //Used to show books
            Guid, //Primary key of the book entity
            PagedAndSortedResultRequestDto, //Used for paging/sorting
            CreateUpdateBookDto> //Used to create/update a book
    {
    }
}