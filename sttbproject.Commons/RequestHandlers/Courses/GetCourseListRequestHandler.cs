using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.Courses;
using sttbproject.Contracts.ResponseModels.Courses;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.Courses;

public class GetCourseListRequestHandler : IRequestHandler<GetCourseListRequest, GetCourseListResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetCourseListRequestHandler> _logger;

    public GetCourseListRequestHandler(
        SttbprojectContext context,
        ILogger<GetCourseListRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetCourseListResponse> Handle(GetCourseListRequest request, CancellationToken cancellationToken)
    {
        var query = _context.Courses.AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.CourseName!.Contains(request.SearchTerm) || 
                                     c.Description!.Contains(request.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Always order by CourseId ascending
        var courses = await query
            .OrderBy(c => c.CourseId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CourseListItem
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName ?? string.Empty,
                Description = c.Description,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} courses ordered by CourseId ascending", courses.Count);

        return new GetCourseListResponse
        {
            Courses = courses,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}