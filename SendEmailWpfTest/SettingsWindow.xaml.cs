using System;
using System.Windows;

namespace SendEmailWpfTest
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private AppSettings? _currentSettings;
        public bool SettingsSaved { get; private set; }

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                    // Set placeholder to indicate password is already set
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
                MessageBox.Show($"Error loading settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (!ValidateInputs())
            {
                return;
            }

            try
            {
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
                        MessageBox.Show("Failed to save credentials to Windows Credential Manager.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                }
                else if (_currentSettings != null && 
                         !string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password))
                {
                    // Password field is empty, but we have existing credentials
                    // Keep the existing credentials if username hasn't changed
                    if (_currentSettings.SmtpSettings.Username == updatedSettings.SmtpSettings.Username)
                    {
                        // Credentials remain unchanged
                    }
                    else
                    {
                        // Username changed but no password provided
                        var result = MessageBox.Show(
                            "You changed the username but didn't enter a password. Do you want to continue without updating the password?",
                            "Password Required",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.No)
                        {
                            txtPassword.Focus();
                            return;
                        }
                    }
                }

                // Save settings to appsettings.json
                bool settingsSaved = ConfigurationHelper.SaveAppSettings(updatedSettings);

                if (settingsSaved)
                {
                    SettingsSaved = true;
                    MessageBox.Show("Settings saved successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to save settings to configuration file.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            // Validate SMTP Host
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                MessageBox.Show("Please enter an SMTP host.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtSmtpHost.Focus();
                return false;
            }

            // Validate SMTP Port
            if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtSmtpPort.Focus();
                return false;
            }

            // Validate Username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username (email address).",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtUsername.Focus();
                return false;
            }

            // Validate Password - require it if no existing password or username changed
            bool hasExistingPassword = _currentSettings != null && 
                                      !string.IsNullOrEmpty(_currentSettings.SmtpSettings.Password);
            bool usernameChanged = _currentSettings != null && 
                                  _currentSettings.SmtpSettings.Username != txtUsername.Text.Trim();
            
            if (string.IsNullOrEmpty(txtPassword.Password) && (!hasExistingPassword || usernameChanged))
            {
                MessageBox.Show("Please enter a password.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validate From Address
            if (string.IsNullOrWhiteSpace(txtFromAddress.Text))
            {
                MessageBox.Show("Please enter a from email address.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFromAddress.Focus();
                return false;
            }

            // Validate From Name
            if (string.IsNullOrWhiteSpace(txtFromName.Text))
            {
                MessageBox.Show("Please enter a from name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFromName.Focus();
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            // Validate SMTP settings first
            if (string.IsNullOrWhiteSpace(txtSmtpHost.Text))
            {
                MessageBox.Show("Please enter an SMTP host.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtSmtpHost.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSmtpPort.Text) || 
                !int.TryParse(txtSmtpPort.Text, out int port) || 
                port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtSmtpPort.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtUsername.Focus();
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
                MessageBox.Show("Please enter a password to test the connection.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            // Disable button and show testing state
            btnTestConnection.IsEnabled = false;
            btnTestConnection.Content = "Testing...";
            this.Cursor = System.Windows.Input.Cursors.Wait;

            try
            {
                // Test the connection
                bool success = await TestSmtpConnectionAsync(
                    txtSmtpHost.Text.Trim(),
                    port,
                    txtUsername.Text.Trim(),
                    passwordToTest);

                if (success)
                {
                    MessageBox.Show("Connection successful!\n\nSMTP server authentication was successful.",
                        "Test Connection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Connection failed.\n\nPlease check your SMTP settings and credentials.",
                        "Test Connection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed:\n\n{ex.Message}",
                    "Test Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // Re-enable button
                btnTestConnection.IsEnabled = true;
                btnTestConnection.Content = "Test Connection";
                this.Cursor = System.Windows.Input.Cursors.Arrow;
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
