using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.ContactMessages;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class ContactMessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ContactMessagesController> _logger;

    public ContactMessagesController(
        IMediator mediator,
        ILogger<ContactMessagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetContactMessageListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{contactMessageId}")]
    public async Task<IActionResult> GetById(
        int contactMessageId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetContactMessageByIdRequest { ContactMessageId = contactMessageId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact message not found: {ContactMessageId}", contactMessageId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactMessageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create contact message");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{contactMessageId}/status")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> UpdateStatus(
        int contactMessageId,
        [FromBody] UpdateContactMessageStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            request.ContactMessageId = contactMessageId;
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Status updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact message not found: {ContactMessageId}", contactMessageId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{contactMessageId}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Delete(
        int contactMessageId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteContactMessageRequest { ContactMessageId = contactMessageId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Contact message deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Contact message not found: {ContactMessageId}", contactMessageId);
            return NotFound(new { message = ex.Message });
        }
    }
}
