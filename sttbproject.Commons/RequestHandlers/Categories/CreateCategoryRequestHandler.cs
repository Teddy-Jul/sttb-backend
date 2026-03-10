using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.Contracts.ResponseModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Categories;

public class CreateCategoryRequestHandler : IRequestHandler<CreateCategoryRequest, CategoryDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreateCategoryRequestHandler> _logger;

    public CreateCategoryRequestHandler(
        SttbprojectContext context,
        ILogger<CreateCategoryRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CategoryDetailResponse> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category: {Name}", request.Name);

        var category = new Category
        {
            Name = request.Name,
            Slug = request.Slug
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category created successfully with ID: {CategoryId}", category.CategoryId);

        return new CategoryDetailResponse
        {
            CategoryId = category.CategoryId,
            Name = category.Name ?? string.Empty,
            Slug = category.Slug ?? string.Empty,
            PostCount = 0
        };
    }
}
