using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace SendEmailWinUITest
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        private AppSettings? _currentSettings;
        public bool SettingsSaved { get; private set; }

        public SettingsDialog()
        {
            this.InitializeComponent();
            this.Loaded += SettingsDialog_Loaded;
            
            // Set dialog properties
            PrimaryButtonText = "Save Settings";
            CloseButtonText = "Cancel";
            DefaultButton = ContentDialogButton.Primary;
            
            // Hook up button handlers
            PrimaryButtonClick += SettingsDialog_PrimaryButtonClick;
        }

        private void SettingsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            try
            {
                _currentSettings = ConfigurationHelper.GetAppSettings();

                // Load SMTP settings
                txtSmtpHost.Text = _currentSettings.SmtpSettings.Host;
                txtSmtpPort.Text = _currentSettings.SmtpSettings.Port.ToString();
                txtUsername.Text = _currentSettings.SmtpSettings.Username;
                
                // Show placeholder hint if password exists
                if (!string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password))
                {
                    txtPassword.Password = string.Empty;
                    txtPasswordHint.Visibility = Visibility.Visible;
                }
                else
                {
                    txtPassword.Password = string.Empty;
                    txtPasswordHint.Visibility = Visibility.Collapsed;
                }

                // Load email defaults
                txtFromAddress.Text = _currentSettings.EmailDefaults.FromAddress;
                txtFromName.Text = _currentSettings.EmailDefaults.FromName;
                
                // Ensure InfoBar shows security notice
                ResetInfoBarToSecurityNotice();
            }
            catch (Exception ex)
            {
                ShowErrorInInfoBar($"Error loading settings: {ex.Message}");
            }
        }

        private async void SettingsDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Get deferral to perform async operations
            var deferral = args.GetDeferral();

            try
            {
                // Validate inputs
                var validationError = ValidateInputs();
                if (validationError != null)
                {
                    args.Cancel = true;
                    ShowErrorInInfoBar(validationError);
                    deferral.Complete();
                    return;
                }

                // Create updated settings object
                var updatedSettings = new AppSettings
                {
                    SmtpSettings = new SmtpSettings
                    {
                        Host = txtSmtpHost.Text.Trim(),
                        Port = int.Parse(txtSmtpPort.Text.Trim()),
                        Username = txtUsername.Text.Trim(),
                        Password = string.Empty // Don't store in JSON
                    },
                    EmailDefaults = new EmailDefaults
                    {
                        FromAddress = txtFromAddress.Text.Trim(),
                        FromName = txtFromName.Text.Trim()
                    }
                };

                // Save credentials to Windows Credential Manager if password is provided
                if (!string.IsNullOrEmpty(txtPassword.Password))
                {
                    bool credentialSaved = SecureCredentialService.SaveCredential(
                        updatedSettings.SmtpSettings.Username,
                        txtPassword.Password);

                    if (!credentialSaved)
                    {
                        args.Cancel = true;
                        ShowErrorInInfoBar("Failed to save credentials to secure storage.");
                        deferral.Complete();
                        return;
                    }
                }

                // Save settings to appsettings.json
                bool settingsSaved = ConfigurationHelper.SaveAppSettings(updatedSettings);

                if (settingsSaved)
                {
                    SettingsSaved = true;
                    ResetInfoBarToSecurityNotice();
                }
                else
                {
                    args.Cancel = true;
                    ShowErrorInInfoBar("Failed to save settings to configuration file.");
                }
            }
            catch (Exception ex)
            {
                args.Cancel = true;
                ShowErrorInInfoBar($"Failed to save settings: {ex.Message}");
            }
            finally
            {
                deferral.Complete();
            }
        }

        private string? ValidateInputs()
        {
            // Validate SMTP Host
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                return "Please enter an SMTP host.";
            }

            // Validate SMTP Port
            if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                return "Please enter a valid port number (1-65535).";
            }

            // Validate Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                return "Please enter a username (email address).";
            }

            // Validate Password - require it if no existing password
            bool hasExistingPassword = _currentSettings != null && 
                                      !string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password);
            
            if (string.IsNullOrEmpty(txtPassword.Password) && !hasExistingPassword)
            {
                return "Please enter a password.";
            }

            // Validate From Address
            if (string.IsNullOrWhiteSpace(txtFromAddress.Text))
            {
                return "Please enter a from email address.";
            }

            // Validate From Name
            if (string.IsNullOrWhiteSpace(txtFromName.Text))
            {
                return "Please enter a from name.";
            }

            return null;
        }

        private void ShowErrorInInfoBar(string message)
        {
            securityNoticeInfoBar.Title = "Error";
            securityNoticeInfoBar.Message = message;
            securityNoticeInfoBar.Severity = InfoBarSeverity.Error;
            securityNoticeInfoBar.IsOpen = true;
        }

        private void ResetInfoBarToSecurityNotice()
        {
            securityNoticeInfoBar.Title = "Security Notice";
            securityNoticeInfoBar.Message = "";
            securityNoticeInfoBar.Severity = InfoBarSeverity.Informational;
            securityNoticeInfoBar.IsOpen = true;
        }

        private async Task ShowErrorDialogAsync(string title, string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void ShowErrorMessage(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Validation Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async Task ShowErrorDialogAsync(string message)
        {
            var errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await errorDialog.ShowAsync();
        }

        private async void btnTestConnection_Click(object sender, RoutedEventArgs e)
        {
            // Validate SMTP settings first
            string? validationError = null;
            
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                validationError = "Please enter an SMTP host.";
            }
            else if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                validationError = "Please enter a valid port number (1-65535).";
            }
            else if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                validationError = "Please enter a username.";
            }

            if (validationError != null)
            {
                ShowErrorInInfoBar(validationError);
                return;
            }

            // Check password
            string passwordToTest;
            if (!string.IsNullOrEmpty(txtPassword.Password))
            {
                passwordToTest = txtPassword.Password;
            }
            else if (_currentSettings != null && !string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password))
            {
                passwordToTest = _currentSettings.SmtpSettings.Password;
            }
            else
            {
                ShowErrorInInfoBar("Please enter a password to test the connection.");
                return;
            }

            // Disable button and show testing state
            btnTestConnection.IsEnabled = false;
            var originalContent = btnTestConnection.Content;
            btnTestConnection.Content = "Testing...";

            try
            {
                // Test the connection
                int portValue = int.Parse(txtSmtpPort.Text.Trim());
                bool success = await TestSmtpConnectionAsync(
                    txtSmtpHost.Text.Trim(),
                    portValue,
                    txtUsername.Text.Trim(),
                    passwordToTest);

                if (success)
                {
                    // Show success in InfoBar
                    securityNoticeInfoBar.Title = "Test Connection";
                    securityNoticeInfoBar.Message = "Connection successful! SMTP server authentication was successful.";
                    securityNoticeInfoBar.Severity = InfoBarSeverity.Success;
                    securityNoticeInfoBar.IsOpen = true;
                }
                else
                {
                    ShowErrorInInfoBar("Connection failed. Please check your SMTP settings and credentials.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorInInfoBar($"Connection failed: {ex.Message}");
            }
            finally
            {
                // Re-enable button
                btnTestConnection.IsEnabled = true;
                btnTestConnection.Content = originalContent;
            }
        }

        private async Task<bool> TestSmtpConnectionAsync(string host, int port, string username, string password)
        {
            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();
                
                // Connect to the SMTP server
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                
                // Authenticate
                await client.AuthenticateAsync(username, password);
                
                // Disconnect
                await client.DisconnectAsync(true);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
