using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.Events;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IMediator mediator, ILogger<EventsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetEventListRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{eventId:int}")]
    public async Task<IActionResult> GetById(int eventId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetEventByIdRequest { EventId = eventId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Event not found: {EventId}", eventId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetEventBySlugRequest { Slug = slug };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Event not found: {Slug}", slug);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Create(
        [FromBody] CreateEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create event");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create event");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Event not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update event");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{eventId:int}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Delete(int eventId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteEventRequest { EventId = eventId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Event deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Event not found: {EventId}", eventId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{eventId:int}/publish")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Publish(int eventId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new PublishEventRequest { EventId = eventId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Event published successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Event not found: {EventId}", eventId);
            return NotFound(new { message = ex.Message });
        }
    }
}
