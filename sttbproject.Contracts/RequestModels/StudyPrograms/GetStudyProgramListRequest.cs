using MediatR;
using sttbproject.Contracts.ResponseModels.StudyPrograms;

namespace sttbproject.Contracts.RequestModels.StudyPrograms;

public class GetStudyProgramListRequest : IRequest<GetStudyProgramListResponse>
{
    public string? SearchTerm { get; set; }
    public string? DegreeLevel { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
