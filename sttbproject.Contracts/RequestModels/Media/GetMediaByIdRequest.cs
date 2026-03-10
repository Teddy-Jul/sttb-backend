using MediatR;
using sttbproject.Contracts.ResponseModels.Media;

namespace sttbproject.Contracts.RequestModels.Media;

public class GetMediaByIdRequest : IRequest<MediaDetailResponse>
{
    public int MediaId { get; set; }
}
