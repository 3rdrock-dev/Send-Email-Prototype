namespace SendEmailTest
{
    public partial class MainForm : Form
    {
        private IMAPSender.EmailSender? _emailSender;
        private AppSettings? _appSettings;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Load and display settings
            try
            {
                _appSettings = ConfigurationHelper.GetAppSettings();
                
                // Display SMTP settings in labels
                lblHostValue.Text = _appSettings.SmtpSettings.Host;
                lblPortValue.Text = _appSettings.SmtpSettings.Port.ToString();
                lblUsernameValue.Text = _appSettings.SmtpSettings.Username;
                
                // Mask password for security (show asterisks if password is set)
                lblPasswordValue.Text = string.IsNullOrEmpty(_appSettings.SmtpSettings.Password) 
                    ? "(not set)" 
                    : new string('*', _appSettings.SmtpSettings.Password.Length);
                
                // Update form title
                this.Text = $"Email Sender - {_appSettings.SmtpSettings.Host}";

                // Initialize email sender
                _emailSender = new IMAPSender.EmailSender(
                    _appSettings.SmtpSettings.Host,
                    _appSettings.SmtpSettings.Port,
                    _appSettings.SmtpSettings.Username,
                    _appSettings.SmtpSettings.Password
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading configuration: {ex.Message}", 
                    "Configuration Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                
                // Show error in labels
                lblHostValue.Text = "Error loading settings";
                lblPortValue.Text = "-";
                lblUsernameValue.Text = "-";
                lblPasswordValue.Text = "-";
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtToEmail.Text))
            {
                MessageBox.Show("Please enter a recipient email address.", 
                    "Validation Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                txtToEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Please enter a subject.", 
                    "Validation Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                txtSubject.Focus();
                return;
            }

            if (_emailSender == null || _appSettings == null)
            {
                MessageBox.Show("Email sender is not initialized. Please check your configuration.", 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                return;
            }

            // Disable send button to prevent double-clicking
            btnSend.Enabled = false;
            btnSend.Text = "Sending...";
            Cursor = Cursors.WaitCursor;

            try
            {
                await _emailSender.SendAsync(
                    from: _appSettings.EmailDefaults.FromAddress,
                    to: txtToEmail.Text.Trim(),
                    subject: txtSubject.Text.Trim(),
                    body: txtBody.Text,
                    isHtml: chkIsHtml.Checked
                );

                MessageBox.Show($"Email sent successfully to {txtToEmail.Text}!", 
                    "Success", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);

                // Clear form after successful send
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}", 
                    "Send Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable send button
                btnSend.Enabled = true;
                btnSend.Text = "Send Email";
                Cursor = Cursors.Default;
            }
        }

        private void ClearForm()
        {
            txtToEmail.Clear();
            txtSubject.Clear();
            txtBody.Clear();
            chkIsHtml.Checked = false;
            txtToEmail.Focus();
        }
    }
}
