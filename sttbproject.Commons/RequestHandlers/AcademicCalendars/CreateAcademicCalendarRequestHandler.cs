using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class CreateAcademicCalendarRequestHandler : IRequestHandler<CreateAcademicCalendarRequest, AcademicCalendarDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<CreateAcademicCalendarRequestHandler> _logger;

    public CreateAcademicCalendarRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<CreateAcademicCalendarRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<AcademicCalendarDetailResponse> Handle(CreateAcademicCalendarRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating academic calendar entry: {Title}", request.Title);

        var slugExists = await _context.AcademicCalendars
            .AnyAsync(a => a.Slug == request.Slug, cancellationToken);
        if (slugExists)
            throw new InvalidOperationException($"An academic calendar entry with slug '{request.Slug}' already exists");

        var item = new AcademicCalendar
        {
            Title = request.Title,
            Slug = request.Slug,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AcademicYear = request.AcademicYear,
            Semester = request.Semester,
            EventType = request.EventType,
            Status = request.Status,
            FeaturedImageId = request.FeaturedImageId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.AcademicCalendars.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Academic calendar entry created with ID: {Id}", item.AcademicCalendarId);

        // Reload with FeaturedImage for URL mapping
        await _context.Entry(item).Reference(a => a.FeaturedImage).LoadAsync(cancellationToken);

        return GetAcademicCalendarByIdRequestHandler.MapToDetailResponse(item, _fileStorageService);
    }
}
