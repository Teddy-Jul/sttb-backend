using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Media;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Media;

public class DeleteMediaRequestHandler : IRequestHandler<DeleteMediaRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<DeleteMediaRequestHandler> _logger;

    public DeleteMediaRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<DeleteMediaRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMediaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting media: {MediaId}", request.MediaId);

        var media = await _context.Media
            .FirstOrDefaultAsync(m => m.MediaId == request.MediaId, cancellationToken);

        if (media == null)
        {
            _logger.LogWarning("Media not found: {MediaId}", request.MediaId);
            throw new InvalidOperationException("Media not found");
        }

        // Check if media is used as featured image in any posts
        var postsUsingMedia = await _context.Posts
            .Where(p => p.FeaturedImageId == request.MediaId)
            .ToListAsync(cancellationToken);

        if (postsUsingMedia.Any())
        {
            _logger.LogInformation("Removing media reference from {Count} posts", postsUsingMedia.Count);
            
            // Set featured_image_id to NULL for all posts using this media
            foreach (var post in postsUsingMedia)
            {
                post.FeaturedImageId = null;
            }
            
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Delete physical file
        if (!string.IsNullOrEmpty(media.FilePath))
        {
            await _fileStorageService.DeleteFileAsync(media.FilePath, cancellationToken);
        }

        // Now safe to delete media record
        _context.Media.Remove(media);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Media deleted successfully: {MediaId}", request.MediaId);

        return true;
    }
}
