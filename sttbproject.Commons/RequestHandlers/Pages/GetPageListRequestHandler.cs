using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.Contracts.ResponseModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class GetPageListRequestHandler : IRequestHandler<GetPageListRequest, GetPageListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetPageListRequestHandler> _logger;

    public GetPageListRequestHandler(
        SttbprojectContext context,
        ILogger<GetPageListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetPageListResponse> Handle(GetPageListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Pages
            .Include(p => p.CreatedByNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Title!.Contains(request.SearchTerm) || p.Content!.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(p => p.Status == request.Status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pages = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PageListItem
            {
                PageId = p.PageId,
                Title = p.Title ?? string.Empty,
                Slug = p.Slug ?? string.Empty,
                Status = p.Status ?? string.Empty,
                CreatedByName = p.CreatedByNavigation!.Name ?? string.Empty,
                CreatedAt = p.CreatedAt ?? DateTime.MinValue,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} pages (Page {Page})", pages.Count, request.PageNumber);

        return new GetPageListResponse
        {
            Pages = pages,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
