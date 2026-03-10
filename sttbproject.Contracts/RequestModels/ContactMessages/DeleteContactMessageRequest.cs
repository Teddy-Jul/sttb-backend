using MediatR;

namespace sttbproject.Contracts.RequestModels.ContactMessages;

public class DeleteContactMessageRequest : IRequest<bool>
{
    public int ContactMessageId { get; set; }
}
