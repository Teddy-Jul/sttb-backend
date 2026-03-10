using MediatR;
using Microsoft.Extensions.Logging;
using sttbproject.Commons.Constants;
using sttbproject.Contracts.RequestModels.ContactMessages;
using sttbproject.Contracts.ResponseModels.ContactMessages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.ContactMessages;

public class CreateContactMessageRequestHandler : IRequestHandler<CreateContactMessageRequest, ContactMessageDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreateContactMessageRequestHandler> _logger;

    public CreateContactMessageRequestHandler(
        SttbprojectContext context,
        ILogger<CreateContactMessageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ContactMessageDetailResponse> Handle(CreateContactMessageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating contact message from: {Email}", request.Email);

        var contactMessage = new ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject,
            MessageText = request.MessageText,
            Status = MessageStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        _context.ContactMessages.Add(contactMessage);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact message created successfully with ID: {ContactMessageId}", contactMessage.ContactMessageId);

        return new ContactMessageDetailResponse
        {
            ContactMessageId = contactMessage.ContactMessageId,
            Name = contactMessage.Name ?? string.Empty,
            Email = contactMessage.Email ?? string.Empty,
            Subject = contactMessage.Subject ?? string.Empty,
            MessageText = contactMessage.MessageText ?? string.Empty,
            Status = contactMessage.Status ?? MessageStatus.New,
            CreatedAt = contactMessage.CreatedAt ?? DateTime.MinValue
        };
    }
}
