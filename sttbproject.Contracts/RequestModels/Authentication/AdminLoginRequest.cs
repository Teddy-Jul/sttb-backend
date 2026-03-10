using MediatR;
using sttbproject.Contracts.ResponseModels.Authentication;

namespace sttbproject.Contracts.RequestModels.Authentication;

/// <summary>
/// Request model for admin user login (requires content_creator, editor, admin, or super_admin role)
/// </summary>
public class AdminLoginRequest : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
