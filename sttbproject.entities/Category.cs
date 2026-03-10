using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
