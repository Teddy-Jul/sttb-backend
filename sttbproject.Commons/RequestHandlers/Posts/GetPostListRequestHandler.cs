using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Posts;
using sttbproject.Contracts.ResponseModels.Posts;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Posts;

public class GetPostListRequestHandler : IRequestHandler<GetPostListRequest, GetPostListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetPostListRequestHandler> _logger;

    public GetPostListRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetPostListRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<GetPostListResponse> Handle(GetPostListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Categories)
            .Include(p => p.FeaturedImage)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p => p.Title!.Contains(request.SearchTerm) || p.Content!.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(p => p.Status == request.Status);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.Categories.Any(c => c.CategoryId == request.CategoryId.Value));
        }

        if (request.AuthorId.HasValue)
        {
            query = query.Where(p => p.AuthorId == request.AuthorId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PostListItem
            {
                PostId = p.PostId,
                Title = p.Title ?? string.Empty,
                Slug = p.Slug ?? string.Empty,
                Excerpt = p.Excerpt,
                Status = p.Status ?? string.Empty,
                AuthorName = p.Author!.Name ?? string.Empty,
                Categories = p.Categories.Select(c => c.Name ?? string.Empty).ToList(),
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt ?? DateTime.MinValue,
                FeaturedImageId = p.FeaturedImageId,
                FeaturedImageUrl = p.FeaturedImage != null
                    ? _fileStorageService.GetFileUrl(p.FeaturedImage.FilePath ?? string.Empty)
                    : null
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} posts (Page {Page})", posts.Count, request.PageNumber);

        return new GetPostListResponse
        {
            Posts = posts,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
