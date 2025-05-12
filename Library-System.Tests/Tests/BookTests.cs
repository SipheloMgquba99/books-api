using AutoMapper;
using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Cache.Interfaces;
using Library_System.Tests.Builders;
using Moq;
using Xunit;

namespace Library_System.Tests.Tests
{
	public class BookTests
	{
		private Mock<IBookRepository> _bookRepository;
		private Mock<IMapper> _mapper;
		private Mock<ICacheService> _cacheService;
		private IBookService _bookService;

		public BookTests()
		{
			_bookRepository = new Mock<IBookRepository>();
			_mapper = new Mock<IMapper>();
			_cacheService = new Mock<ICacheService>();
		}

		[Fact]
		public async Task AddBook_ShouldAddBook_Should_Return_Success()
		{
			// Arrange
			var bookDto = new BookDtoBuilder().Build();
			var book = new BooksBuilder().Build();

			_bookRepository.Setup(repo => repo.AddAsync(It.IsAny<Book>()))
				.ReturnsAsync(ServiceResult<Book>.SuccessResult(book));

			_mapper.Setup(mapper => mapper.Map<Book>(It.IsAny<BookDto>()))
				.Returns(book);

			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.AddBook(bookDto);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRepository.Verify(repo => repo.AddAsync(It.IsAny<Book>()), Times.Once);
		}

		[Test]
		public async Task AddBook_ShouldReturnFailure_WhenBookDtoIsNull()
		{
			// Arrange
			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.AddBook(null);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task UpdateBook_ShouldUpdateBook_Should_Return_Success()
		{
			// Arrange
			var bookDto = new BookDtoBuilder().WithId(Guid.NewGuid()).Build();
			var book = new BooksBuilder().Build();

			_bookRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Book>()))
				.ReturnsAsync(ServiceResult<Book>.SuccessResult(book));

			_mapper.Setup(mapper => mapper.Map<Book>(It.IsAny<BookDto>()))
				.Returns(book);

			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.UpdateBook(bookDto);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Book>()), Times.Once);
		}

		[Test]
		public async Task UpdateBook_ShouldReturnFailure_WhenBookDtoIsNull()
		{
			// Arrange
			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.UpdateBook(null);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task DeleteBook_ShouldDeleteBook_Should_Return_Success()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var book = new BooksBuilder().Build();

			_bookRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
				.ReturnsAsync(ServiceResult<Book>.SuccessResult(book));

			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.DeleteBook(bookId);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
		}

		[Test]
		public async Task DeleteBook_ShouldReturnFailure_WhenBookIdIsEmpty()
		{
			// Arrange
			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.DeleteBook(Guid.Empty);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task GetBook_ShouldReturnBook_WhenBookExists()
		{
			// Arrange
			var bookId = Guid.NewGuid();
			var book = new BooksBuilder().Build();
			var bookDto = new BookDtoBuilder().Build();

			_cacheService.Setup(cache => cache.GetAsync<Book>(It.IsAny<string>()))
				.ReturnsAsync((Book)null);

			_bookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(book);

			_mapper.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>()))
				.Returns(bookDto);

			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.GetBook(bookId);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.NotNull(result.Data);
			_bookRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
		}

		[Test]
		public async Task GetBook_ShouldReturnFailure_WhenBookIdIsEmpty()
		{
			// Arrange
			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.GetBook(Guid.Empty);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task GetBooks_ShouldReturnBooks_WhenFilterIsValid()
		{
			// Arrange
			var filter = new BooksFilter();
			var books = new List<Book> { new BooksBuilder().Build() };
			var pagedResult = new PaginationResult<Book>
			{
				Items = books.AsQueryable(),
				TotalCount = 1,
				TotalPages = 1,
				PageNumber = 1,
				PageSize = 10
			};

			_bookRepository.Setup(repo => repo.GetAllAsync(It.IsAny<BooksFilter>()))
				.ReturnsAsync(ServiceResult<PaginationResult<Book>>.SuccessResult(pagedResult));

			_mapper.Setup(mapper => mapper.Map<BookDto>(It.IsAny<Book>()))
				.Returns(new BookDtoBuilder().Build());

			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.GetBooks(filter);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.IsNotEmpty(result.Data.Items);
			_bookRepository.Verify(repo => repo.GetAllAsync(It.IsAny<BooksFilter>()), Times.Once);
		}

		[Test]
		public async Task GetBooks_ShouldReturnFailure_WhenFilterIsNull()
		{
			// Arrange
			_bookService = new BookService(_bookRepository.Object, _mapper.Object, _cacheService.Object);

			// Act
			var result = await _bookService.GetBooks(null);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}
	}
}
