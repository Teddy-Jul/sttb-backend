using MediatR;
using sttbproject.Contracts.ResponseModels.Menus;

namespace sttbproject.Contracts.RequestModels.Menus;

public class CreateMenuRequest : IRequest<MenuDetailResponse>
{
    public string Name { get; set; } = string.Empty;
    public List<MenuItemDto> Items { get; set; } = new();
}

public class MenuItemDto
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int Position { get; set; }
}
