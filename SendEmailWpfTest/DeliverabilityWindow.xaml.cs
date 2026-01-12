using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SendEmailWpfTest
{
    public partial class DeliverabilityWindow : Window
    {
        private readonly string _fromEmail;

        public DeliverabilityWindow(string fromEmail)
        {
            InitializeComponent();
            _fromEmail = fromEmail;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var domain = EmailDeliverabilityChecker.ExtractDomain(_fromEmail);
            txtDomain.Text = domain;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void CheckDeliverability_Click(object sender, RoutedEventArgs e)
        {
            btnCheckDeliverability.IsEnabled = false;
            btnCheckDeliverability.Content = "Checking...";
            this.Cursor = Cursors.Wait;

            try
            {
                var domain = EmailDeliverabilityChecker.ExtractDomain(_fromEmail);
                var results = await EmailDeliverabilityChecker.CheckAllRecordsAsync(domain);

                // Clear previous results
                pnlResults.Children.Clear();

                // Display results
                foreach (var result in results)
                {
                    var resultCard = CreateResultCard(result);
                    pnlResults.Children.Add(resultCard);
                }

                pnlResults.Visibility = Visibility.Visible;
                pnlRecommendations.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking deliverability: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnCheckDeliverability.IsEnabled = true;
                btnCheckDeliverability.Content = "Run Diagnostics";
                this.Cursor = Cursors.Arrow;
            }
        }

        private Border CreateResultCard(DeliverabilityCheckResult result)
        {
            var border = new Border
            {
                Background = (SolidColorBrush)FindResource("SurfaceBrush"),
                BorderBrush = (SolidColorBrush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var mainGrid = new Grid();
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var stackPanel = new StackPanel();

            // Header with icon and status
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var statusIcon = new TextBlock
            {
                Text = result.IsConfigured ? "\uE73E" : "\uE783", // Checkmark or Warning
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                FontSize = 20,
                Foreground = result.IsConfigured 
                    ? (SolidColorBrush)FindResource("SuccessBrush") 
                    : (SolidColorBrush)FindResource("ErrorBrush"),
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var recordTypeText = new TextBlock
            {
                Text = $"{result.RecordType} Record",
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Foreground = (SolidColorBrush)FindResource("TextBrush"),
                VerticalAlignment = VerticalAlignment.Center
            };

            headerPanel.Children.Add(statusIcon);
            headerPanel.Children.Add(recordTypeText);
            stackPanel.Children.Add(headerPanel);

            // Status message
            var messageText = new TextBlock
            {
                Text = result.Message,
                FontSize = 13,
                Foreground = (SolidColorBrush)FindResource("SecondaryTextBrush"),
                Margin = new Thickness(0, 0, 0, 10),
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
                    FontFamily = new FontFamily("Consolas"),
                    Foreground = (SolidColorBrush)FindResource("MutedTextBrush"),
                    TextWrapping = TextWrapping.Wrap,
                    Background = (SolidColorBrush)FindResource("BackgroundBrush"),
                    Padding = new Thickness(10)
                };
                stackPanel.Children.Add(valueText);
            }

            Grid.SetColumn(stackPanel, 0);
            mainGrid.Children.Add(stackPanel);

            // Copy button
            var copyButton = new Button
            {
                Content = "\uE8C8", // Copy icon
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Width = 32,
                Height = 32,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 0, 0, 0),
                ToolTip = "Copy to clipboard",
                Tag = result, // Store result for click handler
                Cursor = Cursors.Hand
            };

            copyButton.Click += CopyResultButton_Click;

            // Style the copy button
            var copyButtonStyle = new Style(typeof(Button));
            copyButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
            copyButtonStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
            copyButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, FindResource("SecondaryTextBrush")));

            var copyButtonTemplate = new ControlTemplate(typeof(Button));
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            var presenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            presenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            presenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            borderFactory.AppendChild(presenterFactory);
            copyButtonTemplate.VisualTree = borderFactory;
            copyButtonStyle.Setters.Add(new Setter(Button.TemplateProperty, copyButtonTemplate));

            var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, FindResource("InputBrush")));
            trigger.Setters.Add(new Setter(Button.ForegroundProperty, FindResource("TextBrush")));
            copyButtonStyle.Triggers.Add(trigger);

            copyButton.Style = copyButtonStyle;

            Grid.SetColumn(copyButton, 1);
            mainGrid.Children.Add(copyButton);

            border.Child = mainGrid;
            return border;
        }

        private void CopyResultButton_Click(object sender, RoutedEventArgs e)
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
                    Clipboard.SetText(textToCopy);
                    button.Content = "\uE73E"; // Checkmark icon
                    
                    // Reset icon after 2 seconds
                    var timer = new System.Windows.Threading.DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(2)
                    };
                    timer.Tick += (s, args) =>
                    {
                        button.Content = "\uE8C8"; // Copy icon
                        timer.Stop();
                    };
                    timer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to copy to clipboard: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void CopyRecommendations_Click(object sender, RoutedEventArgs e)
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
                Clipboard.SetText(recommendations);
                btnCopyRecommendations.Content = "\uE73E"; // Checkmark icon
                
                // Reset icon after 2 seconds
                var timer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(2)
                };
                timer.Tick += (s, args) =>
                {
                    btnCopyRecommendations.Content = "\uE8C8"; // Copy icon
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
