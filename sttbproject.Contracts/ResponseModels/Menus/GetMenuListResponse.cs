namespace sttbproject.Contracts.ResponseModels.Menus;

public class GetMenuListResponse
{
    public List<MenuListItem> Menus { get; set; } = new();
}

public class MenuListItem
{
    public int MenuId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
