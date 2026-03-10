using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.Contracts.ResponseModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, PostDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreatePostRequestHandler> _logger;

    public CreatePostRequestHandler(
        SttbprojectContext context,
        ILogger<CreatePostRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostDetailResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating post: {Title}", request.Title);

        var authorExists = await _context.Users.AnyAsync(u => u.UserId == request.AuthorId, cancellationToken);
        if (!authorExists)
        {
            throw new InvalidOperationException($"Author with ID {request.AuthorId} not found");
        }

        var post = new Post
        {
            Title = request.Title,
            Slug = request.Slug,
            Content = request.Content,
            Excerpt = request.Excerpt,
            AuthorId = request.AuthorId,
            FeaturedImageId = request.FeaturedImageId,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        if (request.CategoryIds.Any())
        {
            var categories = await _context.Categories
                .Where(c => request.CategoryIds.Contains(c.CategoryId))
                .ToListAsync(cancellationToken);

            post.Categories = categories;
            await _context.SaveChangesAsync(cancellationToken);
        }

        var createdPost = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.FeaturedImage)
            .Include(p => p.Categories)
            .FirstAsync(p => p.PostId == post.PostId, cancellationToken);

        _logger.LogInformation("Post created successfully with ID: {PostId}", post.PostId);

        return new PostDetailResponse
        {
            PostId = createdPost.PostId,
            Title = createdPost.Title ?? string.Empty,
            Slug = createdPost.Slug ?? string.Empty,
            Content = createdPost.Content ?? string.Empty,
            Excerpt = createdPost.Excerpt,
            Status = createdPost.Status ?? ContentStatus.Draft,
            AuthorId = createdPost.AuthorId ?? 0,
            AuthorName = createdPost.Author?.Name ?? string.Empty,
            FeaturedImageId = createdPost.FeaturedImageId,
            FeaturedImageUrl = createdPost.FeaturedImage?.FilePath,
            Categories = createdPost.Categories.Select(c => new CategoryInfo
            {
                CategoryId = c.CategoryId,
                Name = c.Name ?? string.Empty,
                Slug = c.Slug ?? string.Empty
            }).ToList(),
            PublishedAt = createdPost.PublishedAt,
            CreatedAt = createdPost.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = createdPost.UpdatedAt
        };
    }
}
