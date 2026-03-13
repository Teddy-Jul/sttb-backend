using MediatR;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;

namespace sttbproject.Contracts.RequestModels.AcademicCalendars;

public class GetAcademicCalendarListRequest : IRequest<GetAcademicCalendarListResponse>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public string? AcademicYear { get; set; }
    public string? Semester { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
