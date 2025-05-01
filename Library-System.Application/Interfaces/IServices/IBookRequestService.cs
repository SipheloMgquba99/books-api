using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Library_System.Domain.Entities;


namespace Library_System.Application.Interfaces.IServices;

public interface IBookRequestService
{
    Task<ServiceResult<BookRequest>> AddBookRequest(BookRequestDTO bookRequestDTO
        );
    Task<ServiceResult<BookRequest>> UpdateBookRequest(BookRequestDTO bookRequestDto);
    Task<ServiceResult<BookRequest>> DeleteBookRequest(Guid id);
    Task<ServiceResult<BookRequest>> GetBookRequest(Guid id);
    Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequests(BookRequestFilter filter);
}