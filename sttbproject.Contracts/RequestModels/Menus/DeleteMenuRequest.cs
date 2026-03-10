using MediatR;

namespace sttbproject.Contracts.RequestModels.Menus;

public class DeleteMenuRequest : IRequest<bool>
{
    public int MenuId { get; set; }
}
