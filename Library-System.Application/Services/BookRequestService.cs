using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;
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

    public async Task<ServiceResult<BookRequestDto>> AddBookRequest(BookRequestDto bookRequestDTO)
    {
        try
        {
            if (bookRequestDTO == null)
                return ServiceResult<BookRequestDto>.FailureResult("Book request data is null.");

            var book = await GetOrCacheBookAsync(bookRequestDTO.BookTitle);
            if (book == null)
                return ServiceResult<BookRequestDto>.FailureResult($"Book with title '{bookRequestDTO.BookTitle}' not found.");

            var requestor = await GetOrCreateRequestorAsync(bookRequestDTO.FirstName, bookRequestDTO.LastName, bookRequestDTO.ContactNumber);
            if (requestor == null)
                return ServiceResult<BookRequestDto>.FailureResult("Failed to save book requestor.");

            var bookRequest = new BookRequest
            {
                Id = Guid.NewGuid(),
                BookId = book.Id,
                BookRequestorId = requestor.Id,
                RequestDate = DateTime.UtcNow,
                ReturnDate = DateTime.UtcNow.AddDays(7)
            };

            var result = await _bookRequestRepository.AddAsync(bookRequest);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{bookRequest.Id}");

                return ServiceResult<BookRequestDto>.SuccessResult(new BookRequestDto(
                    bookRequest.Id,
                    book.Title,
                    requestor.FirstName,
                    requestor.LastName,
                    requestor.ContactNumber,
                    bookRequest.RequestDate,
                    bookRequest.ReturnDate
                ));
            }

            return ServiceResult<BookRequestDto>.FailureResult(result.Message ?? "An unknown error occurred.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequestDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequestDto>> UpdateBookRequest(BookRequestDto bookRequestDTO)
    {
        try
        {
            if (bookRequestDTO == null || bookRequestDTO.Id == Guid.Empty)
                return ServiceResult<BookRequestDto>.FailureResult("Invalid book request data.");

            var bookRequest = await _bookRequestRepository.GetByIdAsync(bookRequestDTO.Id);
            if (bookRequest == null)
                return ServiceResult<BookRequestDto>.FailureResult("Book request not found.");

            var book = await _bookRepository.GetByBookTitle(bookRequestDTO.BookTitle);
            if (book == null)
                return ServiceResult<BookRequestDto>.FailureResult($"Book with title '{bookRequestDTO.BookTitle}' not found.");

            bookRequest.BookId = book.Id;

            var result = await _bookRequestRepository.UpdateAsync(bookRequest);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{bookRequest.Id}");

                return ServiceResult<BookRequestDto>.SuccessResult(new BookRequestDto(
                    bookRequest.Id,
                    book.Title,
                    bookRequestDTO.FirstName,
                    bookRequestDTO.LastName,
                    bookRequestDTO.ContactNumber,
                    bookRequest.RequestDate,
                    bookRequest.ReturnDate
                ));
            }

            return ServiceResult<BookRequestDto>.FailureResult(result.Message ?? "An unknown error occurred.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequestDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequestDto>> DeleteBookRequest(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookRequestDto>.FailureResult("Invalid book request ID.");

            var bookRequest = await _bookRequestRepository.GetByIdAsync(id);
            if (bookRequest == null)
                return ServiceResult<BookRequestDto>.FailureResult("Book request not found.");

            var result = await _bookRequestRepository.DeleteAsync(bookRequest.Id);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookRequestCacheKeyPrefix}{id}");
                return ServiceResult<BookRequestDto>.SuccessResult(new BookRequestDto(
                    bookRequest.Id,
                    bookRequest.Book?.Title ?? "",
                    bookRequest.BookRequestor?.FirstName ?? "",
                    bookRequest.BookRequestor?.LastName ?? "",
                    bookRequest.BookRequestor?.ContactNumber ?? "",
                    bookRequest.RequestDate,
                    bookRequest.ReturnDate
                ));
            }

            return ServiceResult<BookRequestDto>.FailureResult(result.Message ?? "An unknown error occurred.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequestDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequestDto>> GetBookRequest(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookRequestDto>.FailureResult("Invalid book request ID.");

            string cacheKey = $"{BookRequestCacheKeyPrefix}{id}";
            var cached = await _cacheService.GetAsync<BookRequest>(cacheKey);
            var request = cached ?? await _bookRequestRepository.GetByIdAsync(id);

            if (request != null)
            {
                if (cached == null)
                    await _cacheService.SetAsync(cacheKey, request, TimeSpan.FromMinutes(5));

                return ServiceResult<BookRequestDto>.SuccessResult(new BookRequestDto(
                    request.Id,
                    request.Book?.Title ?? string.Empty,
                    request.BookRequestor?.FirstName ?? string.Empty,
                    request.BookRequestor?.LastName ?? string.Empty,
                    request.BookRequestor?.ContactNumber ?? string.Empty,
                    request.RequestDate,
                    request.ReturnDate
                ));
            }

            return ServiceResult<BookRequestDto>.FailureResult("Book request not found.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequestDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }


    public async Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequests(BookRequestFilter filter)
    {
        try
        {
            if (filter == null)
                return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult("Invalid filter data.");

            var result = await _bookRequestRepository.GetBookRequestDetails(filter);
            if (result.Success && result.Data != null)
            {
                var sanitizedData = result.Data.Items.Select(item => new BookRequestDetails
                {
                    BookTitle = item.BookTitle,
                    Author = item.Author,
                    Requestor = string.IsNullOrWhiteSpace(item.Requestor) ? string.Empty : item.Requestor,
                    ContactNumber = string.IsNullOrWhiteSpace(item.ContactNumber) ? string.Empty : item.ContactNumber,
                    RequestDate = item.RequestDate,
                    ReturnDate = item.ReturnDate
                }).AsQueryable(); 

                return ServiceResult<PaginationResult<BookRequestDetails>>.SuccessResult(new PaginationResult<BookRequestDetails>
                {
                    Items = sanitizedData,
                    TotalCount = result.Data.TotalCount
                });
            }

            return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult(result.Message ?? "An unknown error occurred.");
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    private async Task<Book?> GetOrCacheBookAsync(string bookTitle)
    {
        const int CacheExpirationMinutes = 10;
        string bookCacheKey = $"{BookRequestCacheKeyPrefix}book:{bookTitle}";

        var book = await _cacheService.GetAsync<Book>(bookCacheKey);
        if (book == null)
        {
            book = await _bookRepository.GetByBookTitle(bookTitle);
            if (book != null)
            {
                await _cacheService.SetAsync(bookCacheKey, book, TimeSpan.FromMinutes(CacheExpirationMinutes));
            }
        }

        return book;
    }

    private async Task<BookRequestor?> GetOrCreateRequestorAsync(string firstName, string lastName, string contactNumber)
    {
        const int CacheExpirationMinutes = 10;
        string requestorCacheKey = $"{BookRequestCacheKeyPrefix}requestor:{contactNumber}";

        var requestor = await _cacheService.GetAsync<BookRequestor>(requestorCacheKey);
        if (requestor == null)
        {
            requestor = await _bookRequestRepository.GetByContactAsync(contactNumber);
            if (requestor == null)
            {
                requestor = new BookRequestor
                {
                    Id = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    ContactNumber = contactNumber
                };

                var result = await _bookRequestRepository.AddAsync(requestor);
                if (!result.Success)
                    return null;

                await _cacheService.SetAsync(requestorCacheKey, requestor, TimeSpan.FromMinutes(CacheExpirationMinutes));
            }
            else
            {
                await _cacheService.SetAsync(requestorCacheKey, requestor, TimeSpan.FromMinutes(CacheExpirationMinutes));
            }
        }

        return requestor;
    }
}
