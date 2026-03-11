using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class ProgramFeeCategory
{
    public int FeeCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProgramFee> ProgramFees { get; set; } = new List<ProgramFee>();
}
