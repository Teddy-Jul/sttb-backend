using MediatR;
using sttbproject.Contracts.ResponseModels.ContactMessages;

namespace sttbproject.Contracts.RequestModels.ContactMessages;

public class GetContactMessageListRequest : IRequest<GetContactMessageListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
}
