using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Authentication;
using sttbproject.Contracts.ResponseModels.Authentication;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Authentication;

/// <summary>
/// Admin login handler - validates admin roles (content_creator, editor, admin, super_admin)
/// </summary>
public class AdminLoginRequestHandler : IRequestHandler<AdminLoginRequest, LoginResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<AdminLoginRequestHandler> _logger;

    private static readonly HashSet<string> AdminRoles = new()
    {
        "super_admin",
        "admin",
        "editor",
        "content_creator",
        "marketing"
    };

    public AdminLoginRequestHandler(
        SttbprojectContext context,
        IPasswordHashService passwordHashService,
        ILogger<AdminLoginRequestHandler> logger)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(AdminLoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin login attempt for email: {Email}", request.Email);

        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Admin login failed: User not found for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if user has an admin role
        if (user.Role == null || !AdminRoles.Contains(user.Role.Name))
        {
            _logger.LogWarning("Admin login failed: User {Email} does not have admin privileges", request.Email);
            throw new UnauthorizedAccessException("Access denied. Admin privileges required.");
        }

        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Admin login failed: User account is {Status} for email {Email}", user.Status, request.Email);
            throw new UnauthorizedAccessException($"Account is {user.Status}");
        }

        if (!_passwordHashService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Admin login failed: Invalid password for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var permissions = user.Role?.Permissions.Select(p => p.Name).ToList() ?? new List<string>();

        _logger.LogInformation("Admin login successful for user {UserId} with role {Role}", user.UserId, user.Role?.Name);

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
