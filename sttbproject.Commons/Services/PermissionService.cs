using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.entities;

namespace sttbproject.Commons.Services;

public class PermissionService : IPermissionService
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        SttbprojectContext context,
        ILogger<PermissionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user?.Role == null)
        {
            _logger.LogWarning("User {UserId} or role not found", userId);
            return false;
        }

        return user.Role.Permissions.Any(p => p.Name == permission);
    }

    public async Task<bool> UserHasAnyPermissionAsync(int userId, string[] permissions, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user?.Role == null)
        {
            return false;
        }

        return user.Role.Permissions.Any(p => permissions.Contains(p.Name));
    }

    public async Task<bool> UserHasAllPermissionsAsync(int userId, string[] permissions, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user?.Role == null)
        {
            return false;
        }

        var userPermissions = user.Role.Permissions.Select(p => p.Name).ToList();
        return permissions.All(p => userPermissions.Contains(p));
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r!.Permissions)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user?.Role == null)
        {
            return new List<string>();
        }

        return user.Role.Permissions.Select(p => p.Name).ToList();
    }
}