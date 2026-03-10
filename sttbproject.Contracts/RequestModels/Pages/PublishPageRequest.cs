using MediatR;

namespace sttbproject.Contracts.RequestModels.Pages;

public class PublishPageRequest : IRequest<bool>
{
    public int PageId { get; set; }
}
