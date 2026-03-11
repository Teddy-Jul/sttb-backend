using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sttbproject.Contracts.ResponseModels.Courses;

public class CourseDetailResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
