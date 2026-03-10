using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class ContactMessage
{
    public int ContactMessageId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Subject { get; set; }

    public string? MessageText { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }
}
