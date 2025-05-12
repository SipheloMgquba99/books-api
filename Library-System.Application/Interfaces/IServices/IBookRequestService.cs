using System;
using System.Threading.Tasks;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;


namespace Library_System.Application.Interfaces.IServices;

public interface IBookRequestService
{
    Task<ServiceResult<BookRequestDto>> AddBookRequest(BookRequestDto bookRequestDTO);
    Task<ServiceResult<BookRequestDto>> UpdateBookRequest(BookRequestDto bookRequestDto);
    Task<ServiceResult<BookRequestDto>> DeleteBookRequest(Guid id);
    Task<ServiceResult<BookRequestDto>> GetBookRequest(Guid id);
    Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequests(BookRequestFilter filter);
}