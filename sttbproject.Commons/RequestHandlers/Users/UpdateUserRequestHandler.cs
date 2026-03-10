using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateUserRequestHandler> _logger;

    public UpdateUserRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateUserRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user: {UserId}", request.UserId);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new InvalidOperationException("User not found");
        }

        user.Name = request.Name;
        user.Email = request.Email;
        user.RoleId = request.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated successfully: {UserId}", request.UserId);

        return true;
    }
}
