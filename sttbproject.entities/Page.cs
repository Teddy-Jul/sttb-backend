using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Page
{
    public int PageId { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<PageView> PageViews { get; set; } = new List<PageView>();

    public virtual User? UpdatedByNavigation { get; set; }
}
