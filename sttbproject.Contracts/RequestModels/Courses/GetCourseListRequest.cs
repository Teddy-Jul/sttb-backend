using MediatR;
using sttbproject.Contracts.ResponseModels.Courses;

namespace sttbproject.Contracts.RequestModels.Courses;

public class GetCourseListRequest : IRequest<GetCourseListResponse>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
