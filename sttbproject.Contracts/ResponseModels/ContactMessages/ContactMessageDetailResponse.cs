namespace sttbproject.Contracts.ResponseModels.ContactMessages;

public class ContactMessageDetailResponse
{
    public int ContactMessageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
