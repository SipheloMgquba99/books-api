namespace Library_System.Domain.Entities;

public class BookRequestor
{
    public Guid Id {get;set;}
    public string FirstName {get;set;} = string.Empty;
    public string LastName {get;set;} = string.Empty;
    public string ContactNumber {get;set;} = string.Empty;
    public ICollection<BookRequest> BookRequests { get; set; } = [];
}