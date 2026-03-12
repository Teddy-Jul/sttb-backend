using MediatR;
using sttbproject.Contracts.ResponseModels.Posts;

namespace sttbproject.Contracts.RequestModels.Posts;

public class GetPostBySlugRequest : IRequest<PostDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
