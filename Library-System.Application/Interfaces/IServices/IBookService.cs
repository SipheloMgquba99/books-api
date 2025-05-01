using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;

namespace Library_System.Application.Interfaces.IServices;

public interface IBookService
{
    Task<ServiceResult<Book>> AddBook(Book book);
    Task<ServiceResult<Book>> UpdateBook(Book book);
    Task<ServiceResult<Book>> DeleteBook(Guid id);
    Task<ServiceResult<Book>> GetBook(Guid id);
    Task<ServiceResult<PaginationResult<Book>>> GetBooks(BooksFilter filter);
}