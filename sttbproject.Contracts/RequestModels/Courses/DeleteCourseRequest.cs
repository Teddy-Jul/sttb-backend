using MediatR;

namespace sttbproject.Contracts.RequestModels.Courses;

public class DeleteCourseRequest : IRequest<bool>
{
    public int CourseId { get; set; }
}
