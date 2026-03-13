using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class GetAcademicCalendarListRequestHandler : IRequestHandler<GetAcademicCalendarListRequest, GetAcademicCalendarListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetAcademicCalendarListRequestHandler> _logger;

    public GetAcademicCalendarListRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetAcademicCalendarListRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<GetAcademicCalendarListResponse> Handle(GetAcademicCalendarListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.AcademicCalendars.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(a => a.Title.Contains(request.SearchTerm) || (a.Description != null && a.Description.Contains(request.SearchTerm)));

        if (!string.IsNullOrWhiteSpace(request.Status))
            query = query.Where(a => a.Status == request.Status);

        if (!string.IsNullOrWhiteSpace(request.AcademicYear))
            query = query.Where(a => a.AcademicYear == request.AcademicYear);

        if (!string.IsNullOrWhiteSpace(request.Semester))
            query = query.Where(a => a.Semester == request.Semester);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(a => a.StartDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(a => a.FeaturedImage)
            .ToListAsync(cancellationToken);

        var mapped = items.Select(a => new AcademicCalendarListItem
        {
            AcademicCalendarId = a.AcademicCalendarId,
            Title = a.Title ?? string.Empty,
            Slug = a.Slug ?? string.Empty,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            AcademicYear = a.AcademicYear,
            Semester = a.Semester,
            EventType = a.EventType,
            Status = a.Status ?? string.Empty,
            FeaturedImageId = a.FeaturedImageId,
            FeaturedImageUrl = a.FeaturedImage != null
                ? _fileStorageService.GetFileUrl(a.FeaturedImage.FilePath ?? string.Empty)
                : null,
            CreatedAt = a.CreatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} academic calendar entries (Page {Page})", mapped.Count, request.PageNumber);

        return new GetAcademicCalendarListResponse
        {
            AcademicCalendars = mapped,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
