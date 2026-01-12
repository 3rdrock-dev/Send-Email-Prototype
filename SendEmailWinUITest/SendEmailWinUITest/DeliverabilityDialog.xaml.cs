using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace SendEmailWinUITest
{
    public sealed partial class DeliverabilityDialog : ContentDialog
    {
        private readonly string _fromEmail;
        private List<DeliverabilityCheckResult> _results = new();

        public DeliverabilityDialog(string fromEmail)
        {
            this.InitializeComponent();
            _fromEmail = fromEmail;
            
            Title = "Email Deliverability Diagnostics";
            CloseButtonText = "Close";
            
            var domain = EmailDeliverabilityChecker.ExtractDomain(_fromEmail);
            txtDomain.Text = domain;
        }

        private async void btnCheckDeliverability_Click(object sender, RoutedEventArgs e)
        {
            btnCheckDeliverability.IsEnabled = false;
            var originalContent = btnCheckDeliverability.Content;
            btnCheckDeliverability.Content = "Checking...";

            try
            {
                var domain = EmailDeliverabilityChecker.ExtractDomain(_fromEmail);
                _results = await EmailDeliverabilityChecker.CheckAllRecordsAsync(domain);

                // Clear previous results
                pnlResults.Children.Clear();

                // Display results
                foreach (var result in _results)
                {
                    var resultCard = CreateResultCard(result);
                    pnlResults.Children.Add(resultCard);
                }

                pnlResults.Visibility = Visibility.Visible;
                pnlRecommendations.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Error checking deliverability: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
            finally
            {
                btnCheckDeliverability.IsEnabled = true;
                btnCheckDeliverability.Content = originalContent;
            }
        }

        private Border CreateResultCard(DeliverabilityCheckResult result)
        {
            var border = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var stackPanel = new StackPanel { Spacing = 8 };

            // Header with icon and status
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10
            };

            var statusIcon = new FontIcon
            {
                Glyph = result.IsConfigured ? "\uE73E" : "\uE783", // Checkmark or Warning
                FontSize = 20,
                Foreground = result.IsConfigured 
                    ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green)
                    : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red)
            };

            var recordTypeText = new TextBlock
            {
                Text = $"{result.RecordType} Record",
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            };

            headerPanel.Children.Add(statusIcon);
            headerPanel.Children.Add(recordTypeText);
            stackPanel.Children.Add(headerPanel);

            // Status message
            var messageText = new TextBlock
            {
                Text = result.Message,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };
            stackPanel.Children.Add(messageText);

            // Record value (if found)
            if (!string.IsNullOrEmpty(result.Value))
            {
                var valueText = new TextBlock
                {
                    Text = result.Value,
                    FontSize = 11,
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                    TextWrapping = TextWrapping.Wrap,
                    IsTextSelectionEnabled = true
                };

                var valueBorder = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)App.Current.Resources["LayerFillColorDefaultBrush"],
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(4),
                    Child = valueText
                };
                stackPanel.Children.Add(valueBorder);
            }

            Grid.SetColumn(stackPanel, 0);
            mainGrid.Children.Add(stackPanel);

            // Copy button
            var copyButton = new Button
            {
                Content = new FontIcon { Glyph = "\uE8C8", FontSize = 14 },
                Width = 36,
                Height = 36,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(8, 0, 0, 0),
                Tag = result
            };

            copyButton.Click += CopyResultButton_Click;

            Grid.SetColumn(copyButton, 1);
            mainGrid.Children.Add(copyButton);

            border.Child = mainGrid;
            return border;
        }

        private async void CopyResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is DeliverabilityCheckResult result)
            {
                var textToCopy = $"{result.RecordType} Record\n" +
                                $"Status: {(result.IsConfigured ? "Configured" : "Not Configured")}\n" +
                                $"Message: {result.Message}\n";

                if (!string.IsNullOrEmpty(result.Value))
                {
                    textToCopy += $"Value: {result.Value}\n";
                }

                try
                {
                    var dataPackage = new DataPackage();
                    dataPackage.SetText(textToCopy);
                    Clipboard.SetContent(dataPackage);
                    
                    button.Content = new FontIcon { Glyph = "\uE73E", FontSize = 14 }; // Checkmark
                    
                    // Reset icon after 2 seconds
                    await Task.Delay(2000);
                    button.Content = new FontIcon { Glyph = "\uE8C8", FontSize = 14 }; // Copy icon
                }
                catch (Exception ex)
                {
                    var errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"Failed to copy to clipboard: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void btnCopyRecommendations_Click(object sender, RoutedEventArgs e)
        {
            var recommendations = @"Recommendations for SmarterASP.NET Email Deliverability

1. Contact SmarterASP.NET Support:
   Ask them to configure SPF, DKIM, and DMARC records for your domain.

2. Microsoft-Specific Whitelisting:
   - Sign up for Microsoft SNDS (Smart Network Data Services)
   - URL: https://postmaster.live.com/snds/
   - This helps monitor your IP reputation with Microsoft

3. Junk Mail Reporting (JMR):
   - Sign up at: https://postmaster.live.com/
   - This provides feedback on spam complaints

4. Email Best Practices:
   - Avoid spam trigger words in subject lines
   - Include an unsubscribe link if sending bulk emails
   - Use a consistent From address
   - Ensure your email has both HTML and plain text versions";

            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(recommendations);
                Clipboard.SetContent(dataPackage);
                
                btnCopyRecommendations.Content = new FontIcon { Glyph = "\uE73E", FontSize = 14 }; // Checkmark
                
                // Reset icon after 2 seconds
                await Task.Delay(2000);
                btnCopyRecommendations.Content = new FontIcon { Glyph = "\uE8C8", FontSize = 14 }; // Copy icon
            }
            catch (Exception ex)
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to copy to clipboard: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}
