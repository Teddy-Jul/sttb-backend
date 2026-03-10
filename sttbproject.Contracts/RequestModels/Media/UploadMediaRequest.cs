using MediatR;
using Microsoft.AspNetCore.Http;
using sttbproject.Contracts.ResponseModels.Media;

namespace sttbproject.Contracts.RequestModels.Media;

public class UploadMediaRequest : IRequest<UploadMediaResponse>
{
    public IFormFile File { get; set; } = null!;
    public int UploadedBy { get; set; }
}
