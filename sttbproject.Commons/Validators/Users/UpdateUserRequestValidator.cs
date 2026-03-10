using FluentValidation;
using Microsoft.EntityFrameworkCore;
using sttbproject.Contracts.RequestModels.Users;
using sttbproject.entities;

namespace sttbproject.Commons.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    private readonly SttbprojectContext _context;

    public UpdateUserRequestValidator(SttbprojectContext context)
    {
        _context = context;

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Invalid user ID");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("Role is required")
            .MustAsync(RoleExists).WithMessage("Invalid role selected");
    }

    private async Task<bool> BeUniqueEmail(UpdateUserRequest request, string email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email && u.UserId != request.UserId, cancellationToken);
    }

    private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
    {
        return await _context.Roles.AnyAsync(r => r.RoleId == roleId, cancellationToken);
    }
}
