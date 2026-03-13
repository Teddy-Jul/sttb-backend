using MediatR;
using sttbproject.Contracts.ResponseModels.AcademicCalendars;

namespace sttbproject.Contracts.RequestModels.AcademicCalendars;

public class UpdateAcademicCalendarRequest : IRequest<AcademicCalendarDetailResponse>
{
    public int AcademicCalendarId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AcademicYear { get; set; }
    public string? Semester { get; set; }
    public string? EventType { get; set; }
    public string Status { get; set; } = "published";
    public int? UpdatedBy { get; set; }
}
