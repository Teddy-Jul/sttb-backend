using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class SearchIndex
{
    public int SearchIndexId { get; set; }

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
