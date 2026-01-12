using IMAPSender;
using Microsoft.Extensions.Logging;

namespace IMAPSender.Examples;

/// <summary>
/// Example usage of the IMAPSender library.
/// NOTE: These examples demonstrate the API but require a configured logger factory
/// from Microsoft.Extensions.Logging for the logging examples to compile.
/// </summary>
public static class UsageExamples
{
    /// <summary>
    /// Example 1: Simple plain text email
    /// </summary>
    public static void SendPlainTextEmail()
    {
        var sender = new EmailSender(
            smtpHost: "mail.example.com",
            smtpPort: 465,
            username: "your-email@example.com",
            password: "your-password"
        );

        sender.Send(
            from: "sender@example.com",
            to: "recipient@example.com",
            subject: "Plain Text Email",
            body: "This is a simple plain text email.",
            isHtml: false
        );

        Console.WriteLine("Plain text email sent successfully!");
    }

    /// <summary>
    /// Example 2: HTML email
    /// </summary>
    public static void SendHtmlEmail()
    {
        var sender = new EmailSender(
            smtpHost: "mail.example.com",
            smtpPort: 465,
            username: "your-email@example.com",
            password: "your-password"
        );

        var htmlBody = @"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; }
                    .header { background-color: #4CAF50; color: white; padding: 20px; }
                    .content { padding: 20px; }
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Welcome to IMAPSender</h1>
                </div>
                <div class='content'>
                    <p>This is an <strong>HTML</strong> email with <em>formatting</em>.</p>
                    <ul>
                        <li>Feature 1</li>
                        <li>Feature 2</li>
                        <li>Feature 3</li>
                    </ul>
                </div>
            </body>
            </html>
        ";

        sender.Send(
            from: "sender@example.com",
            to: "recipient@example.com",
            subject: "HTML Email with Styling",
            body: htmlBody,
            isHtml: true
        );

        Console.WriteLine("HTML email sent successfully!");
    }

    /// <summary>
    /// Example 3: Async email sending
    /// </summary>
    public static async Task SendEmailAsync()
    {
        var sender = new EmailSender(
            smtpHost: "mail.example.com",
            smtpPort: 465,
            username: "your-email@example.com",
            password: "your-password"
        );

        await sender.SendAsync(
            from: "sender@example.com",
            to: "recipient@example.com",
            subject: "Async Email",
            body: "This email was sent asynchronously.",
            isHtml: false
        );

        Console.WriteLine("Async email sent successfully!");
    }

    /// <summary>
    /// Example 4: Sending multiple emails
    /// </summary>
    public static async Task SendMultipleEmails()
    {
        var sender = new EmailSender(
            smtpHost: "mail.example.com",
            smtpPort: 465,
            username: "notifications@example.com",
            password: "your-password"
        );

        var recipients = new[]
        {
            "user1@example.com",
            "user2@example.com",
            "user3@example.com"
        };

        foreach (var recipient in recipients)
        {
            try
            {
                await sender.SendAsync(
                    from: "notifications@example.com",
                    to: recipient,
                    subject: "Batch Email Notification",
                    body: $"<h2>Hello!</h2><p>This is a batch email sent to {recipient}.</p>",
                    isHtml: true
                );

                Console.WriteLine($"? Email sent to {recipient}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Failed to send email to {recipient}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Example 5: Error handling
    /// </summary>
    public static async Task SendEmailWithErrorHandling()
    {
        try
        {
            var sender = new EmailSender(
                smtpHost: "mail.example.com",
                smtpPort: 465,
                username: "your-email@example.com",
                password: "your-password"
            );

            await sender.SendAsync(
                from: "sender@example.com",
                to: "recipient@example.com",
                subject: "Test Email",
                body: "Testing error handling.",
                isHtml: false
            );

            Console.WriteLine("Email sent successfully!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid argument: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// Example 6: Using CancellationToken
    /// </summary>
    public static async Task SendEmailWithCancellation()
    {
        var sender = new EmailSender(
            smtpHost: "mail.example.com",
            smtpPort: 465,
            username: "your-email@example.com",
            password: "your-password"
        );

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            await sender.SendAsync(
                from: "sender@example.com",
                to: "recipient@example.com",
                subject: "Email with Timeout",
                body: "This email will timeout after 30 seconds.",
                isHtml: false,
                cancellationToken: cts.Token
            );

            Console.WriteLine("Email sent within timeout!");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Email sending was cancelled or timed out.");
        }
    }
}
