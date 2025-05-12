using Library_System.Domain.Dtos;

namespace Library_System.Tests.Builders;

public class BookRequestDtoBuilder
{
	private Guid _id = Guid.NewGuid();
	private string _bookTitle = "Default Book Title";
	private string _firstName = "John";
	private string _lastName = "Doe";
	private string _contactNumber = "1234567890";
	private DateTime _requestDate = DateTime.Now;
	private DateTime _returnDate = DateTime.Now.AddDays(14);

	public BookRequestDtoBuilder WithId(Guid id)
	{
		_id = id;
		return this;
	}

	public BookRequestDtoBuilder WithBookTitle(string bookTitle)
	{
		_bookTitle = bookTitle;
		return this;
	}

	public BookRequestDtoBuilder WithFirstName(string firstName)
	{
		_firstName = firstName;
		return this;
	}

	public BookRequestDtoBuilder WithLastName(string lastName)
	{
		_lastName = lastName;
		return this;
	}

	public BookRequestDtoBuilder WithContactNumber(string contactNumber)
	{
		_contactNumber = contactNumber;
		return this;
	}

	public BookRequestDtoBuilder WithRequestDate(DateTime requestDate)
	{
		_requestDate = requestDate;
		return this;
	}

	public BookRequestDtoBuilder WithReturnDate(DateTime returnDate)
	{
		_returnDate = returnDate;
		return this;
	}

	public BookRequestDto Build()
	{
		return new BookRequestDto(
			_id,
			_bookTitle,
			_firstName,
			_lastName,
			_contactNumber,
			_requestDate,
			_returnDate
		);
	}
}
