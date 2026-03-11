/// <summary>
/// Request to get a category by its slug.
/// </summary>
using MediatR;
using sttbproject.Contracts.ResponseModels.Categories;

namespace sttbproject.Contracts.RequestModels.Categories;

public class GetCategoryBySlugRequest : IRequest<CategoryDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
