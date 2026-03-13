using MediatR;

namespace sttbproject.Contracts.RequestModels.Events;

public class PublishEventRequest : IRequest<bool>
{
    public int EventId { get; set; }
}
