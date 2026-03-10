using MediatR;
using Microsoft.AspNetCore.Mvc;
using sttbproject.Contracts.RequestModels.Posts;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IMediator mediator,
        ILogger<PostsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetPostListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create post");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create post");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdatePostRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Post not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update post");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> Delete(
        int postId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeletePostRequest { PostId = postId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Post deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Post not found: {PostId}", postId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{postId}/publish")]
    public async Task<IActionResult> Publish(
        int postId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new PublishPostRequest { PostId = postId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Post published successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Post not found: {PostId}", postId);
            return NotFound(new { message = ex.Message });
        }
    }
}
