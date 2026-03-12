using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class StudyProgram
{
    public int ProgramId { get; set; }

    public string ProgramName { get; set; } = null!;

    public string? DegreeLevel { get; set; }

    public string? DegreeTitle { get; set; }

    public int? TotalCredits { get; set; }

    public string? StudyDuration { get; set; }

    public string? Description { get; set; }

    public string? Slug { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProgramCourseCategory> ProgramCourseCategories { get; set; } = new List<ProgramCourseCategory>();

    public virtual ICollection<ProgramFee> ProgramFees { get; set; } = new List<ProgramFee>();
}
