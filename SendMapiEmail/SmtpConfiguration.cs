namespace SendIMAPEmail;

/// <summary>
/// SMTP server configuration with validation.
/// Uses C# 14 primary constructor.
/// </summary>
/// <param name="host">SMTP server host</param>
/// <param name="port">SMTP server port</param>
/// <param name="username">SMTP username</param>
/// <param name="password">SMTP password</param>
public class SmtpConfiguration(string host, int port, string username, string password) : ISmtpConfiguration
{
    public string Host { get; } = !string.IsNullOrWhiteSpace(host) 
        ? host 
        : throw new ArgumentException("Host cannot be null or empty.", nameof(host));
    
    public int Port { get; } = port is > 0 and <= 65535 
        ? port 
        : throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");
    
    public string Username { get; } = !string.IsNullOrWhiteSpace(username) 
        ? username 
        : throw new ArgumentException("Username cannot be null or empty.", nameof(username));
    
    public string Password { get; } = password ?? throw new ArgumentNullException(nameof(password));
}
