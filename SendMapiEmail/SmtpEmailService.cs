using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;

namespace SendIMAPEmail;

/// <summary>
/// Email service implementation using MailKit SMTP client.
/// Follows Single Responsibility and Dependency Inversion principles.
/// </summary>
public class SmtpEmailService(ISmtpConfiguration configuration, ILogger<SmtpEmailService>? logger = null) : IEmailService
{
    private readonly ISmtpConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly ILogger<SmtpEmailService>? _logger = logger;

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        _logger?.LogInformation("Preparing to send email to {Recipient}", to);

        var message = new EmailMessageBuilder()
            .From(_configuration.Username, "nvädi Support")
            .To(to)
            .WithSubject(subject)
            .WithBody(body)
            .Build();

        await SendMessageAsync(message, cancellationToken);

        _logger?.LogInformation("Email sent successfully to {Recipient}", to);
    }

    private async Task SendMessageAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            _logger?.LogDebug("Connecting to {Host}:{Port} with SSL/TLS", _configuration.Host, _configuration.Port);
            await client.ConnectAsync(_configuration.Host, _configuration.Port, SecureSocketOptions.SslOnConnect, cancellationToken);

            _logger?.LogDebug("Authenticating with username: {Username}", _configuration.Username);
            await client.AuthenticateAsync(_configuration.Username, _configuration.Password, cancellationToken);

            _logger?.LogDebug("Sending message");
            await client.SendAsync(message, cancellationToken);

            _logger?.LogDebug("Disconnecting from server");
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to send email to {Recipient}", message.To);
            throw;
        }
    }
}
