using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Media;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")] // Apply general rate limit to all endpoints
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ILogger<MediaController> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
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
    [EnableRateLimiting("upload")] // Stricter limit for uploads
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Upload(
        [FromForm] UploadMediaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "Please select a file to upload" });
            }

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
    [EnableRateLimiting("write")] // Use write policy for delete
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

    /// <summary>
    /// Streams a media file directly from storage (R2 or local) by media ID.
    /// This endpoint is intentionally public — images should be accessible without auth.
    /// </summary>
    [HttpGet("{mediaId}/file")]
    public async Task<IActionResult> ServeFile(
        int mediaId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetMediaByIdRequest { MediaId = mediaId };
            var media = await _mediator.Send(request, cancellationToken);

            if (string.IsNullOrEmpty(media.FilePath))
                return NotFound(new { message = "File path not found" });

            var stream = await _fileStorageService.GetFileAsync(media.FilePath, cancellationToken);
            var contentType = string.IsNullOrEmpty(media.FileType) ? "application/octet-stream" : media.FileType;

            Response.Headers["Cache-Control"] = "public, max-age=86400";
            return File(stream, contentType, enableRangeProcessing: true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Media not found: {MediaId}", mediaId);
            return NotFound(new { message = ex.Message });
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "File not found in storage" });
        }
    }
}
