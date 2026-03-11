using MediatR;
using sttbproject.Contracts.ResponseModels.StudyPrograms;

namespace sttbproject.Contracts.RequestModels.StudyPrograms;

public class GetStudyProgramByIdRequest : IRequest<StudyProgramDetailResponse>
{
    public int ProgramId { get; set; }
}
