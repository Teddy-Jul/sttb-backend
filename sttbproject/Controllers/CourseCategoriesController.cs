using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using sttbproject.Contracts.RequestModels.CourseCategories;

namespace sttbproject.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("general")]
public class CourseCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCourseCategoryListRequest(), cancellationToken);
        return Ok(result);
    }
}
