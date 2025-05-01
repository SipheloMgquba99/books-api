using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;

namespace Library_System.Application.Interfaces.IRepositories;

public interface IBookRequestRepository
{
    Task<BookRequest?> GetByIdAsync(Guid id);
    Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequestDetails(BookRequestFilter filter);
    Task<ServiceResult<BookRequest>> AddAsync(BookRequest bookRequest);
    Task<ServiceResult<BookRequest>> UpdateAsync(BookRequest bookRequest);
    Task<ServiceResult<BookRequest>> DeleteAsync(Guid id);
}