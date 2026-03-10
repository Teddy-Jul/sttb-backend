using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class SystemLog
{
    public int SystemLogId { get; set; }

    public string? Level { get; set; }

    public string? LogMessage { get; set; }

    public string? Context { get; set; }

    public DateTime? CreatedAt { get; set; }
}
