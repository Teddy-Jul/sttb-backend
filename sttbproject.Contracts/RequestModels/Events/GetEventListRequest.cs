using MediatR;
using sttbproject.Contracts.ResponseModels.Events;

namespace sttbproject.Contracts.RequestModels.Events;

public class GetEventListRequest : IRequest<GetEventListResponse>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    /// <summary>Filter events whose date range overlaps on or after this date (inclusive).</summary>
    public DateTime? StartDateFrom { get; set; }
    /// <summary>Filter events whose date range overlaps on or before this date (inclusive).</summary>
    public DateTime? StartDateTo { get; set; }
}
