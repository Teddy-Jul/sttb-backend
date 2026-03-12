using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.Courses;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(
        IMediator mediator,
        ILogger<CoursesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetCourseListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create course");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Course not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update course");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{courseId}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Delete(
        int courseId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteCourseRequest { CourseId = courseId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Course deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", courseId);
            return NotFound(new { message = ex.Message });
        }
    }
}