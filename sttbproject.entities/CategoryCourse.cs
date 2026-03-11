using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class CategoryCourse
{
    public int CategoryCourseId { get; set; }

    public int? ProgramCategoryId { get; set; }

    public int? CourseId { get; set; }

    public int? Credits { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ProgramCourseCategory? ProgramCategory { get; set; }
}
