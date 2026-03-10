using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Pages;

public class PublishPageRequestHandler : IRequestHandler<PublishPageRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<PublishPageRequestHandler> _logger;

    public PublishPageRequestHandler(
        SttbprojectContext context,
        ILogger<PublishPageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(PublishPageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing page: {PageId}", request.PageId);

        var page = await _context.Pages
            .FirstOrDefaultAsync(p => p.PageId == request.PageId, cancellationToken);

        if (page == null)
        {
            _logger.LogWarning("Page not found: {PageId}", request.PageId);
            throw new InvalidOperationException("Page not found");
        }

        page.Status = ContentStatus.Published;
        page.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Page published successfully: {PageId}", request.PageId);

        return true;
    }
}
