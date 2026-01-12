# Send Email Prototype

**Professional email configuration and testing toolkit for Windows**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![WinUI 3](https://img.shields.io/badge/WinUI-3-0078D4?logo=windows)](https://microsoft.github.io/microsoft-ui-xaml/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A comprehensive Windows desktop application for testing and validating email server configurations with SMTP/IMAP support and DNS deliverability validation.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Building from Source](#building-from-source)
- [Configuration](#configuration)
- [Usage](#usage)
- [Projects](#projects)
- [Technology Stack](#technology-stack)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## 🎯 Overview

Send Email Prototype is a professional-grade email testing utility designed for IT administrators, developers, and email system administrators. It provides a modern WinUI 3 interface for:

- **SMTP/IMAP Testing**: Validate email server connectivity and authentication
- **Email Deliverability**: Check DNS records (SPF, DKIM, DMARC) for email domains
- **Secure Credential Storage**: Windows Data Protection API (DPAPI) integration
- **Real-time Diagnostics**: Monitor email delivery status and troubleshoot issues

---

## ✨ Features

### Core Functionality
- ✅ **SMTP Server Testing** - Test outbound email configuration
- ✅ **IMAP Server Testing** - Validate inbound email server settings
- ✅ **DNS Deliverability Checks** - Verify SPF, DKIM, and DMARC records
- ✅ **Secure Credential Storage** - Encrypted password storage using Windows DPAPI
- ✅ **Modern UI** - WinUI 3 with Fluent Design
- ✅ **MSIX Packaging** - Easy installation and deployment

### Technical Features
- Multi-protocol support (SMTP, IMAP, POP3)
- TLS/SSL encryption support
- Custom port configuration
- Detailed error reporting and logging
- Configuration import/export
- Portable settings storage

---

## 🏗️ Architecture

This solution consists of **5 projects**:

```
Send-Email-Prototype/
│
├── SendEmailWinUITest/              # Main WinUI 3 Desktop Application
│   ├── App.xaml                     # Application entry point
│   ├── MainWindow.xaml              # Primary UI
│   ├── DeliverabilityDialog.xaml   # DNS validation UI
│   └── ConfigurationHelper.cs      # Settings management
│
├── SendEmailWinUITest (Package)/    # MSIX Packaging Project
│   ├── Package.appxmanifest        # App manifest
│   └── Images/                     # Branding assets
│
├── SendEmailWpfTest/                # WPF Test Application
│   └── MainWindow.xaml             # WPF UI alternative
│
├── SendEmailTest/                   # Console Test Application
│   └── Program.cs                  # CLI test harness
│
└── IMAPSender/                      # IMAP Library (Multi-targeting)
    ├── IMAPHelper.cs               # IMAP functionality
    └── Targets: net8.0, net10.0    # Multi-framework support
```

---

## 📦 Prerequisites

### Development Requirements
- **Visual Studio 2022** (17.8 or later)
- **.NET 8.0 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **.NET 10.0 SDK** (optional, for multi-targeting)
- **Windows App SDK 1.8** (included via NuGet)
- **Windows 10 SDK (10.0.26100.0)** or later

### Runtime Requirements
- **Windows 10** version 1809 (Build 17763) or later
- **Windows 11** (recommended)
- **.NET 8.0 Runtime** (Desktop)
- **Windows App Runtime 1.8**

---

## 🚀 Installation

### Option 1: Install from MSIX Package (Recommended)

1. **Download** the latest release from [Releases](https://github.com/3rdrock-dev/Send-Email-Prototype/releases)
2. **Extract** the ZIP file
3. **Install the certificate** (first-time only):
   ```powershell
   Import-Certificate -FilePath "SendEmailWinUITest_TemporaryKey.cer" -CertStoreLocation "Cert:\CurrentUser\Root"
   ```
4. **Install the app**:
   ```powershell
   Add-AppxPackage -Path "SendEmailWinUITest (Package)_1.0.x.0_x64.msix"
   ```

### Option 2: Build from Source

See [Building from Source](#building-from-source) section below.

---

## 🔨 Building from Source

### Clone the Repository
```bash
git clone https://github.com/3rdrock-dev/Send-Email-Prototype.git
cd Send-Email-Prototype
```

### Build with Visual Studio
1. Open `Send Mapi Email Prototype.sln` in Visual Studio 2022
2. Set `SendEmailWinUITest (Package)` as the startup project
3. Select **Release | x64** configuration
4. Press **F5** or **Build → Rebuild Solution**

### Build with MSBuild (Command Line)
```powershell
# Restore NuGet packages
msbuild /t:Restore

# Build Release configuration
msbuild "Send Mapi Email Prototype.sln" /p:Configuration=Release /p:Platform=x64

# Create MSIX package
cd "SendEmailWinUITest\SendEmailWinUITest (Package)"
msbuild "SendEmailWinUITest (Package).wapproj" `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:AppxPackageDir="..\..\AppPackages\" `
  /p:AppxBundle=Never `
  /p:UapAppxPackageBuildMode=SideloadOnly `
  /t:Rebuild
```

### Package Output
The MSIX package will be created at:
```
AppPackages\SendEmailWinUITest (Package)_1.0.x.0_x64_Test\
└── SendEmailWinUITest (Package)_1.0.x.0_x64.msix
```

---

## ⚙️ Configuration

### Application Settings Location
Settings are stored at:
```
%LOCALAPPDATA%\SendEmailWinUITest\appsettings.json
```

### Configuration File Structure
```json
{
  "SmtpSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "user@example.com",
    "Password": ""
  },
  "EmailDefaults": {
    "FromAddress": "sender@example.com",
    "FromName": "Sender Name"
  }
}
```

### Secure Credential Storage
Passwords are encrypted using **Windows Data Protection API (DPAPI)**:
- User-scoped encryption (tied to Windows user account)
- Automatic decryption on the same machine/user
- Base64-encoded storage in configuration file

---

## 🎮 Usage

### Main Application (WinUI 3)

1. **Launch** the application from Start Menu: "Send Email Prototype"

2. **Configure Email Settings**:
   - Enter SMTP/IMAP server details
   - Configure ports (587 for SMTP, 993 for IMAP)
   - Enable TLS encryption (recommended)
   - Enter credentials

3. **Send Test Email**:
   - Click **"Send Email"** button
   - Monitor status in the output window

4. **Check Deliverability**:
   - Click **"Check Deliverability"** button
   - View DNS records (SPF, DKIM, DMARC)
   - Validate email domain configuration

### Console Application (SendEmailTest)

```bash
cd SendEmailTest\bin\Release\net8.0
SendEmailTest.exe
```

Reads configuration from `appsettings.json` in the executable directory.

### WPF Application (SendEmailWpfTest)

Alternative WPF-based UI for testing and legacy compatibility.

---

## 📁 Projects

### 1. SendEmailWinUITest
**Type**: WinUI 3 Desktop Application (.NET 8)

**Purpose**: Main user interface for email testing

**Key Components**:
- `App.xaml` - Application initialization
- `MainWindow.xaml` - Primary application window
- `DeliverabilityDialog.xaml` - DNS validation dialog
- `ConfigurationHelper.cs` - Settings management with DPAPI encryption

**Dependencies**:
- Microsoft.WindowsAppSDK 1.8
- MailKit 4.14.1
- DnsClient 1.8.0
- System.Security.Cryptography.ProtectedData 10.0.1

### 2. SendEmailWinUITest (Package)
**Type**: Windows Application Packaging Project

**Purpose**: MSIX packaging for deployment

**Contains**:
- `Package.appxmanifest` - Application manifest
- Branding assets (logos, splash screen)
- Certificate for code signing
- Installation scripts

### 3. SendEmailWpfTest
**Type**: WPF Desktop Application (.NET 8)

**Purpose**: Alternative WPF-based UI

**Features**:
- Traditional Windows Forms experience
- Legacy system compatibility
- Rapid prototyping and testing

### 4. SendEmailTest
**Type**: Console Application (.NET 8)

**Purpose**: Command-line testing and automation

**Features**:
- Batch email testing
- CI/CD integration
- Scriptable email validation

### 5. IMAPSender
**Type**: Class Library (Multi-targeting: .NET 8.0, .NET 10.0)

**Purpose**: IMAP protocol implementation

**Provides**:
- IMAP connection management
- Email retrieval functionality
- Mailbox operations
- Cross-framework compatibility

---

## 🛠️ Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Framework** | .NET | 8.0, 10.0 |
| **UI Framework** | WinUI 3 | 1.8 |
| **UI Framework (Alt)** | WPF | - |
| **Email Library** | MailKit | 4.14.1 |
| **DNS Client** | DnsClient | 1.8.0 |
| **Packaging** | MSIX | - |
| **Security** | Windows DPAPI | - |
| **Configuration** | Microsoft.Extensions.Configuration | 10.0.1 |

---

## 🐛 Troubleshooting

### Installation Issues

**Problem**: "Cannot open app package" error

**Solution**: Install the certificate first:
```powershell
Import-Certificate -FilePath "SendEmailWinUITest_TemporaryKey.cer" `
  -CertStoreLocation "Cert:\CurrentUser\Root"
```

---

**Problem**: "Package conflicts with an installed package" error

**Solution**: Uninstall the existing version:
```powershell
Get-AppxPackage -Name "*b999425f-96a3-4c24-a4a4-a1156755d635*" | Remove-AppxPackage
```

---

### Build Issues

**Problem**: Image duplication errors during build

**Solution**: The packaging project should contain branding assets. Ensure `SendEmailWinUITest.csproj` excludes:
```xml
<Content Remove="Images\StoreLogo.png" />
<Content Remove="Images\Square*.png" />
<Content Remove="Images\Wide*.png" />
<Content Remove="Images\SplashScreen*.png" />
```

---

**Problem**: Manifest parsing errors

**Solution**: Ensure `Package.appxmanifest` does NOT contain conflicting trust levels:
- Remove `uap18:TrustLevel="appContainer"` attributes
- Keep only `<rescap:Capability Name="runFullTrust" />`

---

### Runtime Issues

**Problem**: Email sending fails with SSL/TLS errors

**Solution**: 
- Verify TLS settings match server requirements
- Check firewall/antivirus blocking ports 587/993
- Ensure server supports STARTTLS

---

**Problem**: Credentials not saving

**Solution**: 
- Check Windows user account has DPAPI access
- Verify `%LOCALAPPDATA%` is writable
- Ensure appsettings.json exists and is valid JSON

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

### Development Workflow
1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request to `DevMain` branch

### Coding Standards
- Follow **C# Coding Conventions** ([Microsoft Guidelines](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions))
- Use **async/await** for I/O operations
- Add **XML documentation** for public APIs
- Include **unit tests** for new functionality

### Commit Message Format
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

**Example**:
```
feat(smtp): Add SMTP authentication timeout configuration

- Added SmtpTimeout property to EmailSettings
- Updated ConfigurationHelper to persist timeout value
- Added UI control for timeout configuration

Closes #123
```

---

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## 👥 Authors

**3rdRock Development**
- GitHub: [@3rdrock-dev](https://github.com/3rdrock-dev)
- Repository: [Send-Email-Prototype](https://github.com/3rdrock-dev/Send-Email-Prototype)

---

## 🙏 Acknowledgments

- **MailKit** by Jeffrey Stedfast - Excellent email protocol library
- **DnsClient.NET** - Robust DNS client implementation
- **Microsoft** - WinUI 3 and Windows App SDK
- **Community contributors** - Thank you for your support!

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/3rdrock-dev/Send-Email-Prototype/issues)
- **Discussions**: [GitHub Discussions](https://github.com/3rdrock-dev/Send-Email-Prototype/discussions)

---

## 🗺️ Roadmap

- [ ] OAuth 2.0 authentication support
- [ ] Multiple email account profiles
- [ ] Email template management
- [ ] Bulk email testing
- [ ] Export test results to CSV/JSON
- [ ] Scheduled email testing
- [ ] Dark/Light theme toggle
- [ ] Localization (i18n)
- [ ] Microsoft Store distribution

---

**Made with ❤️ by 3rdRock Development**
