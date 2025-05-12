using System;
using System.Threading.Tasks;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Library_System.Domain.Entities;

namespace Library_System.Application.Interfaces.IServices;

public interface IBookService
{
    Task<ServiceResult<BookDto>> AddBook(BookDto bookDto);
    Task<ServiceResult<BookDto>> UpdateBook(BookDto bookDto);
    Task<ServiceResult<BookDto>> DeleteBook(Guid id);
    Task<ServiceResult<BookDto>> GetBook(Guid id);
    Task<ServiceResult<PaginationResult<BookDto>>> GetBooks(BooksFilter filter);
}