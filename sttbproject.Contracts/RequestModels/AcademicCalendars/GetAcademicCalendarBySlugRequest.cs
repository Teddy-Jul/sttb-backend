using MediatR;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;

namespace sttbproject.Contracts.RequestModels.AcademicCalendars;

public class GetAcademicCalendarBySlugRequest : IRequest<AcademicCalendarDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
