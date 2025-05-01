using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_System.Application.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<Book> _dbSet;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<Book>();
    }


    public async Task<Book?> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                return null;
            }

            return entity;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    
    public async Task<Book?> GetByBookTitle(string bookName)
    {
        try
        {
            var entity = await _dbSet.FirstOrDefaultAsync(b => b.Title.ToLower() == bookName.Trim().ToLower());
            return entity;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<ServiceResult<PaginationResult<Book>>> GetAllAsync(BooksFilter filter)
    {
        try
        {
            var booksQuery = _dbSet.AsQueryable();

            // Apply filters
            booksQuery = ApplyFilters(booksQuery, filter);

            // Apply pagination
            var pagedBooksQuery = booksQuery
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var books = await pagedBooksQuery.ToListAsync();

            var totalCount = await booksQuery.CountAsync();

            var pagedResult = new PaginationResult<Book>
            {
                Items = books.AsQueryable(),
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                PageNumber = filter.PageIndex,
                PageSize = filter.PageSize
            };

            return ServiceResult<PaginationResult<Book>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<Book>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<Book>> AddAsync(Book book)
    {
        try
        {
            _dbSet.Add(book);
            await _context.SaveChangesAsync();
            return ServiceResult<Book>.SuccessResult(book);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult("An error occurred while adding the book.");
        }
    }
    public async Task<ServiceResult<Book>> UpdateAsync(Book book)
    {
        try
        {
            var existingBook = await _dbSet.FindAsync(book.Id);
            if (existingBook == null)
            {
                return ServiceResult<Book>.FailureResult("Book not found.");
            }
            _context.Entry(existingBook).CurrentValues.SetValues(book);

            await _context.SaveChangesAsync();
            return ServiceResult<Book>.SuccessResult(existingBook);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult("An error occurred while updating the book.");
        }
    }
    public async Task<ServiceResult<Book>> DeleteAsync(Guid id)
    {
        try
        {
            var book = await _dbSet.FindAsync(id);
            if (book == null)
            {
                return ServiceResult<Book>.FailureResult("Book not found.");
            }

            _dbSet.Remove(book);
            await _context.SaveChangesAsync();
            return ServiceResult<Book>.SuccessResult(book);
        }
        catch (Exception ex)
        {
            return ServiceResult<Book>.FailureResult("An error occurred while deleting the book.");
        }
    }

    private IQueryable<Book> ApplyFilters(IQueryable<Book> query, BooksFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Title))
        {
            query = query.Where(m => m.Title.Contains(filter.Title));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(m => m.Status == filter.Status.Value);
        }

        if (!string.IsNullOrEmpty(filter.Author))
        {
            query = query.Where(m => m.Author.Contains(filter.Author));
        }

        if (!string.IsNullOrEmpty(filter.Isbn))
        {
            query = query.Where(m => m.Isbn.Contains(filter.Isbn));
        }

        return query;
    }
}