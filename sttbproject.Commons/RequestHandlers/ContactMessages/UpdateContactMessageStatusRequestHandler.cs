using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.ContactMessages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.ContactMessages;

public class UpdateContactMessageStatusRequestHandler : IRequestHandler<UpdateContactMessageStatusRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateContactMessageStatusRequestHandler> _logger;

    public UpdateContactMessageStatusRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateContactMessageStatusRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateContactMessageStatusRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact message status: {ContactMessageId} to {Status}", request.ContactMessageId, request.Status);

        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(c => c.ContactMessageId == request.ContactMessageId, cancellationToken);

        if (message == null)
        {
            _logger.LogWarning("Contact message not found: {ContactMessageId}", request.ContactMessageId);
            throw new InvalidOperationException("Contact message not found");
        }

        message.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact message status updated successfully: {ContactMessageId}", request.ContactMessageId);

        return true;
    }
}
