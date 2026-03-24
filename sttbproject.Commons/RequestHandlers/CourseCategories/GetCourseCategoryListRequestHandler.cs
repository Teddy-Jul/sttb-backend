using MediatR;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.CourseCategories;
using sttbproject.Contracts.ResponseModels.CourseCategories;
using sttbproject.entities;

namespace sttbproject.Commons.RequestHandlers.CourseCategories;

public class GetCourseCategoryListRequestHandler : IRequestHandler<GetCourseCategoryListRequest, GetCourseCategoryListResponse>
{
    private readonly SttbprojectContext _context;

    public GetCourseCategoryListRequestHandler(SttbprojectContext context)
    {
        _context = context;
    }

    public async Task<GetCourseCategoryListResponse> Handle(GetCourseCategoryListRequest request, CancellationToken cancellationToken)
    {
        var categories = await _context.CourseCategories
            .OrderBy(c => c.CategoryName)
            .Select(c => new CourseCategoryListItem
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName ?? string.Empty,
            })
            .ToListAsync(cancellationToken);

        return new GetCourseCategoryListResponse
        {
            Categories = categories,
        };
    }
}
