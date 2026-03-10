using MediatR;
using sttbproject.Contracts.ResponseModels.Users;

namespace sttbproject.Contracts.RequestModels.Users;

public class GetUserByIdRequest : IRequest<UserDetailResponse>
{
    public int UserId { get; set; }
}
