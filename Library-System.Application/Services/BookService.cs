using System;
using System.Threading.Tasks;
using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Cache.Interfaces;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    private const string BookCacheKeyPrefix = "book:";

    public BookService(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
    }

    public async Task<ServiceResult<Book>> AddBook(Book book)
    {
        try
        {
            if (book == null)
                return ServiceResult<Book>.FailureResult("Book is null.");

            var result = await _bookRepository.AddAsync(book);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{book.Id}");
                return ServiceResult<Book>.SuccessResult(result.Data);
            }

            return ServiceResult<Book>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<Book>> UpdateBook(Book book)
    {
        try
        {
            if (book == null || book.Id == Guid.Empty)
                return ServiceResult<Book>.FailureResult("Invalid book data.");

            var result = await _bookRepository.UpdateAsync(book);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{book.Id}");
                return ServiceResult<Book>.SuccessResult(result.Data);
            }

            return ServiceResult<Book>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<Book>> DeleteBook(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<Book>.FailureResult("Invalid book ID.");

            var result = await _bookRepository.DeleteAsync(id);
            if (result.Success)
            {
                await _cacheService.RemoveAsync($"{BookCacheKeyPrefix}{id}");
                return ServiceResult<Book>.SuccessResult(result.Data);
            }

            return ServiceResult<Book>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<Book>> GetBook(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return ServiceResult<Book>.FailureResult("Invalid book ID.");

            string cacheKey = $"{BookCacheKeyPrefix}{id}";
            var cachedBook = await _cacheService.GetAsync<Book>(cacheKey);
            if (cachedBook != null)
                return ServiceResult<Book>.SuccessResult(cachedBook);
            
            var book = await _bookRepository.GetByIdAsync(id);
            if (book != null)
            {
                await _cacheService.SetAsync(cacheKey, book, TimeSpan.FromMinutes(5));
                return ServiceResult<Book>.SuccessResult(book);
            }

            return ServiceResult<Book>.FailureResult("Book not found.");
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<PaginationResult<Book>>> GetBooks(BooksFilter filter)
    {
        try
        {
            if (filter == null)
                return ServiceResult<PaginationResult<Book>>.FailureResult("Invalid filter data.");
            
            var result = await _bookRepository.GetAllAsync(filter);
            
            if (result.Success)
                return ServiceResult<PaginationResult<Book>>.SuccessResult(result.Data);

            return ServiceResult<PaginationResult<Book>>.FailureResult(result.Message);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<Book>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }
}
