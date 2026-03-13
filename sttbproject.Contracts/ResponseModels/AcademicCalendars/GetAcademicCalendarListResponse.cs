namespace sttbproject.Contracts.ResponseModels.AcademicCalendars;

public class GetAcademicCalendarListResponse
{
    public List<AcademicCalendarListItem> AcademicCalendars { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class AcademicCalendarListItem
{
    public int AcademicCalendarId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AcademicYear { get; set; }
    public string? Semester { get; set; }
    public string? EventType { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? FeaturedImageId { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
}
