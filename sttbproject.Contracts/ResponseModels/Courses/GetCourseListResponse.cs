using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sttbproject.Contracts.ResponseModels.Courses;

public class GetCourseListResponse
{
    public List<CourseListItem> Courses { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class CourseListItem
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}
