using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class DeleteCategoryRequestHandler : IRequestHandler<DeleteCategoryRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteCategoryRequestHandler> _logger;

    public DeleteCategoryRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteCategoryRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category: {CategoryId}", request.CategoryId);

        var category = await _context.Categories
            .Include(c => c.Posts)
            .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
            throw new InvalidOperationException("Category not found");
        }

        if (category.Posts.Any())
        {
            _logger.LogWarning("Cannot delete category with posts: {CategoryId}", request.CategoryId);
            throw new InvalidOperationException("Cannot delete category that has posts. Please reassign or delete the posts first.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category deleted successfully: {CategoryId}", request.CategoryId);

        return true;
    }
}
