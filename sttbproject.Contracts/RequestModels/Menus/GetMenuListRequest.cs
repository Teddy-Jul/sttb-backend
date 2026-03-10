using MediatR;
using sttbproject.Contracts.ResponseModels.Menus;

namespace sttbproject.Contracts.RequestModels.Menus;

public class GetMenuListRequest : IRequest<GetMenuListResponse>
{
    public string? SearchTerm { get; set; }
}
