using MediatR;
using Microsoft.AspNetCore.Mvc;
using sttbproject.Contracts.RequestModels.Media;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MediaController> _logger;

    public MediaController(
        IMediator mediator,
        ILogger<MediaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetMediaListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{mediaId}")]
    public async Task<IActionResult> GetById(
        int mediaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetMediaByIdRequest { MediaId = mediaId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Media not found: {MediaId}", mediaId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] int uploadedBy,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new UploadMediaRequest
            {
                File = file,
                UploadedBy = uploadedBy
            };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{mediaId}")]
    public async Task<IActionResult> Delete(
        int mediaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteMediaRequest { MediaId = mediaId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Media deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Media not found: {MediaId}", mediaId);
            return NotFound(new { message = ex.Message });
        }
    }
}
