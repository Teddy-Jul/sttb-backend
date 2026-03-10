using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Authentication;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Authentication;

public class ChangePasswordRequestHandler : IRequestHandler<ChangePasswordRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<ChangePasswordRequestHandler> _logger;

    public ChangePasswordRequestHandler(
        SttbprojectContext context,
        IPasswordHashService passwordHashService,
        ILogger<ChangePasswordRequestHandler> logger)
    {
        _context = context;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task<bool> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Change password attempt for user: {UserId}", request.UserId);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new InvalidOperationException("User not found");
        }

        if (!_passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            _logger.LogWarning("Invalid current password for user: {UserId}", request.UserId);
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        user.PasswordHash = _passwordHashService.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);

        return true;
    }
}
