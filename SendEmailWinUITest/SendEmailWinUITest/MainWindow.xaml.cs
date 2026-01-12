using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SendEmailWinUITest;

public sealed partial class MainWindow : Window
{
    private IMAPSender.EmailSender? _emailSender;
    private AppSettings? _appSettings;
    private bool _isFirstActivation = true;

    public MainWindow()
    {
        this.InitializeComponent();
        Title = "Email Sender";

        // Set window size
        var appWindow = this.AppWindow;
        appWindow.Resize(new Windows.Graphics.SizeInt32(900, 980));

        // Set window icon
        SetWindowIcon();

        // Load settings after the window is activated
        this.Activated += MainWindow_Activated;
    }

    private void SetWindowIcon()
    {
        try
        {
            // Try to set the window icon from the Images folder
            var iconPath = Path.Combine(AppContext.BaseDirectory, "Images", "3rdRockLogoSmall.ico");
            
            if (File.Exists(iconPath))
            {
                this.AppWindow.SetIcon(iconPath);
            }
            else
            {
                // Fallback to .png if .ico doesn't exist
                var pngPath = Path.Combine(AppContext.BaseDirectory, "Images", "3rdRockLogoSmall.png");
                if (File.Exists(pngPath))
                {
                    this.AppWindow.SetIcon(pngPath);
                }
            }
        }
        catch
        {
            // Silently fail if icon cannot be set
        }
    }

    private async void MainWindow_Activated(object sender, WindowActivatedEventArgs e)
    {
        if (_isFirstActivation && e.WindowActivationState != WindowActivationState.Deactivated)
        {
            _isFirstActivation = false;
            
            // Give the UI thread a moment to fully initialize XamlRoot
            await Task.Delay(100);
            
            // Verify XamlRoot is available before loading settings
            if (this.Content?.XamlRoot != null)
            {
                await LoadSettingsAsync();
            }
        }
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            _appSettings = ConfigurationHelper.GetAppSettings();

            // Check if credentials are configured
            if (string.IsNullOrEmpty(_appSettings.SmtpSettings.Password) ||
                _appSettings.SmtpSettings.Port == 9999)
            {
                // Only show dialog if XamlRoot is available
                if (this.Content?.XamlRoot != null)
                {
                    ShowBlurOverlay();

                    // Show welcome dialog
                    var welcomeDialog = new ContentDialog
                    {
                        Title = "Welcome to Email Sender",
                        Content = "Let's configure your SMTP settings to get started.",
                        PrimaryButtonText = "Configure Now",
                        CloseButtonText = "Cancel",
                        XamlRoot = this.Content.XamlRoot
                    };

                    var result = await welcomeDialog.ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        await OpenSettingsDialog();
                        _appSettings = ConfigurationHelper.GetAppSettings();
                    }

                    HideBlurOverlay();
                }
            }

            // Display SMTP settings
            lblHostValue.Text = _appSettings.SmtpSettings.Host;
            lblPortValue.Text = _appSettings.SmtpSettings.Port.ToString();
            lblUsernameValue.Text = _appSettings.SmtpSettings.Username;
            lblPasswordValue.Text = string.IsNullOrEmpty(_appSettings.SmtpSettings.Password)
                ? "(not set)"
                : new string('*', _appSettings.SmtpSettings.Password.Length);

            Title = $"Email Sender - {_appSettings.SmtpSettings.Host}";

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
            // Only show error dialog if XamlRoot is available
            if (this.Content?.XamlRoot != null)
            {
                await ShowErrorDialog("Configuration Error", $"Error loading configuration: {ex.Message}");
            }
            else
            {
                // Fallback: just set error text without dialog
                System.Diagnostics.Debug.WriteLine($"Configuration Error: {ex.Message}");
            }

            lblHostValue.Text = "Error loading settings";
            lblPortValue.Text = "-";
            lblUsernameValue.Text = "-";
            lblPasswordValue.Text = "-";
        }
    }

    private async void btnSend_Click(object sender, RoutedEventArgs e)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(txtToEmail.Text))
        {
            await ShowWarningDialog("Validation Error", "Please enter a recipient email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(txtSubject.Text))
        {
            await ShowWarningDialog("Validation Error", "Please enter a subject.");
            return;
        }

        if (_emailSender == null || _appSettings == null)
        {
            await ShowErrorDialog("Error", "Email sender is not initialized. Please check your configuration.");
            return;
        }

        btnSend.IsEnabled = false;
        var originalContent = btnSend.Content;
        btnSend.Content = "Sending...";

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

            await ShowSuccessDialog("Success", $"Email sent successfully to {txtToEmail.Text}!");
            ClearForm();
        }
        catch (Exception ex)
        {
            await ShowErrorDialog("Send Error", $"Failed to send email: {ex.Message}");
        }
        finally
        {
            btnSend.IsEnabled = true;
            btnSend.Content = originalContent;
        }
    }

    private void ClearForm()
    {
        txtToEmail.Text = string.Empty;
        txtSubject.Text = string.Empty;
        txtBody.Text = string.Empty;
        chkIsHtml.IsChecked = false;
    }

    private async void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        ShowBlurOverlay();
        await OpenSettingsDialog();
        HideBlurOverlay();
    }

    private async Task OpenSettingsDialog()
    {
        var settingsDialog = new SettingsDialog();
        settingsDialog.XamlRoot = this.Content.XamlRoot;

        var result = await settingsDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await LoadSettingsAsync();

            var restartDialog = new ContentDialog
            {
                Title = "Restart Application",
                Content = "Settings have been saved successfully!\n\nWould you like to restart the application now to apply all changes?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = this.Content.XamlRoot
            };

            var restartResult = await restartDialog.ShowAsync();
            if (restartResult == ContentDialogResult.Primary)
            {
                RestartApplication();
            }
        }
    }

    private void RestartApplication()
    {
        var exePath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(exePath))
        {
            System.Diagnostics.Process.Start(exePath);
            Application.Current.Exit();
        }
    }

    private void ShowBlurOverlay()
    {
        BlurOverlay.Visibility = Visibility.Visible;
    }

    private void HideBlurOverlay()
    {
        BlurOverlay.Visibility = Visibility.Collapsed;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void btnDeliverability_Click(object sender, RoutedEventArgs e)
    {
        if (_appSettings == null || string.IsNullOrEmpty(_appSettings.EmailDefaults.FromAddress))
        {
            await ShowWarningDialog("Configuration Required", "Please configure your email settings first.");
            return;
        }

        ShowBlurOverlay();

        var deliverabilityDialog = new DeliverabilityDialog(_appSettings.EmailDefaults.FromAddress);
        deliverabilityDialog.XamlRoot = this.Content.XamlRoot;
        await deliverabilityDialog.ShowAsync();

        HideBlurOverlay();
    }

    // Helper methods for dialogs
    private async Task ShowErrorDialog(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async Task ShowWarningDialog(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private async Task ShowSuccessDialog(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }
}
