using MediatR;
using sttbproject.Contracts.ResponseModels.Pages;

namespace sttbproject.Contracts.RequestModels.Pages;

public class UpdatePageRequest : IRequest<PageDetailResponse>
{
    public int PageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UpdatedBy { get; set; }
    public string Status { get; set; } = "draft";
}
