using Library_System.Domain.Entities;

public class BookRequest
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Book? Book { get; set; } 
    public Guid MemberId { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime ReturnDate { get; set; }
}