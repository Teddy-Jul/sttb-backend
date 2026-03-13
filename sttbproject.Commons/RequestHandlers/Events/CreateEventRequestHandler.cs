using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.Contracts.ResponseModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class CreateEventRequestHandler : IRequestHandler<CreateEventRequest, EventDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CreateEventRequestHandler> _logger;

    public CreateEventRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<CreateEventRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<EventDetailResponse> Handle(CreateEventRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating event: {Title}", request.Title);

        var slugExists = await _context.Events
            .AnyAsync(e => e.Slug == request.Slug, cancellationToken);
        if (slugExists)
            throw new InvalidOperationException($"An event with slug '{request.Slug}' already exists");

        var ev = new Event
        {
            Title = request.Title,
            Slug = request.Slug,
            Description = request.Description,
            Location = request.Location,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            FeaturedImageId = request.FeaturedImageId,
            Status = request.Status,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync(cancellationToken);

        var created = await _context.Events
            .Include(e => e.FeaturedImage)
            .FirstAsync(e => e.EventId == ev.EventId, cancellationToken);

        _logger.LogInformation("Event created successfully with ID: {EventId}", ev.EventId);

        return GetEventByIdRequestHandler.MapToDetailResponse(created, _fileStorageService);
    }
}
