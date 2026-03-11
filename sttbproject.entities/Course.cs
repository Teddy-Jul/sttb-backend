using System;
using System.Collections.Generic;

namespace sttbproject.entities;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseName { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CategoryCourse> CategoryCourses { get; set; } = new List<CategoryCourse>();
}
