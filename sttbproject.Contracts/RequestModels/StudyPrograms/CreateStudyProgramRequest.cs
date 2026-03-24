using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using sttbproject.Contracts.ResponseModels.StudyPrograms;

namespace sttbproject.Contracts.RequestModels.StudyPrograms;

public class CreateStudyProgramRequest : IRequest<StudyProgramDetailResponse>
{
    public string ProgramName { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? DegreeTitle { get; set; }
    public int? TotalCredits { get; set; }
    public string? StudyDuration { get; set; }
    public string? Description { get; set; }
    public List<int> CourseIds { get; set; } = new();
    public List<StudyProgramCourseSelectionItem> CourseSelections { get; set; } = new();
}

public class StudyProgramCourseSelectionItem
{
    public int CourseId { get; set; }
    public int CategoryId { get; set; }
}
