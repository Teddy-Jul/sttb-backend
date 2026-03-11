using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class ProgramCourseCategory
{
    public int ProgramCategoryId { get; set; }

    public int? ProgramId { get; set; }

    public int? CategoryId { get; set; }

    public int? TotalCredits { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual CourseCategory? Category { get; set; }

    public virtual ICollection<CategoryCourse> CategoryCourses { get; set; } = new List<CategoryCourse>();

    public virtual StudyProgram? Program { get; set; }
}
