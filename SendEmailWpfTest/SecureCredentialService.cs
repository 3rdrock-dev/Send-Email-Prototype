using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SendEmailWpfTest
{
    /// <summary>
    /// Provides secure credential storage and retrieval using Windows DPAPI (Data Protection API).
    /// Credentials are encrypted per-user and stored locally.
    /// </summary>
    public static class SecureCredentialService
    {
        private static readonly string CredentialFileName = "smtp_credentials.dat";
        private static readonly string CredentialDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EmailSenderApp");

        private static string CredentialFilePath => Path.Combine(CredentialDirectory, CredentialFileName);

        /// <summary>
        /// Saves SMTP credentials securely using Windows DPAPI.
        /// </summary>
        /// <param name="username">SMTP username</param>
        /// <param name="password">SMTP password</param>
        /// <returns>True if saved successfully</returns>
        public static bool SaveCredential(string username, string password)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(CredentialDirectory);

                // Combine username and password with a separator
                string credentials = $"{username}|{password}";
                byte[] dataBytes = Encoding.UTF8.GetBytes(credentials);

                // Encrypt using Windows DPAPI (CurrentUser scope)
                byte[] encryptedData = ProtectedData.Protect(
                    dataBytes,
                    null, // Optional entropy (additional password)
                    DataProtectionScope.CurrentUser);

                // Save to file
                File.WriteAllBytes(CredentialFilePath, encryptedData);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save credential: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves SMTP password from secure storage.
        /// </summary>
        /// <param name="username">SMTP username to match</param>
        /// <returns>Password if found and username matches, null otherwise</returns>
        public static string? GetPassword(string username)
        {
            try
            {
                if (!File.Exists(CredentialFilePath))
                {
                    return null;
                }

                // Read encrypted data
                byte[] encryptedData = File.ReadAllBytes(CredentialFilePath);

                // Decrypt using Windows DPAPI
                byte[] decryptedData = ProtectedData.Unprotect(
                    encryptedData,
                    null, // Same entropy as used in Protect
                    DataProtectionScope.CurrentUser);

                string credentials = Encoding.UTF8.GetString(decryptedData);
                string[] parts = credentials.Split('|');

                if (parts.Length == 2 && parts[0] == username)
                {
                    return parts[1];
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to retrieve credential: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if credentials exist in secure storage.
        /// </summary>
        /// <returns>True if credentials are stored</returns>
        public static bool CredentialExists()
        {
            return File.Exists(CredentialFilePath);
        }

        /// <summary>
        /// Deletes stored credentials.
        /// </summary>
        /// <returns>True if deleted successfully</returns>
        public static bool DeleteCredential()
        {
            try
            {
                if (File.Exists(CredentialFilePath))
                {
                    File.Delete(CredentialFilePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete credential: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the storage location for diagnostics/troubleshooting.
        /// </summary>
        public static string GetStorageLocation()
        {
            return CredentialFilePath;
        }
    }
}
