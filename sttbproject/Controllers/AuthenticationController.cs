using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.Authentication;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")] 
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IMediator mediator,
        ILogger<AuthenticationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized login attempt");
            return Unauthorized(new { message = ex.Message });
        }
    }

  
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("admin/login")]
    public async Task<IActionResult> AdminLogin(
        [FromBody] AdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized admin login attempt");
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("admin/register")]
    public async Task<IActionResult> AdminRegister(
        [FromBody] AdminRegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin registration failed");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Password changed successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized password change attempt");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Password change failed");
            return BadRequest(new { message = ex.Message });
        }
    }
}
