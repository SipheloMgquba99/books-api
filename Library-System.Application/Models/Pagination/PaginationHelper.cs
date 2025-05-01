using Microsoft.EntityFrameworkCore;

namespace Library_System.Application.Models;

public static class PaginationHelper
{
    public static async Task<PaginationResult<T>> GetPaginationResult<T>(IQueryable<T> query, int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<T>
        {
            Items = items.AsQueryable(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}