using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class AuditLog
{
    public int AuditLogId { get; set; }

    public int? UserId { get; set; }

    public string? Action { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
