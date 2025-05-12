using Library_System.Domain.Enums;

namespace Library_System.Domain.Dtos;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    string Publisher,
    int Quantity,
    DateTimeOffset ReleaseDate,
    BookStatus Status
);
