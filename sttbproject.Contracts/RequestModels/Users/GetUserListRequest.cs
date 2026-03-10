using MediatR;
using sttbproject.Contracts.ResponseModels.Users;

namespace sttbproject.Contracts.RequestModels.Users;

public class GetUserListRequest : IRequest<GetUserListResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? Status { get; set; }
    public int? RoleId { get; set; }
}
