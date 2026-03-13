using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Events;
using sttbproject.Contracts.ResponseModels.Events;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Events;

public class GetEventBySlugRequestHandler : IRequestHandler<GetEventBySlugRequest, EventDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetEventBySlugRequestHandler> _logger;

    public GetEventBySlugRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetEventBySlugRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<EventDetailResponse> Handle(GetEventBySlugRequest request, CancellationToken cancellationToken)
    {
        var ev = await _context.Events
            .Include(e => e.FeaturedImage)
            .FirstOrDefaultAsync(e => e.Slug == request.Slug, cancellationToken);

        if (ev == null)
        {
            _logger.LogWarning("Event not found with slug: {Slug}", request.Slug);
            throw new InvalidOperationException("Event not found");
        }

        return GetEventByIdRequestHandler.MapToDetailResponse(ev, _fileStorageService);
    }
}
