using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class DeleteEventRequestHandler : IRequestHandler<DeleteEventRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteEventRequestHandler> _logger;

    public DeleteEventRequestHandler(SttbprojectContext context, ILogger<DeleteEventRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

        if (ev == null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            throw new InvalidOperationException("Event not found");
        }

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event deleted: {EventId}", request.EventId);
        return true;
    }
}
