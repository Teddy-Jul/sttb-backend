using MediatR;
using sttbproject.Contracts.ResponseModels.Authentication;

namespace sttbproject.Contracts.RequestModels.Authentication;

public class LoginRequest : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
