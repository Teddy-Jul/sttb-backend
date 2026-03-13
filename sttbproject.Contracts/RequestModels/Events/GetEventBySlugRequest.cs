using MediatR;
using sttbproject.Contracts.ResponseModels.Events;

namespace sttbproject.Contracts.RequestModels.Events;

public class GetEventBySlugRequest : IRequest<EventDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
