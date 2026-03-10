using FluentValidation;
using sttbproject.Contracts.RequestModels.Menus;

namespace sttbproject.Commons.Validators.Menus;

public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required")
            .MaximumLength(100).WithMessage("Menu name must not exceed 100 characters");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Menu item title is required")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters");

            item.RuleFor(x => x.Url)
                .NotEmpty().WithMessage("Menu item URL is required")
                .MaximumLength(255).WithMessage("URL must not exceed 255 characters");

            item.RuleFor(x => x.Position)
                .GreaterThanOrEqualTo(0).WithMessage("Position must be a non-negative number");
        });
    }
}
