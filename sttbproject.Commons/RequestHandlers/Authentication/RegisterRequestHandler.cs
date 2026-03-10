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

        // Get 'user' role for regular users
        var userRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "user", cancellationToken);

        if (userRole == null)
        {
            _logger.LogError("User role not found in database");
            throw new InvalidOperationException("System configuration error: User role not found");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHashService.HashPassword(request.Password),
            RoleId = userRole.RoleId,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Regular user registered successfully with ID: {UserId}", user.UserId);

        return new RegisterResponse
        {
            UserId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email,
            Message = "Registration successful. Welcome!"
        };
    }
}

