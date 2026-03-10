using MediatR;
using sttbproject.Contracts.ResponseModels.Pages;

namespace sttbproject.Contracts.RequestModels.Pages;

public class GetPageByIdRequest : IRequest<PageDetailResponse>
{
    public int PageId { get; set; }
}
