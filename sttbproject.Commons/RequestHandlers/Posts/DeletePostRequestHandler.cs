using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class DeletePostRequestHandler : IRequestHandler<DeletePostRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeletePostRequestHandler> _logger;

    public DeletePostRequestHandler(
        SttbprojectContext context,
        ILogger<DeletePostRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeletePostRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting post: {PostId}", request.PostId);

        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Post not found: {PostId}", request.PostId);
            throw new InvalidOperationException("Post not found");
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Post deleted successfully: {PostId}", request.PostId);

        return true;
    }
}
