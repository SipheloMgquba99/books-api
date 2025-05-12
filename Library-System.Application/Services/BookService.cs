using System;
using System.Threading.Tasks;
using AutoMapper;
using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Cache.Interfaces;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private const string BookCacheKeyPrefix = "book:";

    public BookService(IBookRepository bookRepository, IMapper mapper, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ServiceResult<BookDto>> AddBook(BookDto bookDto)
    {
        try
        {
            if (bookDto == null)
                return ServiceResult<BookDto>.FailureResult("Book data is null.");

            var book = _mapper.Map<Book>(bookDto);
            var result = await _bookRepository.AddAsync(book);

            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{book.Id}");
                var addedBookDto = _mapper.Map<BookDto>(result.Data);
                return ServiceResult<BookDto>.SuccessResult(addedBookDto);
            }

            return ServiceResult<BookDto>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookDto>> UpdateBook(BookDto bookDto)
    {
        try
        {
            if (bookDto == null || bookDto.Id == Guid.Empty)
                return ServiceResult<BookDto>.FailureResult("Invalid book data.");

            var book = _mapper.Map<Book>(bookDto);
            var result = await _bookRepository.UpdateAsync(book);

            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{book.Id}");
                var updatedBookDto = _mapper.Map<BookDto>(result.Data);
                return ServiceResult<BookDto>.SuccessResult(updatedBookDto);
            }

            return ServiceResult<BookDto>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookDto>> DeleteBook(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookDto>.FailureResult("Invalid book ID.");

            var result = await _bookRepository.DeleteAsync(id);

            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{id}");
                var deletedBookDto = _mapper.Map<BookDto>(result.Data);
                return ServiceResult<BookDto>.SuccessResult(deletedBookDto);
            }

            return ServiceResult<BookDto>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookDto>> GetBook(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<BookDto>.FailureResult("Invalid book ID.");

            string cacheKey = $"{BookCacheKeyPrefix}{id}";
            var cachedBook = await _cacheService.GetAsync<Book>(cacheKey);

            if (cachedBook != null)
            {
                var cachedBookDto = _mapper.Map<BookDto>(cachedBook);
                return ServiceResult<BookDto>.SuccessResult(cachedBookDto);
            }

            var book = await _bookRepository.GetByIdAsync(id);

            if (book != null)
            {
                await _cacheService.SetAsync(cacheKey, book, TimeSpan.FromMinutes(5));
                var bookDto = _mapper.Map<BookDto>(book);
                return ServiceResult<BookDto>.SuccessResult(bookDto);
            }

            return ServiceResult<BookDto>.FailureResult("Book not found.");
        }
        catch (Exception ex)
        {
            return ServiceResult<BookDto>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<PaginationResult<BookDto>>> GetBooks(BooksFilter filter)
    {
        try
        {
            if (filter == null)
                return ServiceResult<PaginationResult<BookDto>>.FailureResult("Invalid filter data.");

            var result = await _bookRepository.GetAllAsync(filter);

            if (result.Success)
            {
                var pagedBooksDto = new PaginationResult<BookDto>
                {
                    Items = result.Data.Items.Select(book => _mapper.Map<BookDto>(book)).AsQueryable(),
                    TotalCount = result.Data.TotalCount,
                    TotalPages = result.Data.TotalPages,
                    PageNumber = result.Data.PageNumber,
                    PageSize = result.Data.PageSize
                };

                return ServiceResult<PaginationResult<BookDto>>.SuccessResult(pagedBooksDto);
            }

            return ServiceResult<PaginationResult<BookDto>>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<BookDto>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }
}
