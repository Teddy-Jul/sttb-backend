using MediatR;

namespace sttbproject.Contracts.RequestModels.Categories;

public class DeleteCategoryRequest : IRequest<bool>
{
    public int CategoryId { get; set; }
}
