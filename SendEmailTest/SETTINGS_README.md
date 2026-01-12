# SendEmailTest - Application Settings Guide

This WinForms application uses `appsettings.json` to store IMAPSender configuration.

## Configuration File

The `appsettings.json` file contains SMTP server settings and email defaults:

```json
{
  "SmtpSettings": {
    "Host": "mail5018.site4now.net",
    "Port": 465,
    "Username": "support@nvadi.com",
    "Password": ""
  },
  "EmailDefaults": {
    "FromAddress": "support@nvadi.com",
    "FromName": "nvädi Support"
  }
}
```

## Configuration Structure

### SmtpSettings
- **Host**: SMTP server hostname
- **Port**: SMTP server port (465 for SSL/TLS, 587 for STARTTLS)
- **Username**: SMTP authentication username
- **Password**: SMTP authentication password (store securely in production!)

### EmailDefaults
- **FromAddress**: Default sender email address
- **FromName**: Default sender display name

## Usage in Code

### Load All Settings

```csharp
var appSettings = ConfigurationHelper.GetAppSettings();
```

### Load SMTP Settings Only

```csharp
var smtpSettings = ConfigurationHelper.GetSmtpSettings();
```

### Load Email Defaults Only

```csharp
var emailDefaults = ConfigurationHelper.GetEmailDefaults();
```

### Use with IMAPSender Library

```csharp
// Load settings
var appSettings = ConfigurationHelper.GetAppSettings();

// Create email sender
var sender = new IMAPSender.EmailSender(
    appSettings.SmtpSettings.Host,
    appSettings.SmtpSettings.Port,
    appSettings.SmtpSettings.Username,
    appSettings.SmtpSettings.Password
);

// Send email
await sender.SendAsync(
    from: appSettings.EmailDefaults.FromAddress,
    to: "recipient@example.com",
    subject: "Test Email",
    body: "Email body content",
    isHtml: false
);
```

## Security Considerations

?? **Important Security Notes:**

1. **Never commit passwords** to source control
2. Leave the `Password` field empty in the committed `appsettings.json`
3. For production, consider using:
   - User Secrets (for development)
   - Azure Key Vault
   - Environment variables
   - Windows Credential Manager
   - Encrypted configuration files

### Using User Secrets (Development)

For development, you can use .NET User Secrets:

```bash
# Initialize user secrets
dotnet user-secrets init --project SendEmailTest

# Set password
dotnet user-secrets set "SmtpSettings:Password" "your-password" --project SendEmailTest
```

Then update `ConfigurationHelper.cs` to include user secrets:

```csharp
var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<MainForm>() // Add this line
    .AddEnvironmentVariables();
```

## File Location

The `appsettings.json` file is automatically copied to the output directory when building the project. It will be located in the same folder as the executable:

```
bin/Debug/net10.0-windows/appsettings.json
```

## Modifying Settings

You can modify the `appsettings.json` file:

1. **During Development**: Edit the file in the project root
2. **After Deployment**: Edit the file in the application's directory
3. **Programmatically**: Use the configuration API to read/write (requires additional setup)

## Example: Complete Email Sending Form

```csharp
public partial class SendEmailForm : Form
{
    private IMAPSender.EmailSender _emailSender;
    private AppSettings _appSettings;

    public SendEmailForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        _appSettings = ConfigurationHelper.GetAppSettings();
        
        _emailSender = new IMAPSender.EmailSender(
            _appSettings.SmtpSettings.Host,
            _appSettings.SmtpSettings.Port,
            _appSettings.SmtpSettings.Username,
            _appSettings.SmtpSettings.Password
        );
        
        // Pre-fill from address
        txtFrom.Text = _appSettings.EmailDefaults.FromAddress;
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        try
        {
            await _emailSender.SendAsync(
                from: txtFrom.Text,
                to: txtTo.Text,
                subject: txtSubject.Text,
                body: txtBody.Text,
                isHtml: chkHtml.Checked
            );

            MessageBox.Show("Email sent successfully!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to send email: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
```

## Troubleshooting

### Configuration file not found
- Ensure `appsettings.json` is in the same directory as the .exe
- Check that the file's "Copy to Output Directory" is set to "Copy if newer" or "Copy always"

### Settings not loading
- Verify the JSON format is valid
- Check for typos in section names
- Ensure the file encoding is UTF-8

### Password not working
- Check for extra spaces in the password value
- Verify the password is correct in your email provider
- Ensure special characters are properly escaped in JSON

## Classes Reference

### AppSettings
Container for all application settings

### SmtpSettings
SMTP server configuration

### EmailDefaults
Default email sender information

### ConfigurationHelper
Static helper class for loading configuration
