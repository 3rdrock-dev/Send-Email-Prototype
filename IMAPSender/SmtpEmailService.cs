using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;

namespace IMAPSender;

/// <summary>
/// Email service implementation using MailKit SMTP client.
/// Follows Single Responsibility and Dependency Inversion principles.
/// </summary>
public class SmtpEmailService(ISmtpConfiguration configuration, ILogger<SmtpEmailService>? logger = null) : IEmailService
{
    private readonly ISmtpConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly ILogger<SmtpEmailService>? _logger = logger;

    public async Task SendAsync(string from, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        await SendAsync(from, null, to, subject, body, isHtml, cancellationToken);
    }

    public async Task SendAsync(string from, string? fromName, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(from);
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);

        _logger?.LogInformation("Preparing to send {ContentType} email from {Sender} to {Recipient}", 
            isHtml ? "HTML" : "plain text", from, to);

        var message = new EmailMessageBuilder()
            .From(from, fromName)
            .To(to)
            .WithSubject(subject)
            .WithBody(body, isHtml)
            .Build();

        await SendMessageAsync(message, cancellationToken);

        _logger?.LogInformation("Email sent successfully from {Sender} to {Recipient}", from, to);
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

    /// <summary>
    /// Gets the SMTP configuration for testing purposes.
    /// </summary>
    internal ISmtpConfiguration GetConfiguration() => _configuration;
}
