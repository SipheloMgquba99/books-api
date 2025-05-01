using Library_System.Domain.Enums;

namespace Library_System.Application.Models.Filters;

public class BookRequestFilter
{
    public int PageIndex { get; init; } = 1;
    
    public int PageSize { get; init; } = 10;

    public string? RequestorName { get; init; } 
    
    public string? BookTitle { get; init; } 
    
    public DateTime? RequestDate { get; init; }
}