namespace Library_System.Domain.Dtos;

public record BookRequestDTO(
    Guid Id,
    string BookTitle,
    string FirstName,
    string LastName,
    string ContactNumber
);
