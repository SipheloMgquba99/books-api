namespace Library_System.Domain.Entities;

public class Member
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string ContactNumber { get; set; } = string.Empty;
    
}