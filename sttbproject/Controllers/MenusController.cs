using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.Menus;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class MenusController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MenusController> _logger;

    public MenusController(
        IMediator mediator,
        ILogger<MenusController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetMenuListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{menuId}")]
    public async Task<IActionResult> GetById(
        int menuId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetMenuByIdRequest { MenuId = menuId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Menu not found: {MenuId}", menuId);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateMenuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create menu");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateMenuRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Menu not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update menu");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{menuId}")]
    public async Task<IActionResult> Delete(
        int menuId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteMenuRequest { MenuId = menuId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Menu deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Menu not found: {MenuId}", menuId);
            return NotFound(new { message = ex.Message });
        }
    }
}
