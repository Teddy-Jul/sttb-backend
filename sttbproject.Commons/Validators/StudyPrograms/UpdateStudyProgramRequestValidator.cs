using FluentValidation;
using sttbproject.Contracts.RequestModels.StudyPrograms;

namespace sttbproject.Commons.Validators.StudyPrograms;

public class UpdateStudyProgramRequestValidator : AbstractValidator<UpdateStudyProgramRequest>
{
    public UpdateStudyProgramRequestValidator()
    {
        RuleFor(x => x.ProgramId)
            .GreaterThan(0).WithMessage("Invalid program ID");

        RuleFor(x => x.ProgramName)
            .NotEmpty().WithMessage("Program name is required")
            .MaximumLength(200).WithMessage("Program name must not exceed 200 characters");

        RuleFor(x => x.DegreeLevel)
            .MaximumLength(20).WithMessage("Degree level must not exceed 20 characters");

        RuleFor(x => x.DegreeTitle)
            .MaximumLength(50).WithMessage("Degree title must not exceed 50 characters");

        RuleFor(x => x.TotalCredits)
            .GreaterThan(0).When(x => x.TotalCredits.HasValue)
            .WithMessage("Total credits must be greater than 0");

        RuleFor(x => x.StudyDuration)
            .MaximumLength(100).WithMessage("Study duration must not exceed 100 characters");
    }
}