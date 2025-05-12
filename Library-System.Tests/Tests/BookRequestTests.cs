using Library_System.Application.Interfaces.IRepositories;
using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;
using Library_System.Infrastructure.Cache.Interfaces;
using Library_System.Tests.Builders;
using Moq;

namespace Library_System.Tests.Tests
{
	public class BookRequestTests
	{
		private Mock<IBookRequestRepository> _bookRequestRepository;
		private Mock<IBookRepository> _bookRepository;
		private Mock<ICacheService> _cacheService;
		private IBookRequestService _bookRequestService;

		public BookRequestTests()
		{
			_bookRequestRepository = new Mock<IBookRequestRepository>();
			_bookRepository = new Mock<IBookRepository>();
			_cacheService = new Mock<ICacheService>();
			_bookRequestService = new BookRequestService(
				_bookRequestRepository.Object,
				_bookRepository.Object,
				_cacheService.Object
			);
		}

		[Test]
		public async Task AddBookRequest_ShouldAddRequest_WhenValidData()
		{
			// Arrange
			var bookRequestDto = new BookRequestDtoBuilder().Build();
			var bookRequest = new BookRequestBuilder().Build();
			var bookDto = new BookDtoBuilder().Build();
			var book = new BooksBuilder().Build();
			var requestor = new BookRequestor { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", ContactNumber = "1234567890" };

			_bookRepository.Setup(repo => repo.GetByBookTitle(It.IsAny<string>()))
				.ReturnsAsync(book);

			_bookRequestRepository.Setup(repo => repo.GetByContactAsync(It.IsAny<string>()))
				.ReturnsAsync(requestor);

			_bookRequestRepository.Setup(repo => repo.AddAsync(It.IsAny<BookRequest>()))
				.ReturnsAsync(ServiceResult<BookRequest>.SuccessResult(new BookRequest()));

			// Act
			var result = await _bookRequestService.AddBookRequest(bookRequestDto);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRepository.Verify(repo => repo.GetByBookTitle(It.IsAny<string>()), Times.Once);
			_bookRequestRepository.Verify(repo => repo.AddAsync(It.IsAny<BookRequest>()), Times.Once);
		}

		[Test]
		public async Task AddBookRequest_ShouldReturnFailure_WhenBookNotFound()
		{
			// Arrange
			var bookRequestDto = new BookRequestDtoBuilder().Build();

			_bookRepository.Setup(repo => repo.GetByBookTitle(It.IsAny<string>()))
				.ReturnsAsync((Book?)null);

			// Act
			var result = await _bookRequestService.AddBookRequest(bookRequestDto);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task UpdateBookRequest_ShouldUpdateRequest_WhenValidData()
		{
			// Arrange
			var bookRequestDto = new BookRequestDtoBuilder().WithId(Guid.NewGuid()).Build();
			var bookRequest = new BookRequestBuilder().Build();
			var bookDto = new BookDtoBuilder().Build();
			var book = new BooksBuilder().Build();

			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(bookRequest);

			_bookRepository.Setup(repo => repo.GetByBookTitle(It.IsAny<string>()))
				.ReturnsAsync(book);

			_bookRequestRepository.Setup(repo => repo.UpdateAsync(It.IsAny<BookRequest>()))
				.ReturnsAsync(ServiceResult<BookRequest>.SuccessResult(bookRequest));

			// Act
			var result = await _bookRequestService.UpdateBookRequest(bookRequestDto);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRequestRepository.Verify(repo => repo.UpdateAsync(It.IsAny<BookRequest>()), Times.Once);
		}

		[Test]
		public async Task UpdateBookRequest_ShouldReturnFailure_WhenRequestNotFound()
		{
			// Arrange
			var bookRequestDto = new BookRequestDtoBuilder().WithId(Guid.NewGuid()).Build();

			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync((BookRequest?)null);

			// Act
			var result = await _bookRequestService.UpdateBookRequest(bookRequestDto);

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task DeleteBookRequest_ShouldDeleteRequest_WhenValidId()
		{
			// Arrange
			var bookRequest = new BookRequestBuilder().Build();

			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(bookRequest);

			_bookRequestRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>()))
				.ReturnsAsync(ServiceResult<BookRequest>.SuccessResult(bookRequest));

			// Act
			var result = await _bookRequestService.DeleteBookRequest(bookRequest.Id);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRequestRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Once);
		}

		[Test]
		public async Task DeleteBookRequest_ShouldReturnFailure_WhenRequestNotFound()
		{
			// Arrange
			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync((BookRequest?)null);

			// Act
			var result = await _bookRequestService.DeleteBookRequest(Guid.NewGuid());

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task GetBookRequest_ShouldReturnRequest_WhenValidId()
		{
			// Arrange
			var bookRequest = new BookRequestBuilder().Build();

			_cacheService.Setup(cache => cache.GetAsync<BookRequest>(It.IsAny<string>()))
				.ReturnsAsync((BookRequest?)null);

			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync(bookRequest);

			// Act
			var result = await _bookRequestService.GetBookRequest(bookRequest.Id);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			_bookRequestRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
		}

		[Test]
		public async Task GetBookRequest_ShouldReturnFailure_WhenRequestNotFound()
		{
			// Arrange
			_bookRequestRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
				.ReturnsAsync((BookRequest?)null);

			// Act
			var result = await _bookRequestService.GetBookRequest(Guid.NewGuid());

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}

		[Test]
		public async Task GetBookRequests_ShouldReturnRequests_WhenValidFilter()
		{
			// Arrange
			var filter = new BookRequestFilter();
			var bookRequests = new List<BookRequestDetails>
			{
				new BookRequestDetails
				{
					BookTitle = "Test Book",
					Requestor = "John Doe",
					ContactNumber = "1234567890",
					RequestDate = DateTime.UtcNow,
					ReturnDate = DateTime.UtcNow.AddDays(7)
				}
			};

			var pagedResult = new PaginationResult<BookRequestDetails>
			{
				Items = bookRequests.AsQueryable(),
				TotalCount = 1
			};

			_bookRequestRepository.Setup(repo => repo.GetBookRequestDetails(It.IsAny<BookRequestFilter>()))
				.ReturnsAsync(ServiceResult<PaginationResult<BookRequestDetails>>.SuccessResult(pagedResult));

			// Act
			var result = await _bookRequestService.GetBookRequests(filter);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Success);
			Assert.NotNull(result.Data?.Items);
			_bookRequestRepository.Verify(repo => repo.GetBookRequestDetails(It.IsAny<BookRequestFilter>()), Times.Once);
		}

		[Test]
		public async Task GetBookRequests_ShouldReturnFailure_WhenFilterIsNull()
		{
			// Act
			var result = await _bookRequestService.GetBookRequests(new BookRequestFilter());

			// Assert
			Assert.NotNull(result);
			Assert.False(result.Success);
		}
	}
}
