public record BookRequestDto(
    Guid Id,
    string BookTitle,
    string FirstName,
    string LastName,
    string ContactNumber,
    DateTime RequestDate,
    DateTime ReturnDate
);
