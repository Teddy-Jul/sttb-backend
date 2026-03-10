using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Menus;
using sttbproject.Contracts.ResponseModels.Menus;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Menus;

public class UpdateMenuRequestHandler : IRequestHandler<UpdateMenuRequest, MenuDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateMenuRequestHandler> _logger;

    public UpdateMenuRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateMenuRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MenuDetailResponse> Handle(UpdateMenuRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating menu: {MenuId}", request.MenuId);

        var menu = await _context.Menus
            .Include(m => m.MenuItems)
            .FirstOrDefaultAsync(m => m.MenuId == request.MenuId, cancellationToken);

        if (menu == null)
        {
            _logger.LogWarning("Menu not found: {MenuId}", request.MenuId);
            throw new InvalidOperationException("Menu not found");
        }

        menu.Name = request.Name;

        // Remove existing menu items
        _context.MenuItems.RemoveRange(menu.MenuItems);

        // Add new menu items
        foreach (var item in request.Items)
        {
            var menuItem = new MenuItem
            {
                MenuId = menu.MenuId,
                Title = item.Title,
                Url = item.Url,
                ParentId = item.ParentId,
                Position = item.Position
            };
            _context.MenuItems.Add(menuItem);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var updatedMenu = await _context.Menus
            .Include(m => m.MenuItems)
            .FirstAsync(m => m.MenuId == request.MenuId, cancellationToken);

        _logger.LogInformation("Menu updated successfully: {MenuId}", request.MenuId);

        return new MenuDetailResponse
        {
            MenuId = updatedMenu.MenuId,
            Name = updatedMenu.Name ?? string.Empty,
            Items = BuildMenuItemTree(updatedMenu.MenuItems.ToList())
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
