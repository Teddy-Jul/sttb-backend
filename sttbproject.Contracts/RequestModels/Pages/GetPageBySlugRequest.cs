using MediatR;
using sttbproject.Contracts.ResponseModels.Pages;

namespace sttbproject.Contracts.RequestModels.Pages;

public class GetPageBySlugRequest : IRequest<PageDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
