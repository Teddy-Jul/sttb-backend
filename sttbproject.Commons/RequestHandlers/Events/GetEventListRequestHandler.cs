using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.Contracts.ResponseModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class GetEventListRequestHandler : IRequestHandler<GetEventListRequest, GetEventListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetEventListRequestHandler> _logger;

    public GetEventListRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetEventListRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<GetEventListResponse> Handle(GetEventListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Events
            .Include(e => e.FeaturedImage)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(e => e.Title.Contains(request.SearchTerm) || (e.Description != null && e.Description.Contains(request.SearchTerm)));

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(e => e.Status == request.Status);

        // Date range overlap filter: event overlaps [StartDateFrom, StartDateTo] when
        // StartDate <= to AND (EndDate >= from OR (EndDate == null AND StartDate >= from))
        if (request.StartDateFrom.HasValue)
            query = query.Where(e => (e.EndDate != null ? e.EndDate >= request.StartDateFrom.Value : e.StartDate >= request.StartDateFrom.Value));

        if (request.StartDateTo.HasValue)
            query = query.Where(e => e.StartDate <= request.StartDateTo.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var events = await query
            .OrderByDescending(e => e.StartDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EventListItem
            {
                EventId = e.EventId,
                Title = e.Title ?? string.Empty,
                Slug = e.Slug ?? string.Empty,
                Location = e.Location,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Status = e.Status ?? string.Empty,
                FeaturedImageId = e.FeaturedImageId,
                FeaturedImageUrl = e.FeaturedImage != null
                    ? _fileStorageService.GetFileUrl(e.FeaturedImage.FilePath ?? string.Empty)
                    : null,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} events (Page {Page})", events.Count, request.PageNumber);

        return new GetEventListResponse
        {
            Events = events,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
