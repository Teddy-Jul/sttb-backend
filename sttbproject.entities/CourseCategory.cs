using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class CourseCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ProgramCourseCategory> ProgramCourseCategories { get; set; } = new List<ProgramCourseCategory>();
}
