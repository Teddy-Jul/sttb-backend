using MediatR;
using sttbproject.Contracts.ResponseModels.Categories;

namespace sttbproject.Contracts.RequestModels.Categories;

public class GetCategoryListRequest : IRequest<GetCategoryListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
