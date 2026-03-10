using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.ContactMessages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.ContactMessages;

public class DeleteContactMessageRequestHandler : IRequestHandler<DeleteContactMessageRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteContactMessageRequestHandler> _logger;

    public DeleteContactMessageRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteContactMessageRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteContactMessageRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting contact message: {ContactMessageId}", request.ContactMessageId);

        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(c => c.ContactMessageId == request.ContactMessageId, cancellationToken);

        if (message == null)
        {
            _logger.LogWarning("Contact message not found: {ContactMessageId}", request.ContactMessageId);
            throw new InvalidOperationException("Contact message not found");
        }

        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact message deleted successfully: {ContactMessageId}", request.ContactMessageId);

        return true;
    }
}
