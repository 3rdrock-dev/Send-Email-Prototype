using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text.Json;

namespace SendEmailWinUITest;

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
    /// Gets the application settings with secure password retrieval
    /// </summary>
    public static AppSettings GetAppSettings()
    {
        var appSettings = new AppSettings();
        Configuration.Bind(appSettings);

        // Try to get password from secure storage first
        var securePassword = SecureCredentialService.GetPassword(appSettings.SmtpSettings.Username);
        if (!string.IsNullOrEmpty(securePassword))
        {
            appSettings.SmtpSettings.Password = securePassword;
        }
        // If no secure password and plain text password exists in config, migrate it
        else if (!string.IsNullOrEmpty(appSettings.SmtpSettings.Password))
        {
            // Migrate plain text password to secure storage
            SecureCredentialService.SaveCredential(
                appSettings.SmtpSettings.Username,
                appSettings.SmtpSettings.Password);
        }

        return appSettings;
    }

    /// <summary>
    /// Saves application settings to appsettings.json (excluding password)
    /// </summary>
    public static bool SaveAppSettings(AppSettings settings)
    {
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            // Create a copy without the password for saving to JSON
            var settingsToSave = new
            {
                SmtpSettings = new
                {
                    settings.SmtpSettings.Host,
                    settings.SmtpSettings.Port,
                    settings.SmtpSettings.Username,
                    Password = "" // Never save password to JSON
                },
                EmailDefaults = new
                {
                    settings.EmailDefaults.FromAddress,
                    settings.EmailDefaults.FromName
                }
            };

            // Serialize with pretty formatting
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string jsonString = JsonSerializer.Serialize(settingsToSave, jsonOptions);
            File.WriteAllText(filePath, jsonString);

            // Reload configuration to reflect changes
            _configuration = null;
            LoadConfiguration();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the SMTP settings with secure password retrieval
    /// </summary>
    public static SmtpSettings GetSmtpSettings()
    {
        var settings = new SmtpSettings();
        Configuration.GetSection("SmtpSettings").Bind(settings);

        // Try to get password from secure storage
        var securePassword = SecureCredentialService.GetPassword(settings.Username);
        if (!string.IsNullOrEmpty(securePassword))
        {
            settings.Password = securePassword;
        }

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
