using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.Contracts.ResponseModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class UpdatePageRequestHandler : IRequestHandler<UpdatePageRequest, PageDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdatePageRequestHandler> _logger;

    public UpdatePageRequestHandler(
        SttbprojectContext context,
        ILogger<UpdatePageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PageDetailResponse> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating page: {PageId}", request.PageId);

        var page = await _context.Pages
            .Include(p => p.CreatedByNavigation)
            .Include(p => p.UpdatedByNavigation)
            .FirstOrDefaultAsync(p => p.PageId == request.PageId, cancellationToken);

        if (page == null)
        {
            _logger.LogWarning("Page not found: {PageId}", request.PageId);
            throw new InvalidOperationException("Page not found");
        }

        page.Title = request.Title;
        page.Slug = request.Slug;
        page.Content = request.Content;
        page.Status = request.Status;
        page.UpdatedBy = request.UpdatedBy;
        page.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Page updated successfully: {PageId}", request.PageId);

        return new PageDetailResponse
        {
            PageId = page.PageId,
            Title = page.Title ?? string.Empty,
            Slug = page.Slug ?? string.Empty,
            Content = page.Content ?? string.Empty,
            Status = page.Status ?? string.Empty,
            CreatedBy = page.CreatedBy ?? 0,
            CreatedByName = page.CreatedByNavigation?.Name ?? string.Empty,
            UpdatedBy = page.UpdatedBy,
            UpdatedByName = page.UpdatedByNavigation?.Name,
            CreatedAt = page.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = page.UpdatedAt
        };
    }
}
