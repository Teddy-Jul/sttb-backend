using MediatR;
using sttbproject.Contracts.ResponseModels.Courses;

namespace sttbproject.Contracts.RequestModels.Courses;

public class UpdateCourseRequest : IRequest<CourseDetailResponse>
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
