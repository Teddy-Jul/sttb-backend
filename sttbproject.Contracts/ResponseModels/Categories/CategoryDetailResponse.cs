namespace sttbproject.Contracts.ResponseModels.Categories;

public class CategoryDetailResponse
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int PostCount { get; set; }
}
