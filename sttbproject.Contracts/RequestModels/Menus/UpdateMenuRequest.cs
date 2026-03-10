using MediatR;
using sttbproject.Contracts.ResponseModels.Menus;

namespace sttbproject.Contracts.RequestModels.Menus;

public class UpdateMenuRequest : IRequest<MenuDetailResponse>
{
    public int MenuId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<MenuItemDto> Items { get; set; } = new();
}
