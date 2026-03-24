using MediatR;
using sttbproject.Contracts.ResponseModels.CourseCategories;

namespace sttbproject.Contracts.RequestModels.CourseCategories;

public class GetCourseCategoryListRequest : IRequest<GetCourseCategoryListResponse>
{
}
