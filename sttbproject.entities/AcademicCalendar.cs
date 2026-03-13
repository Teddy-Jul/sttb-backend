using System;

namespace sttbproject.entities;

public partial class AcademicCalendar
{
    public int AcademicCalendarId { get; set; }
    public string Title { get; set; } = null!;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AcademicYear { get; set; }
    public string? Semester { get; set; }
    public string? EventType { get; set; }
    public string? Status { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
    public virtual User? UpdatedByNavigation { get; set; }
}
