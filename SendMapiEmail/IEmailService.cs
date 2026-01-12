namespace SendIMAPEmail;

/// <summary>
/// Defines email sending operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
