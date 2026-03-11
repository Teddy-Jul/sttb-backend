using MediatR;
using sttbproject.Contracts.ResponseModels.Posts;

namespace sttbproject.Contracts.RequestModels.Posts;

public class GetPostByIdRequest : IRequest<PostDetailResponse>
{
    public int PostId { get; set; }
}
