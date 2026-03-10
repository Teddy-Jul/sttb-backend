using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class UpdateUserStatusRequestHandler : IRequestHandler<UpdateUserStatusRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateUserStatusRequestHandler> _logger;

    public UpdateUserStatusRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateUserStatusRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user status: {UserId} to {Status}", request.UserId, request.Status);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new InvalidOperationException("User not found");
        }

        user.Status = request.Status;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User status updated successfully: {UserId}", request.UserId);

        return true;
    }
}
