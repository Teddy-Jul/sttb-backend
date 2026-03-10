using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.Contracts.ResponseModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class CreatePageRequestHandler : IRequestHandler<CreatePageRequest, PageDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreatePageRequestHandler> _logger;

    public CreatePageRequestHandler(
        SttbprojectContext context,
        ILogger<CreatePageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PageDetailResponse> Handle(CreatePageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating page: {Title}", request.Title);

        var page = new Page
        {
            Title = request.Title,
            Slug = request.Slug,
            Content = request.Content,
            Status = request.Status,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.Pages.Add(page);
        await _context.SaveChangesAsync(cancellationToken);

        var createdPage = await _context.Pages
            .Include(p => p.CreatedByNavigation)
            .FirstAsync(p => p.PageId == page.PageId, cancellationToken);

        _logger.LogInformation("Page created successfully with ID: {PageId}", page.PageId);

        return new PageDetailResponse
        {
            PageId = createdPage.PageId,
            Title = createdPage.Title ?? string.Empty,
            Slug = createdPage.Slug ?? string.Empty,
            Content = createdPage.Content ?? string.Empty,
            Status = createdPage.Status ?? ContentStatus.Draft,
            CreatedBy = createdPage.CreatedBy ?? 0,
            CreatedByName = createdPage.CreatedByNavigation?.Name ?? string.Empty,
            CreatedAt = createdPage.CreatedAt ?? DateTime.MinValue
        };
    }
}
