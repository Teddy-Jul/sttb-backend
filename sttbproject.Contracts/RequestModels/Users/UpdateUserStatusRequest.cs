using MediatR;

namespace sttbproject.Contracts.RequestModels.Users;

public class UpdateUserStatusRequest : IRequest<bool>
{
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
}
