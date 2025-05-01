using System;
using System.Threading.Tasks;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;

namespace Library_System.Application.Interfaces.IRepositories;

public interface IBookRepository
{
     Task<Book?> GetByIdAsync(Guid id);
     
     Task<Book>GetByBookTitle(string bookName);
     Task<ServiceResult<PaginationResult<Book>>> GetAllAsync(BooksFilter filter);
     Task<ServiceResult<Book>> AddAsync(Book book);
     Task<ServiceResult<Book>> UpdateAsync(Book book);
     Task<ServiceResult<Book>> DeleteAsync(Guid id);

}