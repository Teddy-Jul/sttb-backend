using FluentValidation;
using sttbproject.Contracts.RequestModels.Courses;

namespace sttbproject.Commons.Validators.Courses;

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Invalid course ID");

        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(255).WithMessage("Course name must not exceed 255 characters");
    }
}