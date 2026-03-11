namespace sttbproject.Commons.Services;

public interface IPermissionService
{
    Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default);
    Task<bool> UserHasAnyPermissionAsync(int userId, string[] permissions, CancellationToken cancellationToken = default);
    Task<bool> UserHasAllPermissionsAsync(int userId, string[] permissions, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
}