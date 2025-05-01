using Library_System.Application.Interfaces.IServices;
using Library_System.Application.Models.Filters;
using Library_System.Domain.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Library_System.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookRequestsController : ControllerBase
{
    private readonly IBookRequestService _bookRequestService;
    private readonly ILogger<BookRequestsController> _logger;

    public BookRequestsController(IBookRequestService bookRequestService, ILogger<BookRequestsController> logger)
    {
        _bookRequestService = bookRequestService;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBookRequest(Guid id)
    {
        _logger.LogInformation("Fetching book request with ID {Id}", id);

        var result = await _bookRequestService.GetBookRequest(id);
        if (result.Success)
        {
            _logger.LogInformation("Book request fetched successfully with ID {Id}", id);
            return Ok(result);
        }

        _logger.LogWarning("Book request with ID {Id} not found.", id);
        return NotFound(new { Success = false, Message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> GetBookRequests([FromQuery] BookRequestFilter filter)
    {
        _logger.LogInformation("Fetching book requests with filter: {Filter}", filter);

        var result = await _bookRequestService.GetBookRequests(filter);
        if (result.Success)
        {
            _logger.LogInformation("Book requests fetched successfully.");
            return Ok(result);
        }

        _logger.LogWarning("Failed to fetch book requests: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> AddBookRequest([FromBody] BookRequestDTO bookRequestDto)
    {
        _logger.LogInformation("Adding a new book request.");

        var result = await _bookRequestService.AddBookRequest(bookRequestDto);
        if (result.Success)
        {
            _logger.LogInformation("Book request added successfully with ID {Id}", result.Data.Id);
            return CreatedAtAction(nameof(GetBookRequest), new { id = result.Data.Id }, result); 
        }

        _logger.LogWarning("Failed to add book request: {Message}", result.Message);
        return BadRequest(new { Success = false, Message = result.Message }); 
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBookRequest([FromBody] BookRequestDTO bookRequestDTO)
    {
        if (bookRequestDTO == null && bookRequestDTO.Id == Guid.Empty)
        {
            _logger.LogWarning("Invalid book request data.");
            return BadRequest(new { Success = false, Message = "Invalid book request data." });
        }
        
        _logger.LogInformation("Updating book request with ID {Id}", bookRequestDTO.Id);
        
        var result = await _bookRequestService.UpdateBookRequest(bookRequestDTO);
    
        if (result.Success)
        {
            _logger.LogInformation("Book request updated successfully with ID {Id}", bookRequestDTO.Id);
            return Ok(result);
        }

        _logger.LogWarning("Failed to update book request with ID {Id}: {Message}", bookRequestDTO.Id, result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBookRequest(Guid id)
    {
        _logger.LogInformation("Deleting book request with ID {Id}", id);

        var result = await _bookRequestService.DeleteBookRequest(id);
        if (result.Success)
        {
            _logger.LogInformation("Book request deleted successfully with ID {Id}", id);
            return Ok(result); 
        }

        _logger.LogWarning("Failed to delete book request with ID {Id}: {Message}", id, result.Message);
        return BadRequest(new { Success = false, Message = result.Message });
    }
}
