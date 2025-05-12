using Library_System.Domain.Entities;
using Library_System.Domain.Enums;

namespace Library_System.Tests.Builders;

public class BooksBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _description = "Default Description";
    private string _title = "Default Title";
    private string _author = "Default Author";
    private string _isbn = "000-0000000000";
    private string _publisher = "Default Publisher";
    private int _quantity = 1;
    private DateTimeOffset _releaseDate = DateTimeOffset.UtcNow;
    private BookStatus _status = BookStatus.Available;

    public BooksBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BooksBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public BooksBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public BooksBuilder WithAuthor(string author)
    {
        _author = author;
        return this;
    }

    public BooksBuilder WithIsbn(string isbn)
    {
        _isbn = isbn;
        return this;
    }

    public BooksBuilder WithPublisher(string publisher)
    {
        _publisher = publisher;
        return this;
    }

    public BooksBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public BooksBuilder WithReleaseDate(DateTimeOffset releaseDate)
    {
        _releaseDate = releaseDate;
        return this;
    }

    public BooksBuilder WithStatus(BookStatus status)
    {
        _status = status;
        return this;
    }

    public Book Build()
    {
        return new Book
        {
            Id = _id,
            Description = _description,
            Title = _title,
            Author = _author,
            Isbn = _isbn,
            Publisher = _publisher,
            Quantity = _quantity,
            ReleaseDate = _releaseDate,
            Status = _status
        };
    }
}