using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.Contracts.ResponseModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class UpdateCategoryRequestHandler : IRequestHandler<UpdateCategoryRequest, CategoryDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateCategoryRequestHandler> _logger;

    public UpdateCategoryRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateCategoryRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CategoryDetailResponse> Handle(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category: {CategoryId}", request.CategoryId);

        // Use AsTracking to ensure EF Core tracks the entity for updates
        var category = await _context.Categories
            .Include(c => c.Posts)
            .AsTracking()
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
            throw new InvalidOperationException("Category not found");
        }

        // Update properties
        category.Name = request.Name;
        category.Slug = request.Slug;

        // Mark as modified explicitly
        _context.Entry(category).State = EntityState.Modified;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category updated successfully: {CategoryId}", request.CategoryId);

        return new CategoryDetailResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name ?? string.Empty,
            Slug = category.Slug ?? string.Empty,
            PostCount = category.Posts.Count
        };
    }
}
