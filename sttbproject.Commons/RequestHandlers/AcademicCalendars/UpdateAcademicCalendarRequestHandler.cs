using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class UpdateAcademicCalendarRequestHandler : IRequestHandler<UpdateAcademicCalendarRequest, AcademicCalendarDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateAcademicCalendarRequestHandler> _logger;

    public UpdateAcademicCalendarRequestHandler(SttbprojectContext context, ILogger<UpdateAcademicCalendarRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AcademicCalendarDetailResponse> Handle(UpdateAcademicCalendarRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating academic calendar entry: {Id}", request.AcademicCalendarId);

        var item = await _context.AcademicCalendars
            .FirstOrDefaultAsync(a => a.AcademicCalendarId == request.AcademicCalendarId, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Academic calendar entry not found: {Id}", request.AcademicCalendarId);
            throw new InvalidOperationException("Academic calendar entry not found");
        }

        var slugExists = await _context.AcademicCalendars
            .AnyAsync(a => a.Slug == request.Slug && a.AcademicCalendarId != request.AcademicCalendarId, cancellationToken);
        if (slugExists)
            throw new InvalidOperationException($"An academic calendar entry with slug '{request.Slug}' already exists");

        item.Title = request.Title;
        item.Slug = request.Slug;
        item.Description = request.Description;
        item.StartDate = request.StartDate;
        item.EndDate = request.EndDate;
        item.AcademicYear = request.AcademicYear;
        item.Semester = request.Semester;
        item.EventType = request.EventType;
        item.Status = request.Status;
        item.UpdatedBy = request.UpdatedBy;
        item.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academic calendar entry updated: {Id}", request.AcademicCalendarId);

        return GetAcademicCalendarByIdRequestHandler.MapToDetailResponse(item);
    }
}
