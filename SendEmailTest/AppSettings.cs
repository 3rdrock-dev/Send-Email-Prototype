namespace SendEmailTest;

/// <summary>
/// SMTP settings loaded from appsettings.json
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Email default settings loaded from appsettings.json
/// </summary>
public class EmailDefaults
{
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

/// <summary>
/// Application settings container
/// </summary>
public class AppSettings
{
    public SmtpSettings SmtpSettings { get; set; } = new();
    public EmailDefaults EmailDefaults { get; set; } = new();
}
