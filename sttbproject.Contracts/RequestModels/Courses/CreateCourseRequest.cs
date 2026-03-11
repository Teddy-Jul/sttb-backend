using MediatR;
using sttbproject.Contracts.ResponseModels.Courses;

namespace sttbproject.Contracts.RequestModels.Courses;

public class CreateCourseRequest : IRequest<CourseDetailResponse>
{
    public string CourseName { get; set; } = string.Empty;
    public string? Description { get; set; }
}
