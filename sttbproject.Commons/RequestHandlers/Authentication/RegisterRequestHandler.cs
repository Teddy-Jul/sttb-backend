using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Authentication;
using sttbproject.Contracts.ResponseModels.Authentication;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Authentication;

public class RegisterRequestHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<RegisterRequestHandler> _logger;

    public RegisterRequestHandler(
        SttbprojectContext context,
        IPasswordHashService passwordHashService,
        ILogger<RegisterRequestHandler> logger)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User registration attempt for email: {Email}", request.Email);

        // Get default content_creator role (or editor if content_creator not found)
        var defaultRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "content_creator" || r.Name == "editor", cancellationToken);

        if (defaultRole == null)
        {
            _logger.LogError("Default role not found in database");
            throw new InvalidOperationException("System configuration error: Default role not found");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHashService.HashPassword(request.Password),
            RoleId = defaultRole.RoleId,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User registered successfully with ID: {UserId}", user.UserId);

        return new RegisterResponse
        {
            UserId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email,
            Message = "Registration successful"
        };
    }
}

