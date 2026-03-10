using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Post
{
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? Content { get; set; }

    public string? Excerpt { get; set; }

    public int? FeaturedImageId { get; set; }

    public string? Status { get; set; }

    public int? AuthorId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Author { get; set; }

    public virtual Medium? FeaturedImage { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
