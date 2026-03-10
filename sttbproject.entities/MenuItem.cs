using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class MenuItem
{
    public int MenuItemId { get; set; }

    public int MenuId { get; set; }

    public string? Title { get; set; }

    public string? Url { get; set; }

    public int? ParentId { get; set; }

    public int? Position { get; set; }

    public virtual ICollection<MenuItem> InverseParent { get; set; } = new List<MenuItem>();

    public virtual Menu Menu { get; set; } = null!;

    public virtual MenuItem? Parent { get; set; }
}
