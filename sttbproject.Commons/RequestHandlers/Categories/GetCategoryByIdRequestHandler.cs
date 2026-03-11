using MediatR;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.Contracts.ResponseModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class GetCategoryByIdRequestHandler : IRequestHandler<GetCategoryByIdRequest, CategoryDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetCategoryByIdRequestHandler> _logger;

    public GetCategoryByIdRequestHandler(
        SttbprojectContext context,
        ILogger<GetCategoryByIdRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CategoryDetailResponse> Handle(GetCategoryByIdRequest request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category not found with ID: {CategoryId}", request.CategoryId);
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
