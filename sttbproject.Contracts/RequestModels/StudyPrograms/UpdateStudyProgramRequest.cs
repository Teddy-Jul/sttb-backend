using MediatR;
using sttbproject.Contracts.ResponseModels.StudyPrograms;

namespace sttbproject.Contracts.RequestModels.StudyPrograms;

public class UpdateStudyProgramRequest : IRequest<StudyProgramDetailResponse>
{
    public int ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? DegreeTitle { get; set; }
    public int? TotalCredits { get; set; }
    public string? StudyDuration { get; set; }
    public string? Description { get; set; }
}
