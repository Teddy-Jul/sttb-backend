using MediatR;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.Contracts.ResponseModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class GetPostBySlugRequestHandler : IRequestHandler<GetPostBySlugRequest, PostDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetPostBySlugRequestHandler> _logger;

    public GetPostBySlugRequestHandler(
        SttbprojectContext context,
        ILogger<GetPostBySlugRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PostDetailResponse> Handle(GetPostBySlugRequest request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .Include(p => p.FeaturedImage)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);

        if (post == null)
        {
            _logger.LogWarning("Post not found with Slug: {Slug}", request.Slug);
            throw new InvalidOperationException("Post not found");
        }

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
            FeaturedImageUrl = post.FeaturedImage?.FileName,
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
