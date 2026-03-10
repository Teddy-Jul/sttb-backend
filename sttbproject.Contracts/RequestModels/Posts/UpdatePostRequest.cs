using MediatR;
using sttbproject.Contracts.ResponseModels.Posts;

namespace sttbproject.Contracts.RequestModels.Posts;

public class UpdatePostRequest : IRequest<PostDetailResponse>
{
    public int PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? FeaturedImageId { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public string Status { get; set; } = "draft";
}
