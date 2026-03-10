using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class DeletePageRequestHandler : IRequestHandler<DeletePageRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeletePageRequestHandler> _logger;

    public DeletePageRequestHandler(
        SttbprojectContext context,
        ILogger<DeletePageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeletePageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting page: {PageId}", request.PageId);

        var page = await _context.Pages
            .FirstOrDefaultAsync(p => p.PageId == request.PageId, cancellationToken);

        if (page == null)
        {
            _logger.LogWarning("Page not found: {PageId}", request.PageId);
            throw new InvalidOperationException("Page not found");
        }

        _context.Pages.Remove(page);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Page deleted successfully: {PageId}", request.PageId);

        return true;
    }
}
