using Microsoft.Extensions.Logging;

namespace sttbproject.Commons.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending (SMTP, SendGrid, etc.)
        _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(IEnumerable<string> to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending (SMTP, SendGrid, etc.)
        _logger.LogInformation("Email sent to {Recipients} with subject {Subject}", string.Join(", ", to), subject);
        return Task.CompletedTask;
    }
}
