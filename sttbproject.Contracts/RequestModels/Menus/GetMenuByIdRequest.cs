using MediatR;
using sttbproject.Contracts.ResponseModels.Menus;

namespace sttbproject.Contracts.RequestModels.Menus;

public class GetMenuByIdRequest : IRequest<MenuDetailResponse>
{
    public int MenuId { get; set; }
}
