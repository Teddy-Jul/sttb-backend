using FluentValidation;
using Microsoft.AspNetCore.Http;
using sttbproject.Contracts.RequestModels.Media;
using System.IO;

namespace sttbproject.Commons.Validators.Media;

public class UploadMediaRequestValidator : AbstractValidator<UploadMediaRequest>
{
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

    public UploadMediaRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty")
            .Must(file => file.Length <= MaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024} MB")
            .Must(file => {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return AllowedExtensions.Contains(extension);
            }).WithMessage($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.UploadedBy)
            .GreaterThan(0).WithMessage("Invalid uploader ID");
    }
}
