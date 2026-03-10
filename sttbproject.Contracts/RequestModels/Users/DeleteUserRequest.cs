using MediatR;

namespace sttbproject.Contracts.RequestModels.Users;

public class DeleteUserRequest : IRequest<bool>
{
    public int UserId { get; set; }
}
