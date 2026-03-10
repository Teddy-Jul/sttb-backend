using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Media;
using sttbproject.Contracts.ResponseModels.Media;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Media;

public class GetMediaListRequestHandler : IRequestHandler<GetMediaListRequest, GetMediaListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetMediaListRequestHandler> _logger;

    public GetMediaListRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetMediaListRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<GetMediaListResponse> Handle(GetMediaListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Media
            .Include(m => m.UploadedByNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(m => m.FileName!.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.FileType))
        {
            query = query.Where(m => m.FileType == request.FileType);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var media = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MediaListItem
            {
                MediaId = m.MediaId,
                FileName = m.FileName ?? string.Empty,
                FileType = m.FileType ?? string.Empty,
                FileSize = m.FileSize ?? 0,
                FileUrl = _fileStorageService.GetFileUrl(m.FilePath ?? string.Empty),
                UploadedByName = m.UploadedByNavigation!.Name ?? string.Empty,
                CreatedAt = m.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} media files (Page {Page})", media.Count, request.PageNumber);

        return new GetMediaListResponse
        {
            Media = media,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
