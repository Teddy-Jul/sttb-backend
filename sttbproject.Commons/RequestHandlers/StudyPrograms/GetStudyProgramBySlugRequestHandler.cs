using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sttbproject.Contracts.RequestModels.StudyPrograms;
using sttbproject.Contracts.ResponseModels.StudyPrograms;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.StudyPrograms;

public class GetStudyProgramBySlugRequestHandler : IRequestHandler<GetStudyProgramBySlugRequest, StudyProgramDetailResponse>
{
    private readonly SttbprojectContext _context;
    private readonly ILogger<GetStudyProgramBySlugRequestHandler> _logger;

    public GetStudyProgramBySlugRequestHandler(
        SttbprojectContext context,
        ILogger<GetStudyProgramBySlugRequestHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<StudyProgramDetailResponse> Handle(GetStudyProgramBySlugRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting study program by Slug: {Slug}", request.Slug);

        var program = await _context.StudyPrograms
            .Include(p => p.ProgramCourseCategories)
                .ThenInclude(pcc => pcc.Category)
            .Include(p => p.ProgramCourseCategories)
                .ThenInclude(pcc => pcc.CategoryCourses)
                    .ThenInclude(cc => cc.Course)
            .Include(p => p.ProgramFees)
                .ThenInclude(pf => pf.FeeCategory)
            .FirstOrDefaultAsync(p => p.Slug == request.Slug, cancellationToken);

        if (program == null)
        {
            _logger.LogWarning("Study program not found: {Slug}", request.Slug);
            throw new InvalidOperationException("Study program not found");
        }

        return new StudyProgramDetailResponse
        {
            ProgramId = program.ProgramId,
            ProgramName = program.ProgramName,
            DegreeLevel = program.DegreeLevel,
            DegreeTitle = program.DegreeTitle,
            TotalCredits = program.TotalCredits,
            StudyDuration = program.StudyDuration,
            Description = program.Description,
            Slug = program.Slug,
            CreatedAt = program.CreatedAt,
            UpdatedAt = program.UpdatedAt,
            CourseCategories = program.ProgramCourseCategories.Select(pcc => new ProgramCourseCategoryInfo
            {
                ProgramCategoryId = pcc.ProgramCategoryId,
                CategoryId = pcc.CategoryId ?? 0,
                CategoryName = pcc.Category?.CategoryName ?? string.Empty,
                TotalCredits = pcc.TotalCredits,
                Courses = pcc.CategoryCourses.Select(cc => new CategoryCourseInfo
                {
                    CategoryCourseId = cc.CategoryCourseId,
                    CourseId = cc.CourseId ?? 0,
                    CourseName = cc.Course?.CourseName ?? string.Empty,
                    Credits = cc.Credits
                }).ToList()
            }).ToList(),
            Fees = program.ProgramFees.Select(pf => new ProgramFeeInfo
            {
                FeeId = pf.FeeId,
                FeeCategoryId = pf.FeeCategoryId,
                FeeCategoryName = pf.FeeCategory?.CategoryName,
                FeeName = pf.FeeName,
                Amount = pf.Amount
            }).ToList()
        };
    }
}
