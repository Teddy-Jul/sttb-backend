using MediatR;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Courses;
using sttbproject.Contracts.ResponseModels.Courses;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Courses;

public class CreateCourseRequestHandler : IRequestHandler<CreateCourseRequest, CourseDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<CreateCourseRequestHandler> _logger;

    public CreateCourseRequestHandler(
        SttbprojectContext context,
        ILogger<CreateCourseRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseDetailResponse> Handle(CreateCourseRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating course: {CourseName}", request.CourseName);

        var course = new Course
        {
            CourseName = request.CourseName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course created successfully with ID: {CourseId}", course.CourseId);

        return new CourseDetailResponse
        {
            CourseId = course.CourseId,
            CourseName = course.CourseName ?? string.Empty,
            Description = course.Description,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }
}