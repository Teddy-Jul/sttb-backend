using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class PublishPostRequestHandler : IRequestHandler<PublishPostRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<PublishPostRequestHandler> _logger;

    public PublishPostRequestHandler(
        SttbprojectContext context,
        ILogger<PublishPostRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(PublishPostRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing post: {PostId}", request.PostId);

        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Post not found: {PostId}", request.PostId);
            throw new InvalidOperationException("Post not found");
        }

        post.Status = ContentStatus.Published;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Post published successfully: {PostId}", request.PostId);

        return true;
    }
}
