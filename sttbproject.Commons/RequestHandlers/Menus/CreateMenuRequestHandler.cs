using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Menus;
using sttbproject.Contracts.ResponseModels.Menus;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Menus;

public class CreateMenuRequestHandler : IRequestHandler<CreateMenuRequest, MenuDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreateMenuRequestHandler> _logger;

    public CreateMenuRequestHandler(
        SttbprojectContext context,
        ILogger<CreateMenuRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MenuDetailResponse> Handle(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating menu: {Name}", request.Name);

        var menu = new Menu
        {
            Name = request.Name
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync(cancellationToken);

        // Separate top-level and child items
        var topLevelItems = request.Items.Where(i => i.ParentId == null).ToList();
        var childItems = request.Items.Where(i => i.ParentId != null).ToList();

        // Step 1: Create all top-level items first
        foreach (var item in topLevelItems)
        {
            var menuItem = new MenuItem
            {
                MenuId = menu.MenuId,
                Title = item.Title,
                Url = item.Url,
                ParentId = null,
                Position = item.Position
            };
            _context.MenuItems.Add(menuItem);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Step 2: Create child items (these reference existing menu_item_ids)
        if (childItems.Any())
        {
            // Validate that all ParentIds exist
            var parentIds = childItems.Select(i => i.ParentId!.Value).Distinct().ToList();
            var existingMenuItemIds = await _context.MenuItems
                .Where(mi => mi.MenuId == menu.MenuId && parentIds.Contains(mi.MenuItemId))
                .Select(mi => mi.MenuItemId)
                .ToListAsync(cancellationToken);

            var invalidParentIds = parentIds.Except(existingMenuItemIds).ToList();
            if (invalidParentIds.Any())
            {
                _logger.LogWarning("Invalid ParentIds found: {ParentIds}", string.Join(", ", invalidParentIds));
                throw new InvalidOperationException($"Invalid ParentIds: {string.Join(", ", invalidParentIds)}. Parent menu items must exist.");
            }

            foreach (var item in childItems)
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
        }

        var createdMenu = await _context.Menus
            .Include(m => m.MenuItems)
            .FirstAsync(m => m.MenuId == menu.MenuId, cancellationToken);

        _logger.LogInformation("Menu created successfully with ID: {MenuId}", menu.MenuId);

        return new MenuDetailResponse
        {
            MenuId = createdMenu.MenuId,
            Name = createdMenu.Name ?? string.Empty,
            Items = BuildMenuItemTree(createdMenu.MenuItems.ToList())
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
