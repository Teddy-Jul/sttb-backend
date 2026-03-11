using MediatR;
using Microsoft.AspNetCore.Mvc;
using sttbproject.Contracts.RequestModels.StudyPrograms;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudyProgramsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StudyProgramsController> _logger;

    public StudyProgramsController(
        IMediator mediator,
        ILogger<StudyProgramsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetStudyProgramListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{programId}")]
    public async Task<IActionResult> GetById(
        int programId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetStudyProgramByIdRequest { ProgramId = programId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Study program not found: {ProgramId}", programId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetStudyProgramBySlugRequest { Slug = slug };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Study program not found: {Slug}", slug);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateStudyProgramRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create study program");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateStudyProgramRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Study program not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update study program");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{programId}")]
    public async Task<IActionResult> Delete(
        int programId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteStudyProgramRequest { ProgramId = programId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Study program deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Study program not found: {ProgramId}", programId);
            return NotFound(new { message = ex.Message });
        }
    }
}