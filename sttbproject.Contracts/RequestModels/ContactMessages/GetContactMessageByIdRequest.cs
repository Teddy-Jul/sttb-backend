using MediatR;
using sttbproject.Contracts.ResponseModels.ContactMessages;

namespace sttbproject.Contracts.RequestModels.ContactMessages;

public class GetContactMessageByIdRequest : IRequest<ContactMessageDetailResponse>
{
    public int ContactMessageId { get; set; }
}
