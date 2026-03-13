namespace sttbproject.Contracts.ResponseModels.Events;

public class GetEventListResponse
{
    public List<EventListItem> Events { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class EventListItem
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? FeaturedImageId { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
}
