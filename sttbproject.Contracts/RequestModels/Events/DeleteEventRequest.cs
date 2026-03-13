using MediatR;

namespace sttbproject.Contracts.RequestModels.Events;

public class DeleteEventRequest : IRequest<bool>
{
    public int EventId { get; set; }
}
