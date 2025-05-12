namespace Library_System.Domain.Dtos;

public record BookRequestorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string ContactNumber
);
