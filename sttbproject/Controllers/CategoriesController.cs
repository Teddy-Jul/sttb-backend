using MediatR;
using Microsoft.AspNetCore.Mvc;
using sttbproject.Contracts.RequestModels.Categories;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        IMediator mediator,
        ILogger<CategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] GetCategoryListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create category");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Category not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update category");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> Delete(
        int categoryId,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new DeleteCategoryRequest { CategoryId = categoryId };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(new { success = result, message = "Category deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Category not found or has posts: {CategoryId}", categoryId);
            return BadRequest(new { message = ex.Message });
        }
    }
}
