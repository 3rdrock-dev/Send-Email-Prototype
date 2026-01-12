using Microsoft.Extensions.Configuration;

namespace SendEmailTest;

/// <summary>
/// Configuration helper for loading application settings
/// </summary>
public static class ConfigurationHelper
{
    private static IConfiguration? _configuration;

    /// <summary>
    /// Gets the application configuration instance
    /// </summary>
    public static IConfiguration Configuration
    {
        get
        {
            if (_configuration == null)
            {
                LoadConfiguration();
            }
            return _configuration!;
        }
    }

    /// <summary>
    /// Loads the configuration from appsettings.json
    /// </summary>
    private static void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();
    }

    /// <summary>
    /// Gets the application settings
    /// </summary>
    public static AppSettings GetAppSettings()
    {
        var appSettings = new AppSettings();
        Configuration.Bind(appSettings);
        return appSettings;
    }

    /// <summary>
    /// Gets the SMTP settings
    /// </summary>
    public static SmtpSettings GetSmtpSettings()
    {
        var settings = new SmtpSettings();
        Configuration.GetSection("SmtpSettings").Bind(settings);
        return settings;
    }

    /// <summary>
    /// Gets the email default settings
    /// </summary>
    public static EmailDefaults GetEmailDefaults()
    {
        var defaults = new EmailDefaults();
        Configuration.GetSection("EmailDefaults").Bind(defaults);
        return defaults;
    }
}
