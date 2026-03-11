using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sttbproject.Contracts.ResponseModels.StudyPrograms;

public class GetStudyProgramListResponse
{
    public List<StudyProgramListItem> Programs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class StudyProgramListItem
{
    public int ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? DegreeTitle { get; set; }
    public int? TotalCredits { get; set; }
    public string? StudyDuration { get; set; }
    public DateTime? CreatedAt { get; set; }
}
