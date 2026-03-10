using MediatR;
using sttbproject.Contracts.ResponseModels.Posts;

namespace sttbproject.Contracts.RequestModels.Posts;

public class CreatePostRequest : IRequest<PostDetailResponse>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public int? FeaturedImageId { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public string Status { get; set; } = "draft";
}
