using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
