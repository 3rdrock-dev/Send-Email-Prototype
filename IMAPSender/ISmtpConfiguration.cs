namespace IMAPSender;

/// <summary>
/// Defines SMTP server configuration settings.
/// </summary>
public interface ISmtpConfiguration
{
    string Host { get; }
    int Port { get; }
    string Username { get; }
    string Password { get; }
}
