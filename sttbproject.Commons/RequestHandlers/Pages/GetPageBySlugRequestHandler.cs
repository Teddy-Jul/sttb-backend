using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.Contracts.ResponseModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class GetPageBySlugRequestHandler : IRequestHandler<GetPageBySlugRequest, PageDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetPageBySlugRequestHandler> _logger;

    public GetPageBySlugRequestHandler(
        SttbprojectContext context,
        ILogger<GetPageBySlugRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PageDetailResponse> Handle(GetPageBySlugRequest request, CancellationToken cancellationToken)
    {
        var page = await _context.Pages
            .Include(p => p.CreatedByNavigation)
            .Include(p => p.UpdatedByNavigation)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);

        if (page == null)
        {
            _logger.LogWarning("Page not found with slug: {Slug}", request.Slug);
            throw new InvalidOperationException("Page not found");
        }

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
