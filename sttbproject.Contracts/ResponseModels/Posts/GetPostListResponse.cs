namespace sttbproject.Contracts.ResponseModels.Posts;

public class GetPostListResponse
{
    public List<PostListItem> Posts { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class PostListItem
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? FeaturedImageId { get; set; }
    public string? FeaturedImageUrl { get; set; }
}
