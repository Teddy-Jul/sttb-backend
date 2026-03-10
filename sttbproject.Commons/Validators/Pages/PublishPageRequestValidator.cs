using FluentValidation;
using sttbproject.Contracts.RequestModels.Pages;

namespace sttbproject.Commons.Validators.Pages;

public class PublishPageRequestValidator : AbstractValidator<PublishPageRequest>
{
    public PublishPageRequestValidator()
    {
        RuleFor(x => x.PageId)
            .GreaterThan(0).WithMessage("Invalid page ID");
    }
}
