using Library_System.Domain.Enums;

namespace Library_System.Domain.Entities;

public class Book
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public DateTimeOffset ReleaseDate { get; set; } 
    public BookStatus Status { get; set; }

    public ICollection<BookRequest> BookRequests { get; set; } = [];
}