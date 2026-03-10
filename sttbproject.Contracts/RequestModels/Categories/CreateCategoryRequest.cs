using MediatR;
using sttbproject.Contracts.ResponseModels.Categories;

namespace sttbproject.Contracts.RequestModels.Categories;

public class CreateCategoryRequest : IRequest<CategoryDetailResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ParentId { get; set; } = 0;
}
