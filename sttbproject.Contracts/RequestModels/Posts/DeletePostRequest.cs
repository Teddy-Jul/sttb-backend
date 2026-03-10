using MediatR;

namespace sttbproject.Contracts.RequestModels.Posts;

public class DeletePostRequest : IRequest<bool>
{
    public int PostId { get; set; }
}
