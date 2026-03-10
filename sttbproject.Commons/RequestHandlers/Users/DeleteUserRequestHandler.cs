using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteUserRequestHandler> _logger;

    public DeleteUserRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteUserRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user: {UserId}", request.UserId);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new InvalidOperationException("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User deleted successfully: {UserId}", request.UserId);

        return true;
    }
}
