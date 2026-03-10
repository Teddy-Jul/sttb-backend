using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.Contracts.ResponseModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Users;

public class GetUserListRequestHandler : IRequestHandler<GetUserListRequest, GetUserListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetUserListRequestHandler> _logger;

    public GetUserListRequestHandler(
        SttbprojectContext context,
        ILogger<GetUserListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetUserListResponse> Handle(GetUserListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(u => 
                u.Name!.Contains(request.SearchTerm) || 
                u.Email.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(u => u.Status == request.Status);
        }

        if (request.RoleId.HasValue)
        {
            query = query.Where(u => u.RoleId == request.RoleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserListItem
            {
                UserId = u.UserId,
                Name = u.Name ?? string.Empty,
                Email = u.Email,
                RoleName = u.Role!.Name,
                Status = u.Status ?? string.Empty,
                CreatedAt = u.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} users (Page {Page})", users.Count, request.PageNumber);

        return new GetUserListResponse
        {
            Users = users,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
