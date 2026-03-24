namespace sttbproject.Contracts.ResponseModels.CourseCategories;

public class GetCourseCategoryListResponse
{
    public List<CourseCategoryListItem> Categories { get; set; } = new();
}

public class CourseCategoryListItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
