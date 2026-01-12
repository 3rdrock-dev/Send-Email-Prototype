namespace IMAPSender;

/// <summary>
/// Defines email sending operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendAsync(string from, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email asynchronously with sender name.
    /// </summary>
    /// <param name="from">Sender email address</param>
    /// <param name="fromName">Sender display name</param>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="isHtml">Whether the body content is HTML</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendAsync(string from, string? fromName, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
}
