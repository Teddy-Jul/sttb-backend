using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Medium
{
    public int MediaId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual User? UploadedByNavigation { get; set; }
}
