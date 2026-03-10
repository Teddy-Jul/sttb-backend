using MediatR;
using sttbproject.Contracts.ResponseModels.Media;

namespace sttbproject.Contracts.RequestModels.Media;

public class GetMediaListRequest : IRequest<GetMediaListResponse>
{
    public string? SearchTerm { get; set; }
    public string? FileType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
