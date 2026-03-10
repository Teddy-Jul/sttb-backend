using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.ContactMessages;
using sttbproject.Contracts.ResponseModels.ContactMessages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.ContactMessages;

public class GetContactMessageByIdRequestHandler : IRequestHandler<GetContactMessageByIdRequest, ContactMessageDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetContactMessageByIdRequestHandler> _logger;

    public GetContactMessageByIdRequestHandler(
        SttbprojectContext context,
        ILogger<GetContactMessageByIdRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ContactMessageDetailResponse> Handle(GetContactMessageByIdRequest request, CancellationToken cancellationToken)
    {
        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(c => c.ContactMessageId == request.ContactMessageId, cancellationToken);

        if (message == null)
        {
            _logger.LogWarning("Contact message not found: {ContactMessageId}", request.ContactMessageId);
            throw new InvalidOperationException("Contact message not found");
        }

        return new ContactMessageDetailResponse
        {
            ContactMessageId = message.ContactMessageId,
            Name = message.Name ?? string.Empty,
            Email = message.Email ?? string.Empty,
            Subject = message.Subject ?? string.Empty,
            MessageText = message.MessageText ?? string.Empty,
            Status = message.Status ?? string.Empty,
            CreatedAt = message.CreatedAt ?? DateTime.MinValue
        };
    }
}
