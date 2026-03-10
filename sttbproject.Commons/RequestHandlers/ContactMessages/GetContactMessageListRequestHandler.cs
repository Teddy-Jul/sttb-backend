using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.ContactMessages;
using sttbproject.Contracts.ResponseModels.ContactMessages;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.ContactMessages;

public class GetContactMessageListRequestHandler : IRequestHandler<GetContactMessageListRequest, GetContactMessageListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetContactMessageListRequestHandler> _logger;

    public GetContactMessageListRequestHandler(
        SttbprojectContext context,
        ILogger<GetContactMessageListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetContactMessageListResponse> Handle(GetContactMessageListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.ContactMessages.AsQueryable();

        // Apply search filter first
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => 
                c.Name!.Contains(request.SearchTerm) || 
                c.Email!.Contains(request.SearchTerm) ||
                c.Subject!.Contains(request.SearchTerm));
        }

        // Apply status filter second
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(c => c.Status == request.Status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering and pagination
        var messages = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ContactMessageListItem
            {
                ContactMessageId = c.ContactMessageId,
                Name = c.Name ?? string.Empty,
                Email = c.Email ?? string.Empty,
                Subject = c.Subject ?? string.Empty,
                Status = c.Status ?? string.Empty,
                CreatedAt = c.CreatedAt ?? DateTime.MinValue
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} contact messages (Page {Page})", messages.Count, request.PageNumber);

        return new GetContactMessageListResponse
        {
            Messages = messages,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
