using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.AcademicCalendars;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class AcademicCalendarsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademicCalendarsController> _logger;

    public AcademicCalendarsController(IMediator mediator, ILogger<AcademicCalendarsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetAcademicCalendarListRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{academicCalendarId:int}")]
    public async Task<IActionResult> GetById(int academicCalendarId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetAcademicCalendarByIdRequest { AcademicCalendarId = academicCalendarId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Academic calendar entry not found: {Id}", academicCalendarId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetAcademicCalendarBySlugRequest { Slug = slug };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Academic calendar entry not found: {Slug}", slug);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAcademicCalendarRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create academic calendar entry");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create academic calendar entry");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateAcademicCalendarRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Academic calendar entry not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update academic calendar entry");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{academicCalendarId:int}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Delete(int academicCalendarId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteAcademicCalendarRequest { AcademicCalendarId = academicCalendarId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Academic calendar entry deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Academic calendar entry not found: {Id}", academicCalendarId);
            return NotFound(new { message = ex.Message });
        }
    }
}
