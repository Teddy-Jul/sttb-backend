using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class PublishEventRequestHandler : IRequestHandler<PublishEventRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<PublishEventRequestHandler> _logger;

    public PublishEventRequestHandler(SttbprojectContext context, ILogger<PublishEventRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(PublishEventRequest request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

        if (ev == null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            throw new InvalidOperationException("Event not found");
        }

        ev.Status = ContentStatus.Published;
        ev.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event published: {EventId}", request.EventId);
        return true;
    }
}
