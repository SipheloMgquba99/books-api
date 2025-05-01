using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Library_System.Infrastructure.Cache.Interfaces;

public class BookRequestService : IBookRequestService
{
    private readonly IBookRequestRepository _bookRequestRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;

    private const string BookRequestCacheKeyPrefix = "bookrequest:";

    public BookRequestService(
        IBookRequestRepository bookRequestRepository,
        IBookRepository bookRepository,
        ICacheService cacheService)
    {
        _bookRequestRepository = bookRequestRepository;
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<ServiceResult<BookRequest>> AddBookRequest(BookRequestDTO bookRequestDTO)
    {
        try
        {
            if (bookRequestDTO == null)
                return ServiceResult<BookRequest>.FailureResult("Book request is null.");

            var book = await _bookRepository.GetByBookTitle(bookRequestDTO.BookTitle);
            if (book == null)
                return ServiceResult<BookRequest>.FailureResult($"Book with title '{bookRequestDTO.BookTitle}' not found.");

            var bookRequest = new BookRequest
            {
                BookId = book.Id,
                RequestDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var result = await _bookRequestRepository.AddAsync(bookRequest);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{bookRequest.Id}");
                return ServiceResult<BookRequest>.SuccessResult(result.Data);
            }

            return ServiceResult<BookRequest>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequest>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequest>> UpdateBookRequest(BookRequestDTO bookRequestDTO)
    {
        try
        {
            if (bookRequestDTO == null || bookRequestDTO.Id == Guid.Empty)
                return ServiceResult<BookRequest>.FailureResult("Invalid book request data.");

            var bookRequest = await _bookRequestRepository.GetByIdAsync(bookRequestDTO.Id);
            if (bookRequest == null)
                return ServiceResult<BookRequest>.FailureResult("Book request not found.");

            var book = await _bookRepository.GetByBookTitle(bookRequestDTO.BookTitle);
            if (book == null)
                return ServiceResult<BookRequest>.FailureResult($"Book with title '{bookRequestDTO.BookTitle}' not found.");

            bookRequest.BookId = book.Id;

            var result = await _bookRequestRepository.UpdateAsync(bookRequest);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{bookRequest.Id}");
                return ServiceResult<BookRequest>.SuccessResult(result.Data);
            }

            return ServiceResult<BookRequest>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequest>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequest>> DeleteBookRequest(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookRequest>.FailureResult("Invalid book request ID.");

            var result = await _bookRequestRepository.DeleteAsync(id);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{id}");
                return ServiceResult<BookRequest>.SuccessResult(result.Data);
            }

            return ServiceResult<BookRequest>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequest>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequest>> GetBookRequest(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookRequest>.FailureResult("Invalid book request ID.");

            string cacheKey = $"{BookRequestCacheKeyPrefix}{id}";
            var cached = await _cacheService.GetAsync<BookRequest>(cacheKey);
            if (cached != null)
                return ServiceResult<BookRequest>.SuccessResult(cached);

            var request = await _bookRequestRepository.GetByIdAsync(id);
            if (request != null)
            {
                await _cacheService.SetAsync(cacheKey, request, TimeSpan.FromMinutes(5));
                return ServiceResult<BookRequest>.SuccessResult(request);
            }

            return ServiceResult<BookRequest>.FailureResult("Book request not found.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequest>.FailureResult($"An error occurred: {ex.Message}");
        }
    }
    
    public async Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequests(BookRequestFilter filter)
    {
        try
        {
            if (filter == null)
                return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult("Invalid filter data.");

            var result = await _bookRequestRepository.GetBookRequestDetails(filter);
            if (result.Success)
                return ServiceResult<PaginationResult<BookRequestDetails>>.SuccessResult(result.Data);

            return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }
}
