using MediatR;
using sttbproject.Contracts.ResponseModels.Categories;

namespace sttbproject.Contracts.RequestModels.Categories;

public class GetCategoryByIdRequest : IRequest<CategoryDetailResponse>
{
    public int CategoryId { get; set; }
}
