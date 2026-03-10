using FluentValidation;
using sttbproject.Contracts.RequestModels.Users;

namespace sttbproject.Commons.Validators.Users;

public class UpdateUserStatusRequestValidator : AbstractValidator<UpdateUserStatusRequest>
{
    public UpdateUserStatusRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Invalid user ID");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeValidStatus).WithMessage("Invalid status value");
    }

    private bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "active", "inactive", "suspended" };
        return validStatuses.Contains(status.ToLower());
    }
}
