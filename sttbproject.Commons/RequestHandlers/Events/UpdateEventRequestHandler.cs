using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.Contracts.ResponseModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class UpdateEventRequestHandler : IRequestHandler<UpdateEventRequest, EventDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UpdateEventRequestHandler> _logger;

    public UpdateEventRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<UpdateEventRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<EventDetailResponse> Handle(UpdateEventRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating event: {EventId}", request.EventId);

        var ev = await _context.Events
            .Include(e => e.FeaturedImage)
            .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

        if (ev == null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            throw new InvalidOperationException("Event not found");
        }

        // Check slug uniqueness (exclude self)
        var slugExists = await _context.Events
            .AnyAsync(e => e.Slug == request.Slug && e.EventId != request.EventId, cancellationToken);
        if (slugExists)
            throw new InvalidOperationException($"An event with slug '{request.Slug}' already exists");

        ev.Title = request.Title;
        ev.Slug = request.Slug;
        ev.Description = request.Description;
        ev.Location = request.Location;
        ev.StartDate = request.StartDate;
        ev.EndDate = request.EndDate;
        ev.FeaturedImageId = request.FeaturedImageId;
        ev.Status = request.Status;
        ev.UpdatedBy = request.UpdatedBy;
        ev.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Reload FeaturedImage navigation if changed
        if (ev.FeaturedImageId.HasValue && ev.FeaturedImage == null)
            await _context.Entry(ev).Reference(e => e.FeaturedImage).LoadAsync(cancellationToken);

        _logger.LogInformation("Event updated successfully: {EventId}", request.EventId);

        return GetEventByIdRequestHandler.MapToDetailResponse(ev, _fileStorageService);
    }
}
