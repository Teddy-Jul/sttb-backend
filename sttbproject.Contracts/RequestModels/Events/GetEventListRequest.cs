using MediatR;
using sttbproject.Contracts.ResponseModels.Events;

namespace sttbproject.Contracts.RequestModels.Events;

public class GetEventListRequest : IRequest<GetEventListResponse>
{
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
