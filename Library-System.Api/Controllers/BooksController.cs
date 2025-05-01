using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace Library_System.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBook(Guid id)
    {
        _logger.LogInformation("Fetching book with ID {BookId}.", id);

        var result = await _bookService.GetBook(id);

        if (result.Success)
        {
            _logger.LogInformation("Book fetched successfully: {BookId}", id);
            return Ok(result);
        }

        _logger.LogWarning("Failed to fetch book: {Message}", result.Message);
        return NotFound(new { Success = false, Message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] BooksFilter filter)
    {
        _logger.LogInformation("Fetching books with provided filter.");

        var result = await _bookService.GetBooks(filter);

        if (result.Success)
        {
            _logger.LogInformation("Books fetched successfully.");
            return Ok(result);
        }

        _logger.LogWarning("Failed to fetch books: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        _logger.LogInformation("Attempting to add a new book.");
        var result = await _bookService.AddBook(book);

        if (result.Success)
        {
            _logger.LogInformation("Book added successfully: {BookId}", result.Data.Id);
            return CreatedAtAction(nameof(GetBook), new { id = result.Data.Id }, result);
        }

        _logger.LogWarning("Failed to add book: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message }); 
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] Book book)
    {
        _logger.LogInformation("Attempting to update book with ID {BookId}.", id);

        if (id != book.Id)
        {
            _logger.LogWarning("ID mismatch: {RouteId} vs {BodyId}", id, book.Id);
            return BadRequest(new { Success = false, Message = "ID in route does not match ID in the body." });
        }

        var result = await _bookService.UpdateBook(book);

        if (result.Success)
        {
            _logger.LogInformation("Book updated successfully: {BookId}", id);
            return Ok(result);
        }

        _logger.LogWarning("Failed to update book: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        _logger.LogInformation("Attempting to delete book with ID {BookId}.", id);

        var result = await _bookService.DeleteBook(id);

        if (result.Success)
        {
            _logger.LogInformation("Book deleted successfully: {BookId}", id);
            return Ok(result);
        }

        _logger.LogWarning("Failed to delete book: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }
}
