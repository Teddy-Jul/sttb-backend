using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.Contracts.ResponseModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class GetEventByIdRequestHandler : IRequestHandler<GetEventByIdRequest, EventDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetEventByIdRequestHandler> _logger;

    public GetEventByIdRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetEventByIdRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<EventDetailResponse> Handle(GetEventByIdRequest request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .Include(e => e.FeaturedImage)
            .FirstOrDefaultAsync(e => e.EventId == request.EventId, cancellationToken);

        if (ev == null)
        {
            _logger.LogWarning("Event not found with ID: {EventId}", request.EventId);
            throw new InvalidOperationException("Event not found");
        }

        return MapToDetailResponse(ev, _fileStorageService);
    }

    public static EventDetailResponse MapToDetailResponse(Event ev, IFileStorageService fileStorageService)
    {
        return new EventDetailResponse
        {
            EventId = ev.EventId,
            Title = ev.Title ?? string.Empty,
            Slug = ev.Slug ?? string.Empty,
            Description = ev.Description,
            Location = ev.Location,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            FeaturedImageId = ev.FeaturedImageId,
            FeaturedImageUrl = ev.FeaturedImage != null
                ? fileStorageService.GetFileUrl(ev.FeaturedImage.FilePath ?? string.Empty)
                : null,
            Status = ev.Status ?? string.Empty,
            CreatedBy = ev.CreatedBy,
            UpdatedBy = ev.UpdatedBy,
            CreatedAt = ev.CreatedAt,
            UpdatedAt = ev.UpdatedAt
        };
    }
}
