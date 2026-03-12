using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.Pages;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class PagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PagesController> _logger;

    public PagesController(
        IMediator mediator,
        ILogger<PagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetPageListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetById(
        int pageId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetPageByIdRequest { PageId = pageId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", pageId);
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
            var request = new GetPageBySlugRequest { Slug = slug };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Page not found with slug: {Slug}", slug);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create page");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Update(
        [FromBody] UpdatePageRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Page not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update page");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{pageId}")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Delete(
        int pageId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeletePageRequest { PageId = pageId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Page deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", pageId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{pageId}/publish")]
    [EnableRateLimiting("write")]
    public async Task<IActionResult> Publish(
        int pageId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new PublishPageRequest { PageId = pageId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Page published successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Page not found: {PageId}", pageId);
            return NotFound(new { message = ex.Message });
        }
    }
}
