using FluentValidation;
using sttbproject.Contracts.RequestModels.Courses;

namespace sttbproject.Commons.Validators.Courses;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseName)
            .NotEmpty().WithMessage("Course name is required")
            .MaximumLength(255).WithMessage("Course name must not exceed 255 characters");
    }
}