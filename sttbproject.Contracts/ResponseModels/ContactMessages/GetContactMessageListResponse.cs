namespace sttbproject.Contracts.ResponseModels.ContactMessages;

public class GetContactMessageListResponse
{
    public List<ContactMessageListItem> Messages { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class ContactMessageListItem
{
    public int ContactMessageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
