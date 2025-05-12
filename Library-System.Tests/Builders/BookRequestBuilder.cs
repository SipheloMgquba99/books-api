using Library_System.Domain.Entities;

namespace Library_System.Tests.Builders;

public class BookRequestBuilder
{
	public Guid _id = Guid.NewGuid();
	public Guid _bookId = Guid.NewGuid();
	public string _bookTitle = "Test Book";
	public string _requestorFirstName = "John";
	public string _requestorLastName = "Doe";
	public string _requestorContactNumber = "1234567890";
	public DateTime _requestDate = DateTime.UtcNow;
	public DateTime _returnDate = DateTime.UtcNow.AddDays(7);

	public BookRequestBuilder WithId(Guid id)
	{
		_id = id;
		return this;
	}

	public BookRequestBuilder WithBookId(Guid bookId)
	{
		_bookId = bookId;
		return this;
	}

	public BookRequestBuilder WithBookTitle(string bookTitle)
	{
		_bookTitle = bookTitle;
		return this;
	}

	public BookRequestBuilder WithRequestorFirstName(string firstName)
	{
		_requestorFirstName = firstName;
		return this;
	}

	public BookRequestBuilder WithRequestorLastName(string lastName)
	{
		_requestorLastName = lastName;
		return this;
	}

	public BookRequestBuilder WithRequestorContactNumber(string contactNumber)
	{
		_requestorContactNumber = contactNumber;
		return this;
	}

	public BookRequestBuilder WithRequestDate(DateTime requestDate)
	{
		_requestDate = requestDate;
		return this;
	}

	public BookRequestBuilder WithReturnDate(DateTime returnDate)
	{
		_returnDate = returnDate;
		return this;
	}

	public BookRequest Build()
	{
		return new BookRequest
		{
			Id = _id,
			BookId = _bookId,
			Book = new Book { Id = _bookId, Title = _bookTitle },
			BookRequestor = new BookRequestor
			{
				Id = Guid.NewGuid(),
				FirstName = _requestorFirstName,
				LastName = _requestorLastName,
				ContactNumber = _requestorContactNumber
			},
			RequestDate = _requestDate,
			ReturnDate = _returnDate
		};
	}
}
