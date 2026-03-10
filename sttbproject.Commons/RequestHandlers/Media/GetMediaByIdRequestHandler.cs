using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Media;
using sttbproject.Contracts.ResponseModels.Media;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Media;

public class GetMediaByIdRequestHandler : IRequestHandler<GetMediaByIdRequest, MediaDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetMediaByIdRequestHandler> _logger;

    public GetMediaByIdRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<GetMediaByIdRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<MediaDetailResponse> Handle(GetMediaByIdRequest request, CancellationToken cancellationToken)
    {
        var media = await _context.Media
            .Include(m => m.UploadedByNavigation)
            .FirstOrDefaultAsync(m => m.MediaId == request.MediaId, cancellationToken);

        if (media == null)
        {
            _logger.LogWarning("Media not found: {MediaId}", request.MediaId);
            throw new InvalidOperationException("Media not found");
        }

        return new MediaDetailResponse
        {
            MediaId = media.MediaId,
            FileName = media.FileName ?? string.Empty,
            FilePath = media.FilePath ?? string.Empty,
            FileType = media.FileType ?? string.Empty,
            FileSize = media.FileSize ?? 0,
            FileUrl = _fileStorageService.GetFileUrl(media.FilePath ?? string.Empty),
            UploadedBy = media.UploadedBy ?? 0,
            UploadedByName = media.UploadedByNavigation?.Name ?? string.Empty,
            CreatedAt = media.CreatedAt ?? DateTime.MinValue
        };
    }
}
