using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Menus;
using sttbproject.Contracts.ResponseModels.Menus;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Menus;

public class GetMenuListRequestHandler : IRequestHandler<GetMenuListRequest, GetMenuListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetMenuListRequestHandler> _logger;

    public GetMenuListRequestHandler(
        SttbprojectContext context,
        ILogger<GetMenuListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetMenuListResponse> Handle(GetMenuListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Menus
            .Include(m => m.MenuItems)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(m => m.Name!.Contains(request.SearchTerm));
        }

        var menus = await query
            .Select(m => new MenuListItem
            {
                MenuId = m.MenuId,
                Name = m.Name ?? string.Empty,
                ItemCount = m.MenuItems.Count
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} menus", menus.Count);

        return new GetMenuListResponse
        {
            Menus = menus
        };
    }
}
