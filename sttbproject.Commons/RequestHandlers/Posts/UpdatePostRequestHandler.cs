using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.Contracts.ResponseModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class UpdatePostRequestHandler : IRequestHandler<UpdatePostRequest, PostDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UpdatePostRequestHandler> _logger;

    public UpdatePostRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<UpdatePostRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<PostDetailResponse> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating post: {PostId}", request.PostId);

        var post = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.FeaturedImage)
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.PostId == request.PostId, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Post not found: {PostId}", request.PostId);
            throw new InvalidOperationException("Post not found");
        }

        post.Title = request.Title;
        post.Slug = request.Slug;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt;
        post.FeaturedImageId = request.FeaturedImageId;
        post.Status = request.Status;
        post.UpdatedAt = DateTime.UtcNow;

        // Update categories
        post.Categories.Clear();
        if (request.CategoryIds.Any())
        {
            var categories = await _context.Categories
                .Where(c => request.CategoryIds.Contains(c.CategoryId))
                .ToListAsync(cancellationToken);

            foreach (var category in categories)
            {
                post.Categories.Add(category);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Post updated successfully: {PostId}", request.PostId);

        return new PostDetailResponse
        {
            PostId = post.PostId,
            Title = post.Title ?? string.Empty,
            Slug = post.Slug ?? string.Empty,
            Content = post.Content ?? string.Empty,
            Excerpt = post.Excerpt,
            Status = post.Status ?? string.Empty,
            AuthorId = post.AuthorId ?? 0,
            AuthorName = post.Author?.Name ?? string.Empty,
            FeaturedImageId = post.FeaturedImageId,
            FeaturedImageUrl = post.FeaturedImage != null
                ? _fileStorageService.GetFileUrl(post.FeaturedImage.FilePath ?? string.Empty)
                : null,
            Categories = post.Categories.Select(c => new CategoryInfo
            {
                CategoryId = c.CategoryId,
                Name = c.Name ?? string.Empty,
                Slug = c.Slug ?? string.Empty
            }).ToList(),
            PublishedAt = post.PublishedAt,
            CreatedAt = post.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = post.UpdatedAt
        };
    }
}
