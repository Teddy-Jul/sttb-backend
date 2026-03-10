using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Categories;
using sttbproject.entities;

namespace sttbproject.Commons.Validators.Categories;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    private readonly SttbprojectContext _context;

    public UpdateCategoryRequestValidator(SttbprojectContext context)
    {
        _context = context;

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Invalid category ID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters")
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug must contain only lowercase letters, numbers, and hyphens")
            .MustAsync(BeUniqueSlug).WithMessage("Slug is already in use");
    }

    private async Task<bool> BeUniqueSlug(UpdateCategoryRequest request, string slug, CancellationToken cancellationToken)
    {
        return !await _context.Categories.AnyAsync(c => c.Slug == slug && c.CategoryId != request.CategoryId, cancellationToken);
    }
}
