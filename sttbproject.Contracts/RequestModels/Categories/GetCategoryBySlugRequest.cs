using MediatR;

namespace sttbproject.Contracts.RequestModels.Categories;

public class GetCategoryBySlugRequest : IRequest<CategoryDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
