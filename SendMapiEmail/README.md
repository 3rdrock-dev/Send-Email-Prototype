# Email Sender - SOLID Architecture

This project demonstrates a well-architected email sending application using SOLID principles and modern C# 14 features.

## Architecture Overview

### SOLID Principles Applied

#### 1. **Single Responsibility Principle (SRP)**
Each class has one clear responsibility:
- `SmtpConfiguration`: Holds and validates SMTP configuration
- `SmtpEmailService`: Handles email sending via SMTP
- `EmailMessageBuilder`: Builds MimeMessage objects
- `Program`: Application entry point and DI configuration

#### 2. **Open/Closed Principle (OCP)**
The system is open for extension but closed for modification:
- `IEmailService` interface allows different email implementations (SMTP, SendGrid, etc.)
- `ISmtpConfiguration` interface allows different configuration sources

#### 3. **Liskov Substitution Principle (LSP)**
Any implementation of `IEmailService` can replace `SmtpEmailService` without breaking the application.

#### 4. **Interface Segregation Principle (ISP)**
Interfaces are focused and specific:
- `IEmailService`: Only defines email sending operations
- `ISmtpConfiguration`: Only defines SMTP configuration properties

#### 5. **Dependency Inversion Principle (DIP)**
High-level modules depend on abstractions, not concretions:
- `Program` depends on `IEmailService`, not `SmtpEmailService`
- `SmtpEmailService` depends on `ISmtpConfiguration`, not `SmtpConfiguration`

## Modern C# 14 Features

### 1. **Primary Constructors**
```csharp
public class SmtpConfiguration(string host, int port, string username, string password) : ISmtpConfiguration
```

### 2. **Collection Expressions**
```csharp
var recipients = new[] { "jim_barber@outlook.com", "admin@nvadi.com" };
```

### 3. **Pattern Matching with `is`**
```csharp
public int Port { get; } = port is > 0 and <= 65535 ? port : throw new ArgumentOutOfRangeException(...);
```

### 4. **Async/Await Throughout**
All I/O operations use async patterns for better performance and scalability.

### 5. **Nullable Reference Types**
Enabled throughout the project for better null safety.

## Project Structure

```
SendIMAPEmail/
??? Program.cs                    // Entry point with DI setup
??? IEmailService.cs             // Email service interface
??? SmtpEmailService.cs          // SMTP implementation
??? ISmtpConfiguration.cs        // Configuration interface
??? SmtpConfiguration.cs         // Configuration implementation with validation
??? EmailMessageBuilder.cs       // Message builder with fluent API
```

## Dependency Injection

The application uses Microsoft.Extensions.DependencyInjection for IoC:

```csharp
services.AddLogging(builder => builder.AddConsole());
services.AddSingleton<ISmtpConfiguration>(new SmtpConfiguration(...));
services.AddTransient<IEmailService, SmtpEmailService>();
```

## Benefits of This Architecture

1. **Testability**: Easy to mock interfaces for unit testing
2. **Maintainability**: Clear separation of concerns
3. **Extensibility**: Easy to add new email providers (SendGrid, AWS SES, etc.)
4. **Configuration**: Easy to swap configuration sources (appsettings.json, environment variables, etc.)
5. **Logging**: Built-in logging infrastructure with Microsoft.Extensions.Logging
6. **Async**: Non-blocking I/O operations for better performance

## Usage

### Basic Usage
```bash
dotnet run
```

### With Custom Subject and Body
```bash
dotnet run "Custom Subject" "Custom email body text"
```

## Future Enhancements

1. **Configuration from appsettings.json**
2. **Email templates with Razor or Handlebars**
3. **Attachment support**
4. **HTML email support**
5. **Retry policies with Polly**
6. **Email queue with background workers**
7. **Multiple provider support (SendGrid, AWS SES, etc.)**
