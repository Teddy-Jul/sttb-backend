using MediatR;

namespace sttbproject.Contracts.RequestModels.AcademicCalendars;

public class DeleteAcademicCalendarRequest : IRequest<bool>
{
    public int AcademicCalendarId { get; set; }
}
