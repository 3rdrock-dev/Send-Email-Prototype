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
            // Set the actual dialog window size after it's loaded
            if (this.Content is FrameworkElement content)
            {
                content.MinHeight = 1111;
                content.MaxHeight = 1111;
            }
            
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
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error loading settings: {ex.Message}");
            }
        }

        private async void SettingsDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Get deferral to perform async operations
            var deferral = args.GetDeferral();

            try
            {
                // Validate inputs
                if (!ValidateInputs())
                {
                    args.Cancel = true;
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
                        await ShowErrorDialogAsync("Failed to save credentials to secure storage.");
                        deferral.Complete();
                        return;
                    }
                }

                // Save settings to appsettings.json
                bool settingsSaved = ConfigurationHelper.SaveAppSettings(updatedSettings);

                if (settingsSaved)
                {
                    SettingsSaved = true;
                }
                else
                {
                    args.Cancel = true;
                    await ShowErrorDialogAsync("Failed to save settings to configuration file.");
                }
            }
            catch (Exception ex)
            {
                args.Cancel = true;
                await ShowErrorDialogAsync($"Failed to save settings: {ex.Message}");
            }
            finally
            {
                deferral.Complete();
            }
        }

        private bool ValidateInputs()
        {
            // Validate SMTP Host
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                ShowErrorMessage("Please enter an SMTP host.");
                return false;
            }

            // Validate SMTP Port
            if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                ShowErrorMessage("Please enter a valid port number (1-65535).");
                return false;
            }

            // Validate Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowErrorMessage("Please enter a username (email address).");
                return false;
            }

            // Validate Password - require it if no existing password
            bool hasExistingPassword = _currentSettings != null && 
                                      !string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password);
            
            if (string.IsNullOrEmpty(txtPassword.Password) && !hasExistingPassword)
            {
                ShowErrorMessage("Please enter a password.");
                return false;
            }

            // Validate From Address
            if (string.IsNullOrWhiteSpace(txtFromAddress.Text))
            {
                ShowErrorMessage("Please enter a from email address.");
                return false;
            }

            // Validate From Name
            if (string.IsNullOrWhiteSpace(txtFromName.Text))
            {
                ShowErrorMessage("Please enter a from name.");
                return false;
            }

            return true;
        }

        private void ShowErrorMessage(string message)
        {
            errorTextBlock.Message = message;
            errorTextBlock.IsOpen = true;
            errorTextBlock.Visibility = Visibility.Visible;
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
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                ShowErrorMessage("Please enter an SMTP host.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                ShowErrorMessage("Please enter a valid port number (1-65535).");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowErrorMessage("Please enter a username.");
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
                ShowErrorMessage("Please enter a password to test the connection.");
                return;
            }

            // Disable button and show testing state
            btnTestConnection.IsEnabled = false;
            var originalContent = btnTestConnection.Content;
            btnTestConnection.Content = "Testing...";

            try
            {
                // Test the connection
                bool success = await TestSmtpConnectionAsync(
                    txtSmtpHost.Text.Trim(),
                    port,
                    txtUsername.Text.Trim(),
                    passwordToTest);

                var resultDialog = new ContentDialog
                {
                    Title = "Test Connection",
                    Content = success ? 
                        "Connection successful!\n\nSMTP server authentication was successful." :
                        "Connection failed.\n\nPlease check your SMTP settings and credentials.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await resultDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Test Connection Error",
                    Content = $"Connection failed:\n\n{ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
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
