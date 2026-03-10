using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Authentication;
using sttbproject.Contracts.ResponseModels.Authentication;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Authentication;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<LoginRequestHandler> _logger;

    public LoginRequestHandler(
        SttbprojectContext context,
        IPasswordHashService passwordHashService,
        ILogger<LoginRequestHandler> logger)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Login failed: User account is {Status} for email {Email}", user.Status, request.Email);
            throw new UnauthorizedAccessException($"Account is {user.Status}");
        }

        if (!_passwordHashService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var permissions = user.Role?.Permissions.Select(p => p.Name).ToList() ?? new List<string>();

        _logger.LogInformation("Login successful for user {UserId}", user.UserId);

        return new LoginResponse
        {
            UserId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email,
            RoleName = user.Role?.Name ?? string.Empty,
            Token = $"mock-token-{user.UserId}", // TODO: Implement JWT token generation
            Permissions = permissions
        };
    }
}
