using Library_System.Domain.Entities;
public class BookRequest
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Book? Book { get; set; }
    public Guid BookRequestorId {get;set;}
    public BookRequestor? BookRequestor{get;set;}
    public DateTime RequestDate { get; set; }
    public DateTime ReturnDate { get; set; }
}