using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SendIMAPEmail;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var subject = args.Length > 0 ? args[0] : "Test Email";
        var body = args.Length > 1 ? args[1] : "This is a test email sent using IMAPSender library.";
        var isHtml = args.Length > 2 && args[2].Equals("html", StringComparison.OrdinalIgnoreCase);

        Console.WriteLine("Email Sender Started...");
        Console.WriteLine($"Content Type: {(isHtml ? "HTML" : "Plain Text")}");
        Console.WriteLine();

        // Using the IMAPSender library directly
        var sender = new IMAPSender.EmailSender(
            smtpHost: "mail5018.site4now.net",
            smtpPort: 465,
            username: "support@nvadi.com",
            password: "Kx13013987!$!"
        );

        try
        {
            var recipients = new[] { "jim_barber@outlook.com", "admin@nvadi.com" };

            foreach (var recipient in recipients)
            {
                await sender.SendAsync(
                    from: "support@nvadi.com",
                    to: recipient,
                    subject: subject,
                    body: body,
                    isHtml: isHtml
                );
                
                Console.WriteLine($"✓ Email sent to {recipient} successfully.");
                Console.WriteLine();
            }

            Console.WriteLine("All emails sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error sending email: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
