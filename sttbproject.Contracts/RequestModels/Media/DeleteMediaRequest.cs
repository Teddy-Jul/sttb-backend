using MediatR;

namespace sttbproject.Contracts.RequestModels.Media;

public class DeleteMediaRequest : IRequest<bool>
{
    public int MediaId { get; set; }
}
