using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Menus;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Menus;

public class DeleteMenuRequestHandler : IRequestHandler<DeleteMenuRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteMenuRequestHandler> _logger;

    public DeleteMenuRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteMenuRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMenuRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting menu: {MenuId}", request.MenuId);

        var menu = await _context.Menus
            .Include(m => m.MenuItems)
            .FirstOrDefaultAsync(m => m.MenuId == request.MenuId, cancellationToken);

        if (menu == null)
        {
            _logger.LogWarning("Menu not found: {MenuId}", request.MenuId);
            throw new InvalidOperationException("Menu not found");
        }

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Menu deleted successfully: {MenuId}", request.MenuId);

        return true;
    }
}
