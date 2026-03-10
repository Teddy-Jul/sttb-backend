using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Pages;
using sttbproject.entities;

namespace sttbproject.Commons.Validators.Pages;

public class CreatePageRequestValidator : AbstractValidator<CreatePageRequest>
{
    private readonly SttbprojectContext _context;

    public CreatePageRequestValidator(SttbprojectContext context)
    {
        _context = context;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MaximumLength(200).WithMessage("Slug must not exceed 200 characters")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueSlug).WithMessage("Slug is already in use");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0).WithMessage("Invalid creator ID");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeValidStatus).WithMessage("Invalid status value");
    }

    private async Task<bool> BeUniqueSlug(string slug, CancellationToken cancellationToken)
    {
        return !await _context.Pages.AnyAsync(p => p.Slug == slug, cancellationToken);
    }

    private bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "draft", "published", "archived" };
        return validStatuses.Contains(status.ToLower());
    }
}
