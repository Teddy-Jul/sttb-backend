using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.AcademicCalendars;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.AcademicCalendars;

public class GetAcademicCalendarBySlugRequestHandler : IRequestHandler<GetAcademicCalendarBySlugRequest, AcademicCalendarDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetAcademicCalendarBySlugRequestHandler> _logger;

    public GetAcademicCalendarBySlugRequestHandler(SttbprojectContext context, ILogger<GetAcademicCalendarBySlugRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AcademicCalendarDetailResponse> Handle(GetAcademicCalendarBySlugRequest request, CancellationToken cancellationToken)
    {
        var item = await _context.AcademicCalendars
            .FirstOrDefaultAsync(a => a.Slug == request.Slug, cancellationToken);

        if (item == null)
        {
            _logger.LogWarning("Academic calendar entry not found with slug: {Slug}", request.Slug);
            throw new InvalidOperationException("Academic calendar entry not found");
        }

        return GetAcademicCalendarByIdRequestHandler.MapToDetailResponse(item);
    }
}
