using MediatR;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;

namespace sttbproject.Contracts.RequestModels.AcademicCalendars;

public class GetAcademicCalendarByIdRequest : IRequest<AcademicCalendarDetailResponse>
{
    public int AcademicCalendarId { get; set; }
}
