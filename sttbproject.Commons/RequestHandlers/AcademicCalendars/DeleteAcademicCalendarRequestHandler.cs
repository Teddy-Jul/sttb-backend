using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class DeleteAcademicCalendarRequestHandler : IRequestHandler<DeleteAcademicCalendarRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteAcademicCalendarRequestHandler> _logger;

    public DeleteAcademicCalendarRequestHandler(SttbprojectContext context, ILogger<DeleteAcademicCalendarRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteAcademicCalendarRequest request, CancellationToken cancellationToken)
    {
        var item = await _context.AcademicCalendars
            .FirstOrDefaultAsync(a => a.AcademicCalendarId == request.AcademicCalendarId, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Academic calendar entry not found: {Id}", request.AcademicCalendarId);
            throw new InvalidOperationException("Academic calendar entry not found");
        }

        _context.AcademicCalendars.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academic calendar entry deleted: {Id}", request.AcademicCalendarId);
        return true;
    }
}
