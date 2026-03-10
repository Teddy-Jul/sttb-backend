using MediatR;
using sttbproject.Contracts.ResponseModels.ContactMessages;

namespace sttbproject.Contracts.RequestModels.ContactMessages;

public class CreateContactMessageRequest : IRequest<ContactMessageDetailResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
}
