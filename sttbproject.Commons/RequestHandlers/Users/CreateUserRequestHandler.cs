using MediatR;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, int>
{
    private readonly SttbprojectContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<CreateUserRequestHandler> _logger;

    public CreateUserRequestHandler(
        SttbprojectContext context,
        IPasswordHashService passwordHashService,
        ILogger<CreateUserRequestHandler> logger)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating user with email: {Email}", request.Email);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHashService.HashPassword(request.Password),
            RoleId = request.RoleId,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created successfully with ID: {UserId}", user.UserId);

        return user.UserId;
    }
}
