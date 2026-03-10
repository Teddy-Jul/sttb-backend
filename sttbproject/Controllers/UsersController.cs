using MediatR;
using Microsoft.AspNetCore.Mvc;
using sttbproject.Contracts.RequestModels.Users;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IMediator mediator,
        ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetUserListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(
        int userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetUserByIdRequest { UserId = userId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", userId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = await _mediator.Send(request, cancellationToken);
            return Ok(new { userId, message = "User created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "User updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(
        int userId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteUserRequest { UserId = userId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "User deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", userId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{userId}/status")]
    public async Task<IActionResult> UpdateStatus(
        int userId,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            request.UserId = userId;
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "User status updated successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User not found: {UserId}", userId);
            return NotFound(new { message = ex.Message });
        }
    }
}
