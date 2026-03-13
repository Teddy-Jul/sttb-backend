using MediatR;
using sttbproject.Contracts.ResponseModels.Events;

namespace sttbproject.Contracts.RequestModels.Events;

public class CreateEventRequest : IRequest<EventDetailResponse>
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? FeaturedImageId { get; set; }
    public string Status { get; set; } = "draft";
    public int? CreatedBy { get; set; }
}
