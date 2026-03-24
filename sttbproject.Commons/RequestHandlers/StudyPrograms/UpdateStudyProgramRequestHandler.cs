using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.Contracts.ResponseModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class UpdateStudyProgramRequestHandler : IRequestHandler<UpdateStudyProgramRequest, StudyProgramDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<UpdateStudyProgramRequestHandler> _logger;

    public UpdateStudyProgramRequestHandler(
        SttbprojectContext context,
        ILogger<UpdateStudyProgramRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StudyProgramDetailResponse> Handle(UpdateStudyProgramRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating study program: {ProgramId}", request.ProgramId);

        var program = await _context.StudyPrograms
            .Include(p => p.ProgramCourseCategories)
                .ThenInclude(pcc => pcc.CategoryCourses)
            .FirstOrDefaultAsync(p => p.ProgramId == request.ProgramId, cancellationToken);

        if (program == null)
        {
            _logger.LogWarning("Study program not found: {ProgramId}", request.ProgramId);
            throw new InvalidOperationException("Study program not found");
        }

        program.ProgramName = request.ProgramName;
        program.DegreeLevel = request.DegreeLevel;
        program.DegreeTitle = request.DegreeTitle;
        program.TotalCredits = request.TotalCredits;
        program.StudyDuration = request.StudyDuration;
        program.Description = request.Description;
        program.UpdatedAt = DateTime.UtcNow;

        if (program.ProgramCourseCategories.Any())
        {
            var existingCategoryCourses = program.ProgramCourseCategories
                .SelectMany(pcc => pcc.CategoryCourses)
                .ToList();

            if (existingCategoryCourses.Any())
            {
                _context.CategoryCourses.RemoveRange(existingCategoryCourses);
            }

            _context.ProgramCourseCategories.RemoveRange(program.ProgramCourseCategories);
        }

        var normalizedSelections = request.CourseSelections
            .Where(x => x.CourseId > 0 && x.CategoryId > 0)
            .GroupBy(x => x.CourseId)
            .Select(g => g.First())
            .ToList();

        if (normalizedSelections.Count == 0)
        {
            var selectedCourseIds = request.CourseIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (selectedCourseIds.Count > 0)
            {
                var fallbackCategoryId = await _context.CourseCategories
                    .OrderBy(c => c.CategoryId)
                    .Select(c => c.CategoryId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (fallbackCategoryId <= 0)
                {
                    throw new InvalidOperationException("No course category available for selected courses");
                }

                normalizedSelections = selectedCourseIds
                    .Select(id => new StudyProgramCourseSelectionItem { CourseId = id, CategoryId = fallbackCategoryId })
                    .ToList();
            }
        }

        if (normalizedSelections.Count > 0)
        {
            var selectedCourseIds = normalizedSelections.Select(x => x.CourseId).Distinct().ToList();
            var selectedCategoryIds = normalizedSelections.Select(x => x.CategoryId).Distinct().ToList();

            var existingCourses = await _context.Courses
                .Where(c => selectedCourseIds.Contains(c.CourseId))
                .Select(c => new { c.CourseId, c.Credits })
                .ToListAsync(cancellationToken);

            if (existingCourses.Count != selectedCourseIds.Count)
            {
                throw new InvalidOperationException("One or more selected courses were not found");
            }

            var existingCategoryIds = await _context.CourseCategories
                .Where(c => selectedCategoryIds.Contains(c.CategoryId))
                .Select(c => c.CategoryId)
                .ToListAsync(cancellationToken);

            if (existingCategoryIds.Count != selectedCategoryIds.Count)
            {
                throw new InvalidOperationException("One or more selected course categories were not found");
            }

            var courseCreditsMap = existingCourses.ToDictionary(c => c.CourseId, c => c.Credits ?? 0);

            var groupedByCategory = normalizedSelections
                .GroupBy(x => x.CategoryId)
                .ToList();

            foreach (var categoryGroup in groupedByCategory)
            {
                var programCategory = new ProgramCourseCategory
                {
                    ProgramId = program.ProgramId,
                    CategoryId = categoryGroup.Key,
                    TotalCredits = categoryGroup.Sum(x => courseCreditsMap.GetValueOrDefault(x.CourseId, 0)),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                _context.ProgramCourseCategories.Add(programCategory);
                await _context.SaveChangesAsync(cancellationToken);

                var categoryCourses = categoryGroup.Select(selection => new CategoryCourse
                {
                    ProgramCategoryId = programCategory.ProgramCategoryId,
                    CourseId = selection.CourseId,
                    Credits = courseCreditsMap.GetValueOrDefault(selection.CourseId, 0),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                });

                _context.CategoryCourses.AddRange(categoryCourses);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Study program updated successfully: {ProgramId}", program.ProgramId);

        return new StudyProgramDetailResponse
        {
            ProgramId = program.ProgramId,
            ProgramName = program.ProgramName,
            DegreeLevel = program.DegreeLevel,
            DegreeTitle = program.DegreeTitle,
            TotalCredits = program.TotalCredits,
            StudyDuration = program.StudyDuration,
            Description = program.Description,
            CreatedAt = program.CreatedAt,
            UpdatedAt = program.UpdatedAt
        };
    }
}
