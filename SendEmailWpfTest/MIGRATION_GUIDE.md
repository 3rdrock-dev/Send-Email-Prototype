# Migration from CredentialManagement to DPAPI

## What Changed?

We've replaced the `CredentialManagement` NuGet package (which had compatibility warnings with .NET 10) with **native Windows DPAPI** (Data Protection API).

## Benefits

? **Fully Compatible**: Native .NET 10 support, no compatibility warnings  
? **No External Dependencies**: Uses built-in `System.Security.Cryptography`  
? **Same Security Level**: DPAPI is what Credential Manager uses internally  
? **Simpler**: No third-party packages to maintain  
? **Future-Proof**: Built into Windows and .NET, always compatible  

## Storage Location Change

### Before (CredentialManagement):
- Stored in: Windows Credential Manager
- View with: `control /name Microsoft.CredentialManager`
- Target: `EmailSenderApp_SMTP`

### After (DPAPI):
- Stored in: `%LocalAppData%\EmailSenderApp\smtp_credentials.dat`
- View with: `explorer %LocalAppData%\EmailSenderApp`
- File: Encrypted binary file

## What You Need to Do

### If You Haven't Run the App Yet
**Nothing!** Just run the app and it will work.

### If You Already Saved Credentials
Your app will continue to work! On first run after this update:
1. The app will prompt for credentials (old ones are in Credential Manager)
2. Enter your credentials
3. They'll be saved using the new DPAPI method
4. Old credentials in Credential Manager are no longer used (you can delete them manually)

### To Clean Up Old Credentials (Optional)
1. Press `Win + R`
2. Type: `control /name Microsoft.CredentialManager`
3. Click "Windows Credentials"
4. Find and remove: `EmailSenderApp_SMTP`

## Security Comparison

Both methods are equally secure:

| Feature | Credential Manager | DPAPI (New) |
|---------|-------------------|-------------|
| Encryption | Windows DPAPI | Windows DPAPI |
| Per-User | ? | ? |
| OS-Level Protection | ? | ? |
| Backup/Restore | With Windows backup | With Windows backup |
| .NET 10 Compatible | ?? (warnings) | ? (native) |
| External Dependencies | CredentialManagement NuGet | None |

## Technical Details

### Old Implementation
```csharp
using CredentialManagement; // NuGet package
var credential = new Credential { Target = "EmailSenderApp_SMTP", ... };
credential.Save();
```

### New Implementation
```csharp
using System.Security.Cryptography; // Built-in
byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
File.WriteAllBytes(path, encrypted);
```

Both use the same underlying Windows DPAPI encryption technology.

## Troubleshooting

### "Credentials not found" after update
- **Expected**: Old credentials are still in Credential Manager
- **Solution**: Re-enter credentials when prompted

### Where is my encrypted file?
```
C:\Users\YourUsername\AppData\Local\EmailSenderApp\smtp_credentials.dat
```

### Can I still use Credential Manager?
The app no longer uses Credential Manager. The new method:
- Is simpler to deploy (no external packages)
- Has better .NET 10 compatibility
- Uses the same encryption (DPAPI)

### What if I want to go back?
The old `CredentialManagement` package had compatibility warnings. DPAPI is the recommended approach for:
- .NET 10 applications
- Future-proof compatibility
- No external dependencies

## Questions?

See `SECURITY_README.md` for detailed security documentation.
