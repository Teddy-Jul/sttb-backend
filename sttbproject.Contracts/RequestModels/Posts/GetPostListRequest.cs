using MediatR;
using sttbproject.Contracts.ResponseModels.Posts;

namespace sttbproject.Contracts.RequestModels.Posts;

public class GetPostListRequest : IRequest<GetPostListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int? CategoryId { get; set; }
    public int? AuthorId { get; set; }
}
