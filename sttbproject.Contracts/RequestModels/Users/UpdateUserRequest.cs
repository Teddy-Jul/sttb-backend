using MediatR;

namespace sttbproject.Contracts.RequestModels.Users;

public class UpdateUserRequest : IRequest<bool>
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
}
