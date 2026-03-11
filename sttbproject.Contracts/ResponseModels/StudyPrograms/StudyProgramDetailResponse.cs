namespace sttbproject.Contracts.ResponseModels.StudyPrograms;

public class StudyProgramDetailResponse
{
    public int ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string? DegreeLevel { get; set; }
    public string? DegreeTitle { get; set; }
    public int? TotalCredits { get; set; }
    public string? StudyDuration { get; set; }
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ProgramCourseCategoryInfo> CourseCategories { get; set; } = new();
    public List<ProgramFeeInfo> Fees { get; set; } = new();
}

public class ProgramCourseCategoryInfo
{
    public int ProgramCategoryId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? TotalCredits { get; set; }
    public List<CategoryCourseInfo> Courses { get; set; } = new();
}

public class CategoryCourseInfo
{
    public int CategoryCourseId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int? Credits { get; set; }
}

public class ProgramFeeInfo
{
    public int FeeId { get; set; }
    public int? FeeCategoryId { get; set; }
    public string? FeeCategoryName { get; set; }
    public string? FeeName { get; set; }
    public decimal? Amount { get; set; }
}
