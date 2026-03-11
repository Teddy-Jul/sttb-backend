using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class ProgramFee
{
    public int FeeId { get; set; }

    public int? ProgramId { get; set; }

    public int? FeeCategoryId { get; set; }

    public string? FeeName { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ProgramFeeCategory? FeeCategory { get; set; }

    public virtual StudyProgram? Program { get; set; }
}
