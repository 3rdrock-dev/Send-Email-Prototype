using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SendEmailWpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMAPSender.EmailSender? _emailSender;
        private AppSettings? _appSettings;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load and display settings
            try
            {
                _appSettings = ConfigurationHelper.GetAppSettings();

                // Check if credentials are configured
                // Port 9999999999 is used as a sentinel value for unconfigured settings
                if (string.IsNullOrEmpty(_appSettings.SmtpSettings.Password) || 
                    _appSettings.SmtpSettings.Port == 9999)
                {
                    // Show blur overlay
                    ShowBlurOverlay();
                    
                    // Show custom welcome dialog
                    var welcomeDialog = new WelcomeDialog
                    {
                        Owner = this
                    };

                    if (welcomeDialog.ShowDialog() == true)
                    {
                        // Open Settings window
                        var settingsWindow = new SettingsWindow
                        {
                            Owner = this
                        };

                        if (settingsWindow.ShowDialog() == true)
                        {
                            // Reload settings after setup
                            _appSettings = ConfigurationHelper.GetAppSettings();
                        }
                    }
                    
                    HideBlurOverlay();
                }

                // Display SMTP settings in labels
                lblHostValue.Content = _appSettings.SmtpSettings.Host;
                lblPortValue.Content = _appSettings.SmtpSettings.Port.ToString();
                lblUsernameValue.Content = _appSettings.SmtpSettings.Username;

                // Mask password for security (show asterisks if password is set)
                lblPasswordValue.Content = string.IsNullOrEmpty(_appSettings.SmtpSettings.Password)
                    ? "(not set)"
                    : new string('*', _appSettings.SmtpSettings.Password.Length);

                // Update window title
                this.Title = $"Email Sender - {_appSettings.SmtpSettings.Host}";

                // Initialize email sender
                if (!string.IsNullOrEmpty(_appSettings.SmtpSettings.Password))
                {
                    _emailSender = new IMAPSender.EmailSender(
                        _appSettings.SmtpSettings.Host,
                        _appSettings.SmtpSettings.Port,
                        _appSettings.SmtpSettings.Username,
                        _appSettings.SmtpSettings.Password
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Show error in labels
                lblHostValue.Content = "Error loading settings";
                lblPortValue.Content = "-";
                lblUsernameValue.Content = "-";
                lblPasswordValue.Content = "-";
            }
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtToEmail.Text))
            {
                MessageBox.Show("Please enter a recipient email address.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtToEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Please enter a subject.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtSubject.Focus();
                return;
            }

            if (_emailSender == null || _appSettings == null)
            {
                MessageBox.Show("Email sender is not initialized. Please check your configuration.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Disable send button to prevent double-clicking
            btnSend.IsEnabled = false;
            btnSend.Content = "Sending...";
            this.Cursor = Cursors.Wait;

            try
            {
                await _emailSender.SendAsync(
                    from: _appSettings.EmailDefaults.FromAddress,
                    fromName: _appSettings.EmailDefaults.FromName,
                    to: txtToEmail.Text.Trim(),
                    subject: txtSubject.Text.Trim(),
                    body: txtBody.Text,
                    isHtml: chkIsHtml.IsChecked == true
                );

                MessageBox.Show($"Email sent successfully to {txtToEmail.Text}!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Clear form after successful send
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}",
                    "Send Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // Re-enable send button
                btnSend.IsEnabled = true;
                btnSend.Content = "Send Email";
                this.Cursor = Cursors.Arrow;
            }
        }

        private void ClearForm()
        {
            txtToEmail.Clear();
            txtSubject.Clear();
            txtBody.Clear();
            chkIsHtml.IsChecked = false;
            txtToEmail.Focus();
        }

        private void btnChangeCredentials_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show blur overlay
                ShowBlurOverlay();

                // Show settings window
                var settingsWindow = new SettingsWindow
                {
                    Owner = this
                };

                var dialogResult = settingsWindow.ShowDialog();

                // Hide blur overlay
                HideBlurOverlay();

                if (dialogResult == true)
                {
                    // Reload settings after changes
                    ReloadSettings();
                    
                    // Ask user if they want to restart the application
                    var result = MessageBox.Show(
                        "Settings have been saved successfully!\n\nWould you like to restart the application now to apply all changes?",
                        "Restart Application",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        RestartApplication();
                    }
                }
            }
            catch (Exception ex)
            {
                // Make sure to hide blur overlay in case of error
                HideBlurOverlay();
                
                MessageBox.Show($"Failed to open settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            btnChangeCredentials_Click(sender, e);
        }

        private void RestartApplication()
        {
            try
            {
                // Get the current executable path
                var exePath = Environment.ProcessPath;
                
                if (string.IsNullOrEmpty(exePath))
                {
                    MessageBox.Show("Unable to determine application path for restart.",
                        "Restart Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                // Start a new instance
                System.Diagnostics.Process.Start(exePath);
                
                // Close current instance
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart application: {ex.Message}",
                    "Restart Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ReloadSettings()
        {
            try
            {
                _appSettings = ConfigurationHelper.GetAppSettings();

                // Update display
                lblHostValue.Content = _appSettings.SmtpSettings.Host;
                lblPortValue.Content = _appSettings.SmtpSettings.Port.ToString();
                lblUsernameValue.Content = _appSettings.SmtpSettings.Username;
                lblPasswordValue.Content = string.IsNullOrEmpty(_appSettings.SmtpSettings.Password)
                    ? "(not set)"
                    : new string('*', _appSettings.SmtpSettings.Password.Length);

                // Update window title
                this.Title = $"Email Sender - {_appSettings.SmtpSettings.Host}";

                // Reinitialize email sender
                if (!string.IsNullOrEmpty(_appSettings.SmtpSettings.Password))
                {
                    _emailSender = new IMAPSender.EmailSender(
                        _appSettings.SmtpSettings.Host,
                        _appSettings.SmtpSettings.Port,
                        _appSettings.SmtpSettings.Username,
                        _appSettings.SmtpSettings.Password
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reloading settings: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ShowBlurOverlay()
        {
            MainContentGrid.Effect = new System.Windows.Media.Effects.BlurEffect { Radius = 10 };
            BlurOverlay.Visibility = Visibility.Visible;
        }

        private void HideBlurOverlay()
        {
            MainContentGrid.Effect = null;
            BlurOverlay.Visibility = Visibility.Collapsed;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDeliverability_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_appSettings == null || string.IsNullOrEmpty(_appSettings.EmailDefaults.FromAddress))
                {
                    MessageBox.Show("Please configure your email settings first.",
                        "Configuration Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Show blur overlay
                ShowBlurOverlay();

                // Show deliverability diagnostics window
                var deliverabilityWindow = new DeliverabilityWindow(_appSettings.EmailDefaults.FromAddress)
                {
                    Owner = this
                };

                deliverabilityWindow.ShowDialog();

                // Hide blur overlay
                HideBlurOverlay();
            }
            catch (Exception ex)
            {
                // Make sure to hide blur overlay in case of error
                HideBlurOverlay();
                
                MessageBox.Show($"Failed to open diagnostics: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}