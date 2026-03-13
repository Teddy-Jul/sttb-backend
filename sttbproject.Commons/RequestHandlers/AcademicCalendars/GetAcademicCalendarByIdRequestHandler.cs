using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class GetAcademicCalendarByIdRequestHandler : IRequestHandler<GetAcademicCalendarByIdRequest, AcademicCalendarDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetAcademicCalendarByIdRequestHandler> _logger;

    public GetAcademicCalendarByIdRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetAcademicCalendarByIdRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<AcademicCalendarDetailResponse> Handle(GetAcademicCalendarByIdRequest request, CancellationToken cancellationToken)
    {
        var item = await _context.AcademicCalendars
            .Include(a => a.FeaturedImage)
            .FirstOrDefaultAsync(a => a.AcademicCalendarId == request.AcademicCalendarId, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Academic calendar entry not found with ID: {Id}", request.AcademicCalendarId);
            throw new InvalidOperationException("Academic calendar entry not found");
        }

        return MapToDetailResponse(item, _fileStorageService);
    }

    public static AcademicCalendarDetailResponse MapToDetailResponse(AcademicCalendar item, IFileStorageService fileStorageService)
    {
        return new AcademicCalendarDetailResponse
        {
            AcademicCalendarId = item.AcademicCalendarId,
            Title = item.Title ?? string.Empty,
            Slug = item.Slug ?? string.Empty,
            Description = item.Description,
            StartDate = item.StartDate,
            EndDate = item.EndDate,
            AcademicYear = item.AcademicYear,
            Semester = item.Semester,
            EventType = item.EventType,
            Status = item.Status ?? string.Empty,
            FeaturedImageId = item.FeaturedImageId,
            FeaturedImageUrl = item.FeaturedImage != null
                ? fileStorageService.GetFileUrl(item.FeaturedImage.FilePath ?? string.Empty)
                : null,
            CreatedBy = item.CreatedBy,
            UpdatedBy = item.UpdatedBy,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
