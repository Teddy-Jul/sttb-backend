using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Courses;
using sttbproject.Contracts.ResponseModels.Courses;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Courses;

public class UpdateCourseRequestHandler : IRequestHandler<UpdateCourseRequest, CourseDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateCourseRequestHandler> _logger;

    public UpdateCourseRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateCourseRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CourseDetailResponse> Handle(UpdateCourseRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating course: {CourseId}", request.CourseId);

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

        if (course == null)
        {
            _logger.LogWarning("Course not found: {CourseId}", request.CourseId);
            throw new InvalidOperationException("Course not found");
        }

        course.CourseName = request.CourseName;
        course.Description = request.Description;
        course.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course updated successfully: {CourseId}", course.CourseId);

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