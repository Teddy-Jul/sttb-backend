using MediatR;

namespace sttbproject.Contracts.RequestModels.Users;

public class CreateUserRequest : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string Status { get; set; } = "active";
}
