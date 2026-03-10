using MediatR;

namespace sttbproject.Contracts.RequestModels.Posts;

public class PublishPostRequest : IRequest<bool>
{
    public int PostId { get; set; }
}
