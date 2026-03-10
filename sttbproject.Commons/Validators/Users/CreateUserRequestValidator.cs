using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly SttbprojectContext _context;

    public CreateUserRequestValidator(SttbprojectContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role is required")
            .MustAsync(RoleExists).WithMessage("Invalid role selected");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(BeValidStatus).WithMessage("Invalid status value");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
    }

    private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
    {
        return await _context.Roles.AnyAsync(r => r.RoleId == roleId, cancellationToken);
    }

    private bool BeValidStatus(string status)
    {
        var validStatuses = new[] { "active", "inactive", "suspended" };
        return validStatuses.Contains(status.ToLower());
    }
}
