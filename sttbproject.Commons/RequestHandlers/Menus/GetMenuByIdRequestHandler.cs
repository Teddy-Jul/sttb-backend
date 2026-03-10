using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Menus;
using sttbproject.Contracts.ResponseModels.Menus;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Menus;

public class GetMenuByIdRequestHandler : IRequestHandler<GetMenuByIdRequest, MenuDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetMenuByIdRequestHandler> _logger;

    public GetMenuByIdRequestHandler(
        SttbprojectContext context,
        ILogger<GetMenuByIdRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MenuDetailResponse> Handle(GetMenuByIdRequest request, CancellationToken cancellationToken)
    {
        var menu = await _context.Menus
            .Include(m => m.MenuItems)
            .FirstOrDefaultAsync(m => m.MenuId == request.MenuId, cancellationToken);

        if (menu == null)
        {
            _logger.LogWarning("Menu not found: {MenuId}", request.MenuId);
            throw new InvalidOperationException("Menu not found");
        }

        return new MenuDetailResponse
        {
            MenuId = menu.MenuId,
            Name = menu.Name ?? string.Empty,
            Items = BuildMenuItemTree(menu.MenuItems.ToList())
        };
    }

    private List<MenuItemResponse> BuildMenuItemTree(List<MenuItem> items)
    {
        var topLevel = items.Where(i => i.ParentId == null)
            .OrderBy(i => i.Position)
            .Select(i => new MenuItemResponse
            {
                MenuItemId = i.MenuItemId,
                Title = i.Title ?? string.Empty,
                Url = i.Url ?? string.Empty,
                ParentId = i.ParentId,
                Position = i.Position ?? 0,
                Children = BuildChildren(i.MenuItemId, items)
            })
            .ToList();

        return topLevel;
    }

    private List<MenuItemResponse> BuildChildren(int parentId, List<MenuItem> items)
    {
        return items.Where(i => i.ParentId == parentId)
            .OrderBy(i => i.Position)
            .Select(i => new MenuItemResponse
            {
                MenuItemId = i.MenuItemId,
                Title = i.Title ?? string.Empty,
                Url = i.Url ?? string.Empty,
                ParentId = i.ParentId,
                Position = i.Position ?? 0,
                Children = BuildChildren(i.MenuItemId, items)
            })
            .ToList();
    }
}
