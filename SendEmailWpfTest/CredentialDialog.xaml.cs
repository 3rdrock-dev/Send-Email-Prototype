using System.Windows;

namespace SendEmailWpfTest
{
    /// <summary>
    /// Interaction logic for CredentialDialog.xaml
    /// </summary>
    public partial class CredentialDialog : Window
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public bool CredentialsSaved { get; private set; }

        public CredentialDialog()
        {
            InitializeComponent();
        }

        public CredentialDialog(string defaultUsername) : this()
        {
            txtUsername.Text = defaultUsername;
            txtPassword.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter a username.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Please enter a password.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            Username = txtUsername.Text.Trim();
            Password = txtPassword.Password;

            // Save to Windows Credential Manager
            if (SecureCredentialService.SaveCredential(Username, Password))
            {
                CredentialsSaved = true;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Failed to save credentials to Windows Credential Manager.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
