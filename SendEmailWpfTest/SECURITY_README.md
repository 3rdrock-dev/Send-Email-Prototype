# Secure Password Storage - Email Sender App

## ? Implementation: Windows DPAPI (Data Protection API)

This application uses **Windows DPAPI** to securely store SMTP passwords - a built-in, native Windows encryption system.

### How It Works

1. **First Run**: Application prompts you to enter SMTP credentials
2. **Storage**: Password is encrypted using Windows DPAPI and stored locally
3. **Retrieval**: Password is securely decrypted when needed
4. **Per-User**: Credentials are encrypted per Windows user account

### Security Benefits

? **Built-in Encryption**: Uses Windows native DPAPI (same technology used by Chrome, Edge, Windows itself)  
? **No Plain Text**: Password never stored in configuration files  
? **Per-User Security**: Each Windows user has separate, inaccessible credentials  
? **OS-Level Protection**: Managed by Windows security infrastructure  
? **Modern & Compatible**: Fully compatible with .NET 10 and all Windows versions  
? **No External Dependencies**: Uses built-in .NET libraries only  

### Storage Location

Credentials are stored at:
```
%LocalAppData%\EmailSenderApp\smtp_credentials.dat
```

Example:
```
C:\Users\YourName\AppData\Local\EmailSenderApp\smtp_credentials.dat
```

### Managing Credentials

#### View Storage Location
The encrypted file is stored in your local app data folder. You can view the location:
- Press `Win + R`
- Type: `%LocalAppData%\EmailSenderApp`
- The file `smtp_credentials.dat` contains your encrypted credentials

#### Delete Credentials
- **Through File Explorer**: Delete `smtp_credentials.dat` from the location above
- **Through App**: Use the Settings window to change credentials
- **Programmatically**: Call `SecureCredentialService.DeleteCredential()`

### appsettings.json Configuration

```json
{
  "SmtpSettings": {
    "Host": "mail.example.com",
    "Port": 587,
    "Username": "your-email@example.com",
    "Password": ""  // Leave empty - stored encrypted locally
  },
  "EmailDefaults": {
    "FromAddress": "your-email@example.com",
    "FromName": "Your Name"
  }
}
```

**Important**: Remove any plain text passwords from `appsettings.json`

### Migration from Plain Text

If you have a plain text password in `appsettings.json`:

1. **Automatic Migration**: On first run, the app will:
   - Detect the plain text password
   - Encrypt and save it using DPAPI
   - Use the secure version going forward

2. **Manual Cleanup**: After migration, you should:
   - Remove the password from `appsettings.json`
   - Set `"Password": ""`

---

## Technical Details

### Windows DPAPI

DPAPI (Data Protection API) is Windows' built-in encryption system that:
- Uses your Windows login credentials as the encryption key
- Automatically handles key management
- Protects data even if the computer is stolen (requires your Windows password)
- Is the same technology used by:
  - Web browsers (Chrome, Edge, Firefox) for password storage
  - Windows Credential Manager
  - Many enterprise applications

### Encryption Scope

**CurrentUser Scope**: Credentials can only be decrypted by:
- The same Windows user account that encrypted them
- On the same computer

This means:
- Other users on the same computer **cannot** access your credentials
- You **cannot** copy the encrypted file to another computer and decrypt it
- If you reinstall Windows, you'll need to re-enter credentials

---

## Alternative Security Options

### Option 2: User Secrets (Development Only)

For development, use .NET User Secrets:

```bash
cd SendEmailWpfTest
dotnet user-secrets init
dotnet user-secrets set "SmtpSettings:Password" "your-password"
```

**Pros**: Easy for development  
**Cons**: Not suitable for production, desktop app distribution

### Option 3: Environment Variables

Set password via environment variable:

```powershell
[Environment]::SetEnvironmentVariable("SMTP_PASSWORD", "your-password", "User")
```

Update `ConfigurationHelper.cs`:
```csharp
.AddEnvironmentVariables()
```

