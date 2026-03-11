using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Courses;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Courses;

public class DeleteCourseRequestHandler : IRequestHandler<DeleteCourseRequest, bool>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<DeleteCourseRequestHandler> _logger;

    public DeleteCourseRequestHandler(
        SttbprojectContext context,
        ILogger<DeleteCourseRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteCourseRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting course: {CourseId}", request.CourseId);

        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

        if (course == null)
        {
            _logger.LogWarning("Course not found: {CourseId}", request.CourseId);
            throw new InvalidOperationException("Course not found");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course deleted successfully: {CourseId}", request.CourseId);

        return true;
    }
}