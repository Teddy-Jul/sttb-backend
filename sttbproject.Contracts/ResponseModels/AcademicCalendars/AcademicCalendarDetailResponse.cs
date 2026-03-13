namespace sttbproject.Contracts.ResponseModels.AcademicCalendars;

public class AcademicCalendarDetailResponse
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
    public string Status { get; set; } = string.Empty;
    public int? FeaturedImageId { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
