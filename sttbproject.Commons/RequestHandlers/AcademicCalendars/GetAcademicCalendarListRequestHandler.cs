using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class GetAcademicCalendarListRequestHandler : IRequestHandler<GetAcademicCalendarListRequest, GetAcademicCalendarListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetAcademicCalendarListRequestHandler> _logger;

    public GetAcademicCalendarListRequestHandler(
        SttbprojectContext context,
        ILogger<GetAcademicCalendarListRequestHandler> logger)
    {
        _context = context;
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
            .Select(a => new AcademicCalendarListItem
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
                CreatedAt = a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} academic calendar entries (Page {Page})", items.Count, request.PageNumber);

        return new GetAcademicCalendarListResponse
        {
            AcademicCalendars = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
