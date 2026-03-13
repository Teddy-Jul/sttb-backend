using MediatR;
using sttbproject.Contracts.ResponseModels.Events;

namespace sttbproject.Contracts.RequestModels.Events;

public class GetEventByIdRequest : IRequest<EventDetailResponse>
{
    public int EventId { get; set; }
}
