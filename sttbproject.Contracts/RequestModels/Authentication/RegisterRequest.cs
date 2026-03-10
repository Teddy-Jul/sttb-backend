using MediatR;
using sttbproject.Contracts.ResponseModels.Authentication;

namespace sttbproject.Contracts.RequestModels.Authentication;

public class RegisterRequest : IRequest<RegisterResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
