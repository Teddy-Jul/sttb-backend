using MediatR;

namespace sttbproject.Contracts.RequestModels.ContactMessages;

public class UpdateContactMessageStatusRequest : IRequest<bool>
{
    public int ContactMessageId { get; set; }
    public string Status { get; set; } = string.Empty;
}
