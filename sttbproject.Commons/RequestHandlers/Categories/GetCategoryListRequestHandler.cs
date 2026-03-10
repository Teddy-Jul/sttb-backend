using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.Contracts.ResponseModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class GetCategoryListRequestHandler : IRequestHandler<GetCategoryListRequest, GetCategoryListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetCategoryListRequestHandler> _logger;

    public GetCategoryListRequestHandler(
        SttbprojectContext context,
        ILogger<GetCategoryListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetCategoryListResponse> Handle(GetCategoryListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsQueryable();

        // Apply search filter first
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name!.Contains(request.SearchTerm) || c.Slug!.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering and pagination
        var categories = await query
            .OrderBy(c => c.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryListItem
            {
                CategoryId = c.CategoryId,
                Name = c.Name ?? string.Empty,
                Slug = c.Slug ?? string.Empty,
                PostCount = c.Posts.Count
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} categories (Page {Page})", categories.Count, request.PageNumber);

        return new GetCategoryListResponse
        {
            Categories = categories,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
