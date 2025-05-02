using System;
using System.Linq;
using System.Threading.Tasks;
using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_System.Application.Repositories;

public class BookRequestRepository : IBookRequestRepository
{
    private readonly LibraryDbContext _context;
    private readonly DbSet<BookRequest> _dbSet;

    public BookRequestRepository(LibraryDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<BookRequest>();
    }

    public async Task<BookRequest?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbSet
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.Id == id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ServiceResult<PaginationResult<BookRequestDetails>>> GetBookRequestDetails(BookRequestFilter filter)
    {
        try
        {
            if (filter == null)
            {
                return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult("Invalid filter data.");
            }

            var bookRequestQuery = _dbSet
                .Include(br => br.Book)
                .Select(br => new BookRequestDetails
                {
                    BookTitle = br.Book.Title,
                    Author = br.Book.Author,
                    RequestDate = br.RequestDate,
                    ReturnDate = br.ReturnDate
                });

            bookRequestQuery = ApplyFilters(bookRequestQuery, filter);

            var totalCount = await bookRequestQuery.CountAsync();
            var pagedBookRequestQuery = bookRequestQuery
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var bookRequestDetails = await pagedBookRequestQuery.ToListAsync();

            var pagedResult = new PaginationResult<BookRequestDetails>
            {
                Items = bookRequestDetails.AsQueryable(),
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize),
                PageNumber = filter.PageIndex,
                PageSize = filter.PageSize
            };

            return ServiceResult<PaginationResult<BookRequestDetails>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginationResult<BookRequestDetails>>.FailureResult($"An error occurred: {ex.Message}");
        }
    }

    public async Task<ServiceResult<BookRequest>> AddAsync(BookRequest bookRequest)
    {
        try
        {
            if (bookRequest == null)
                return ServiceResult<BookRequest>.FailureResult("Book request is null.");

            _dbSet.Add(bookRequest);
            await _context.SaveChangesAsync();
            return ServiceResult<BookRequest>.SuccessResult(bookRequest);
        }
        catch
        {
            return ServiceResult<BookRequest>.FailureResult("An error occurred while adding the book request.");
        }
    }

    public async Task<ServiceResult<BookRequest>> UpdateAsync(BookRequest bookRequest)
    {
        try
        {
            var existing = await _dbSet
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.Id == bookRequest.Id);

            if (existing == null)
                return ServiceResult<BookRequest>.FailureResult("Book request not found.");

            existing.RequestDate = bookRequest.RequestDate;
            existing.ReturnDate = bookRequest.ReturnDate;

            await _context.SaveChangesAsync();
            return ServiceResult<BookRequest>.SuccessResult(existing);
        }
        catch
        {
            return ServiceResult<BookRequest>.FailureResult("An error occurred while updating the book request.");
        }
    }

    public async Task<ServiceResult<BookRequest>> DeleteAsync(Guid id)
    {
        try
        {
            var bookRequest = await _dbSet
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (bookRequest == null)
                return ServiceResult<BookRequest>.FailureResult("Book request not found.");

            _dbSet.Remove(bookRequest);
            await _context.SaveChangesAsync();
            return ServiceResult<BookRequest>.SuccessResult(bookRequest);
        }
        catch
        {
            return ServiceResult<BookRequest>.FailureResult("An error occurred while deleting the book request.");
        }
    }

    public async Task<BookRequestor?> GetByContactAsync(string contactNumber)
    {
        return await _context.BookRequestors
            .FirstOrDefaultAsync(br => br.ContactNumber == contactNumber);
    }

    public async Task<ServiceResult<BookRequestor>> AddAsync(BookRequestor requestor)
    {
        try
        {
            await _context.BookRequestors.AddAsync(requestor);
            await _context.SaveChangesAsync();
            return ServiceResult<BookRequestor>.SuccessResult(requestor);
        }
        catch (Exception ex)
        {
            return ServiceResult<BookRequestor>.FailureResult($"Error adding book requestor: {ex.Message}");
        }
    }

    private IQueryable<BookRequestDetails> ApplyFilters(IQueryable<BookRequestDetails> query, BookRequestFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.BookTitle))
        {
            query = query.Where(br => br.BookTitle.Contains(filter.BookTitle));
        }

        if (filter.RequestDate.HasValue)
        {
            query = query.Where(br => br.RequestDate.Date == filter.RequestDate.Value.Date);
        }

        return query;
    }
}
