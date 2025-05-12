using Library_System.Domain.Dtos;
using Library_System.Domain.Enums;

namespace Library_System.Tests.Builders;

public class BookDtoBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Default Book Title";
    private string _author = "Default Author";
    private string _isbn = "1234567890";
    private string _publisher = "Default Publisher";
    private int _quantity = 1;
    private DateTimeOffset _releaseDate = DateTimeOffset.UtcNow;
    private BookStatus _status = BookStatus.Available;

    public BookDtoBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BookDtoBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public BookDtoBuilder WithAuthor(string author)
    {
        _author = author;
        return this;
    }

    public BookDtoBuilder WithIsbn(string isbn)
    {
        _isbn = isbn;
        return this;
    }

    public BookDtoBuilder WithPublisher(string publisher)
    {
        _publisher = publisher;
        return this;
    }

    public BookDtoBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public BookDtoBuilder WithReleaseDate(DateTimeOffset releaseDate)
    {
        _releaseDate = releaseDate;
        return this;
    }

    public BookDtoBuilder WithStatus(BookStatus status)
    {
        _status = status;
        return this;
    }

    public BookDto Build()
    {
        return new BookDto(
            _id,
            _title,
            _author,
            _isbn,
            _publisher,
            _quantity,
            _releaseDate,
            _status
        );
    }
}