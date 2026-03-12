using MediatR;
using sttbproject.Contracts.ResponseModels.StudyPrograms;

namespace sttbproject.Contracts.RequestModels.StudyPrograms;

public class GetStudyProgramBySlugRequest : IRequest<StudyProgramDetailResponse>
{
    public string Slug { get; set; } = string.Empty;
}
