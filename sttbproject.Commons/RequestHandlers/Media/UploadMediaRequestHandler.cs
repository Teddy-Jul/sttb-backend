using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Services;
using sttbproject.Contracts.RequestModels.Media;
using sttbproject.Contracts.ResponseModels.Media;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Media;

public class UploadMediaRequestHandler : IRequestHandler<UploadMediaRequest, UploadMediaResponse>
{
    private readonly SttbprojectContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<UploadMediaRequestHandler> _logger;

    public UploadMediaRequestHandler(
        SttbprojectContext context,
        IFileStorageService fileStorageService,
        ILogger<UploadMediaRequestHandler> logger)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<UploadMediaResponse> Handle(UploadMediaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading file: {FileName}", request.File.FileName);

        using var stream = request.File.OpenReadStream();
        var filePath = await _fileStorageService.UploadFileAsync(
            stream,
            request.File.FileName,
            request.File.ContentType,
            cancellationToken);

        var media = new Medium
        {
            FileName = request.File.FileName,
            FilePath = filePath,
            FileType = request.File.ContentType,
            FileSize = request.File.Length,
            UploadedBy = request.UploadedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.Media.Add(media);
        await _context.SaveChangesAsync(cancellationToken);

        var fileUrl = _fileStorageService.GetFileUrl(filePath);

        _logger.LogInformation("File uploaded successfully with ID: {MediaId}", media.MediaId);

        return new UploadMediaResponse
        {
            MediaId = media.MediaId,
            FileName = media.FileName ?? string.Empty,
            FilePath = filePath,
            FileUrl = fileUrl,
            Message = "File uploaded successfully"
        };
    }
}
