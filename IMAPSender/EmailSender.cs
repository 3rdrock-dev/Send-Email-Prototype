using Microsoft.Extensions.Logging;

namespace IMAPSender;

/// <summary>
/// Simple facade for sending emails via SMTP.
/// Provides an easy-to-use API for consumers of the library.
/// </summary>
public class EmailSender
{
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of EmailSender with SMTP configuration.
    /// </summary>
    /// <param name="smtpHost">SMTP server host</param>
    /// <param name="smtpPort">SMTP server port (typically 465 for SSL/TLS or 587 for STARTTLS)</param>
    /// <param name="username">SMTP username</param>
    /// <param name="password">SMTP password</param>
    /// <param name="logger">Optional logger for diagnostics</param>
    public EmailSender(string smtpHost, int smtpPort, string username, string password, ILogger<SmtpEmailService>? logger = null)
    {
        var configuration = new SmtpConfiguration(smtpHost, smtpPort, username, password);
        _emailService = new SmtpEmailService(configuration, logger);
    }

    /// <summary>
    /// Initializes a new instance of EmailSender with a custom email service implementation.
    /// </summary>
    /// <param name="emailService">Custom email service implementation</param>
    public EmailSender(IEmailService emailService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <summary>
    /// Sends an email synchronously.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML (default: false)</param>
    public void Send(string from, string to, string subject, string body, bool isHtml = false)
    {
        SendAsync(from, to, subject, body, isHtml).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Sends an email synchronously with sender name.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="fromName">Sender display name</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML (default: false)</param>
    public void Send(string from, string? fromName, string to, string subject, string body, bool isHtml = false)
    {
        SendAsync(from, fromName, to, subject, body, isHtml).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML (default: false)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SendAsync(string from, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        await _emailService.SendAsync(from, to, subject, body, isHtml, cancellationToken);
    }

    /// <summary>
    /// Sends an email asynchronously with sender name.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="fromName">Sender display name</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML (default: false)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SendAsync(string from, string? fromName, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        await _emailService.SendAsync(from, fromName, to, subject, body, isHtml, cancellationToken);
    }

    /// <summary>
    /// Tests the SMTP connection and authentication without sending an email.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if connection and authentication succeed, false otherwise</returns>
    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            
            // Get configuration from the service
            if (_emailService is SmtpEmailService smtpService)
            {
                var config = smtpService.GetConfiguration();
                await client.ConnectAsync(config.Host, config.Port, MailKit.Security.SecureSocketOptions.SslOnConnect, cancellationToken);
                await client.AuthenticateAsync(config.Username, config.Password, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
}
