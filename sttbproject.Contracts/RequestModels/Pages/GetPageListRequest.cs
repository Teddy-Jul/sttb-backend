using MediatR;
using sttbproject.Contracts.ResponseModels.Pages;

namespace sttbproject.Contracts.RequestModels.Pages;

public class GetPageListRequest : IRequest<GetPageListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
}
