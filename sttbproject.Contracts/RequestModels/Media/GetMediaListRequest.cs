using MediatR;
using sttbproject.Contracts.ResponseModels.Media;

namespace sttbproject.Contracts.RequestModels.Media;

public class GetMediaListRequest : IRequest<GetMediaListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? FileType { get; set; }
}
