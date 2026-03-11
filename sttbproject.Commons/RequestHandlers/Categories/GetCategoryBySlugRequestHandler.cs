using MediatR;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.Contracts.ResponseModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class GetCategoryBySlugRequestHandler : IRequestHandler<GetCategoryBySlugRequest, CategoryDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetCategoryBySlugRequestHandler> _logger;

    public GetCategoryBySlugRequestHandler(
        SttbprojectContext context,
        ILogger<GetCategoryBySlugRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CategoryDetailResponse> Handle(GetCategoryBySlugRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.Slug == request.Slug, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category not found with Slug: {Slug}", request.Slug);
            throw new InvalidOperationException("Category not found");
        }

        return new CategoryDetailResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name ?? string.Empty,
            Slug = category.Slug ?? string.Empty,
            PostCount = category.Posts.Count
        };
    }
}
