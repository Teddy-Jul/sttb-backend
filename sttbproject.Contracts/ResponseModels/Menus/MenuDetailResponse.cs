namespace sttbproject.Contracts.ResponseModels.Menus;

public class MenuDetailResponse
{
    public int MenuId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<MenuItemResponse> Items { get; set; } = new();
}

public class MenuItemResponse
{
    public int MenuItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public int Position { get; set; }
    public List<MenuItemResponse> Children { get; set; } = new();
}
