using MediatR;

namespace sttbproject.Contracts.RequestModels.Pages;

public class DeletePageRequest : IRequest<bool>
{
    public int PageId { get; set; }
}
