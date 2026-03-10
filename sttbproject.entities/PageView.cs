using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class PageView
{
    public int PageViewId { get; set; }

    public int? PageId { get; set; }

    public string? VisitorIp { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? ViewedAt { get; set; }

    public virtual Page? Page { get; set; }
}
