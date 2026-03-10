using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.Contracts.ResponseModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class GetUserByIdRequestHandler : IRequestHandler<GetUserByIdRequest, UserDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetUserByIdRequestHandler> _logger;

    public GetUserByIdRequestHandler(
        SttbprojectContext context,
        ILogger<GetUserByIdRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserDetailResponse> Handle(GetUserByIdRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new InvalidOperationException("User not found");
        }

        var permissions = user.Role?.Permissions.Select(p => p.Name).ToList() ?? new List<string>();

        return new UserDetailResponse
        {
            UserId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email,
            RoleId = user.RoleId ?? 0,
            RoleName = user.Role?.Name ?? string.Empty,
            Status = user.Status ?? string.Empty,
            CreatedAt = user.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = user.UpdatedAt,
            Permissions = permissions
        };
    }
}
