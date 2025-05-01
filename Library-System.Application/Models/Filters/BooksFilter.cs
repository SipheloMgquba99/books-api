using Library_System.Domain.Enums;

namespace Library_System.Application.Models.Filters;

public class BooksFilter
{
    public int PageIndex { get; init; } = 1;

    public int PageSize { get; init; } = 10;
    public string Title { get; init; } = string.Empty;
    
    public string Author { get; init; } = string.Empty;

    public string Isbn { get; init; } = string.Empty;
    
    public BookStatus? Status { get; init; }
}