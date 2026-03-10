using MediatR;
using sttbproject.Contracts.ResponseModels.Categories;

namespace sttbproject.Contracts.RequestModels.Categories;

public class UpdateCategoryRequest : IRequest<CategoryDetailResponse>
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