**Pros**: Standard across platforms  
**Cons**: Visible in process environment, less secure

### Option 4: Azure Key Vault (Enterprise)

For enterprise deployments:

```csharp
.AddAzureKeyVault(new Uri("https://your-vault.vault.azure.net/"))
```

**Pros**: Enterprise-grade security, auditing, rotation  
**Cons**: Requires Azure subscription, internet connectivity

### Option 5: Hardware Security Module (HSM)

For maximum security, use hardware tokens:
- YubiKey
- TPM chip
- Smart cards

**Pros**: Physical security, tamper-proof  
**Cons**: Complex setup, requires hardware

---

## Security Best Practices

### ? DO

- ? Use DPAPI for desktop apps on Windows
- ? Never commit passwords to source control
- ? Use app-specific passwords (Gmail, Outlook)
- ? Implement least-privilege access
- ? Rotate credentials regularly
- ? Log authentication attempts
- ? Use TLS/SSL for SMTP connections

### ? DON'T

- ? Store passwords in plain text
- ? Hard-code passwords in source code
- ? Share credentials between users
- ? Use the same password for multiple services
- ? Commit appsettings.json with passwords
- ? Log passwords or password attempts

---

## Testing Secure Storage

### View Storage Location

```csharp
string location = SecureCredentialService.GetStorageLocation();
// Returns: C:\Users\YourName\AppData\Local\EmailSenderApp\smtp_credentials.dat
```

### Check if Credentials Exist

```csharp
bool exists = SecureCredentialService.CredentialExists();
```

### Setup New Credentials

```csharp
bool saved = SecureCredentialService.SaveCredential(
    "your-username@example.com",
    "your-password"
);
```

### Retrieve Password

```csharp
string? password = SecureCredentialService.GetPassword("your-username");
```

### Remove Credentials

```csharp
bool deleted = SecureCredentialService.DeleteCredential();
```

---

## Troubleshooting

### "Failed to save credentials"
- **Cause**: File system permissions issue
- **Solution**: Check that `%LocalAppData%` is writable

### "Password not found"
- **Cause**: Credentials not set up yet or file deleted
- **Solution**: Run the application and enter credentials when prompted

### "Failed to decrypt"
- **Cause**: File corrupted or Windows user profile changed
- **Solution**: Delete the encrypted file and re-enter credentials

### After Windows Reinstall
- **Issue**: Cannot decrypt old credentials
- **Cause**: DPAPI keys are tied to Windows installation
- **Solution**: Re-enter credentials (this is by design for security)

---

## Code Files

### Core Security Files
- `SecureCredentialService.cs` - DPAPI encryption/decryption wrapper
- `ConfigurationHelper.cs` - Configuration with secure password retrieval
- `SettingsWindow.xaml` - UI for all settings configuration
- `SettingsWindow.xaml.cs` - Settings dialog logic

### Configuration Files
- `appsettings.json` - Application settings (passwords removed)
- `AppSettings.cs` - Settings model

---

## Additional Resources

- [Windows DPAPI Documentation](https://docs.microsoft.com/en-us/dotnet/standard/security/how-to-use-data-protection)
- [ProtectedData Class](https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.protecteddata)
- [Data Protection in .NET](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/)
- [SMTP Security Best Practices](https://sendgrid.com/blog/smtp-security/)

---

## Advantages Over Windows Credential Manager

While Windows Credential Manager is excellent, using DPAPI directly offers:

? **No External Dependencies**: Pure .NET, no NuGet packages needed  
? **Full .NET 10 Compatibility**: Built-in to .NET, always compatible  
? **Simpler Deployment**: No need to worry about package compatibility  
? **Same Security Level**: Uses the exact same encryption as Credential Manager  
? **More Control**: Direct access to encrypted data location  

---

## License & Support

This security implementation is provided as-is. Review and test thoroughly before production use.

For questions or issues, consult your security team or IT department.
