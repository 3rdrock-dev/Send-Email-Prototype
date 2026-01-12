# IMAPSender - Email Sending Class Library

A robust, SOLID-principled .NET class library for sending emails via SMTP with support for both plain text and HTML content.

## Features

- ? Send plain text or HTML emails
- ? Custom sender email address support
- ? Async/await support for better performance
- ? Built on MailKit for reliable email delivery
- ? SOLID architecture for easy testing and maintenance
- ? Optional logging support via Microsoft.Extensions.Logging
- ? SSL/TLS encryption support
- ? Input validation and error handling
- ? .NET 10 compatible

## Installation

Add the IMAPSender library to your project:

```bash
dotnet add reference path/to/IMAPSender/IMAPSender.csproj
```

## Quick Start

### Simple Usage (Synchronous)

```csharp
using IMAPSender;

var sender = new EmailSender(
    smtpHost: "mail.example.com",
    smtpPort: 465,
    username: "your-email@example.com",
    password: "your-password"
);

// Send plain text email
sender.Send(
    from: "sender@example.com",
    to: "recipient@example.com",
    subject: "Hello from IMAPSender",
    body: "This is a plain text email.",
    isHtml: false
);

// Send HTML email
sender.Send(
    from: "sender@example.com",
    to: "recipient@example.com",
    subject: "HTML Email",
    body: "<h1>Hello!</h1><p>This is an <strong>HTML</strong> email.</p>",
    isHtml: true
);
```

### Async Usage

```csharp
using IMAPSender;

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
    body: "This email was sent asynchronously!",
    isHtml: false
);
```

## Advanced Usage

### With Logging

```csharp
using IMAPSender;
using Microsoft.Extensions.Logging;

// Create logger
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
var logger = loggerFactory.CreateLogger<SmtpEmailService>();

// Create sender with logging
var sender = new EmailSender(
    smtpHost: "mail.example.com",
    smtpPort: 465,
    username: "your-email@example.com",
    password: "your-password",
    logger: logger
);

await sender.SendAsync(
    from: "sender@example.com",
    to: "recipient@example.com",
    subject: "Test",
    body: "This email includes diagnostic logging.",
    isHtml: false
);
```

### Using Dependency Injection

```csharp
using IMAPSender;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register SMTP configuration
services.AddSingleton<ISmtpConfiguration>(new SmtpConfiguration(
    host: "mail.example.com",
    port: 465,
    username: "your-email@example.com",
    password: "your-password"
));

// Register email service
services.AddTransient<IEmailService, SmtpEmailService>();

// Add logging
services.AddLogging(builder => builder.AddConsole());

var serviceProvider = services.BuildServiceProvider();
var emailService = serviceProvider.GetRequiredService<IEmailService>();

await emailService.SendAsync(
    from: "sender@example.com",
    to: "recipient@example.com",
    subject: "DI Email",
    body: "Sent using dependency injection!",
    isHtml: false
);
```

## API Reference

### EmailSender Class

The main facade class for sending emails.

#### Constructors

```csharp
// Simple constructor
public EmailSender(string smtpHost, int smtpPort, string username, string password, ILogger<SmtpEmailService>? logger = null)

// Advanced constructor with custom email service
public EmailSender(IEmailService emailService)
```

#### Methods

```csharp
// Synchronous send
public void Send(string from, string to, string subject, string body, bool isHtml = false)

// Asynchronous send
public async Task SendAsync(string from, string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
```

### Parameters

- **from** (string): Sender email address
- **to** (string): Recipient email address
- **subject** (string): Email subject line
- **body** (string): Email body content (plain text or HTML)
- **isHtml** (bool): Set to `true` for HTML emails, `false` for plain text (default: `false`)

## Common SMTP Ports

- **465**: SMTP over SSL/TLS (implicit SSL) - Recommended
- **587**: SMTP with STARTTLS (explicit TLS)
- **25**: SMTP (unencrypted) - Not recommended

## Architecture

The library follows SOLID principles:

### Components

1. **IEmailService** - Interface for email sending operations
2. **SmtpEmailService** - MailKit-based SMTP implementation
3. **ISmtpConfiguration** - SMTP configuration interface
4. **SmtpConfiguration** - SMTP configuration with validation
5. **EmailMessageBuilder** - Fluent builder for creating email messages
6. **EmailSender** - Simple facade for easy consumption

### Design Patterns Used

- **Facade Pattern**: `EmailSender` provides a simple API
- **Builder Pattern**: `EmailMessageBuilder` for constructing messages
- **Dependency Injection**: Interface-based design for testability
- **Strategy Pattern**: `IEmailService` allows different implementations

## Error Handling

The library validates inputs and throws appropriate exceptions:

- `ArgumentException` - For null or empty required parameters
- `ArgumentOutOfRangeException` - For invalid port numbers
- `SmtpCommandException` - For SMTP protocol errors
- `AuthenticationException` - For authentication failures

## Example: Sending Multiple Emails

```csharp
using IMAPSender;

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
    await sender.SendAsync(
        from: "notifications@example.com",
        to: recipient,
        subject: "Weekly Newsletter",
        body: "<h1>This Week's Updates</h1><p>Check out what's new!</p>",
        isHtml: true
    );
    
    Console.WriteLine($"Email sent to {recipient}");
}
```

## Testing

The library is designed for easy unit testing:

```csharp
// Mock the IEmailService interface
var mockEmailService = new Mock<IEmailService>();
mockEmailService
    .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), 
                           It.IsAny<string>(), It.IsAny<string>(), 
                           It.IsAny<bool>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(Task.CompletedTask);

var sender = new EmailSender(mockEmailService.Object);

// Test your code
await sender.SendAsync("from@test.com", "to@test.com", "Test", "Body");

// Verify
mockEmailService.Verify(x => x.SendAsync(
    "from@test.com", "to@test.com", "Test", "Body", false, 
    It.IsAny<CancellationToken>()), Times.Once);
```

## Requirements

- .NET 10 or higher
- MailKit 4.14.1 or higher
- Microsoft.Extensions.Logging.Abstractions 9.0.0 or higher (for logging support)

## License

This library uses MailKit which is licensed under the MIT License.

## Support

For issues, questions, or contributions, please refer to the project repository.
