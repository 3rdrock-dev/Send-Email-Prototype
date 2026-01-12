namespace SendEmailTest
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblHostLabel = new Label();
            lblHostValue = new Label();
            lblPortLabel = new Label();
            lblPortValue = new Label();
            lblUsernameLabel = new Label();
            lblUsernameValue = new Label();
            lblPasswordLabel = new Label();
            lblPasswordValue = new Label();
            grpEmailCompose = new GroupBox();
            chkIsHtml = new CheckBox();
            txtBody = new TextBox();
            lblBody = new Label();
            txtSubject = new TextBox();
            lblSubject = new Label();
            txtToEmail = new TextBox();
            lblToEmail = new Label();
            btnSend = new Button();
            grpEmailCompose.SuspendLayout();
            SuspendLayout();
            // 
            // lblHostLabel
            // 
            lblHostLabel.AutoSize = true;
            lblHostLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblHostLabel.Location = new Point(12, 15);
            lblHostLabel.Name = "lblHostLabel";
            lblHostLabel.Size = new Size(38, 15);
            lblHostLabel.TabIndex = 0;
            lblHostLabel.Text = "Host:";
            // 
            // lblHostValue
            // 
            lblHostValue.AutoSize = true;
            lblHostValue.Location = new Point(100, 15);
            lblHostValue.Name = "lblHostValue";
            lblHostValue.Size = new Size(0, 15);
            lblHostValue.TabIndex = 1;
            // 
            // lblPortLabel
            // 
            lblPortLabel.AutoSize = true;
            lblPortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPortLabel.Location = new Point(12, 40);
            lblPortLabel.Name = "lblPortLabel";
            lblPortLabel.Size = new Size(34, 15);
            lblPortLabel.TabIndex = 2;
            lblPortLabel.Text = "Port:";
            // 
            // lblPortValue
            // 
            lblPortValue.AutoSize = true;
            lblPortValue.Location = new Point(100, 40);
            lblPortValue.Name = "lblPortValue";
            lblPortValue.Size = new Size(0, 15);
            lblPortValue.TabIndex = 3;
            // 
            // lblUsernameLabel
            // 
            lblUsernameLabel.AutoSize = true;
            lblUsernameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsernameLabel.Location = new Point(12, 65);
            lblUsernameLabel.Name = "lblUsernameLabel";
            lblUsernameLabel.Size = new Size(70, 15);
            lblUsernameLabel.TabIndex = 4;
            lblUsernameLabel.Text = "User Name:";
            // 
            // lblUsernameValue
            // 
            lblUsernameValue.AutoSize = true;
            lblUsernameValue.Location = new Point(100, 65);
            lblUsernameValue.Name = "lblUsernameValue";
            lblUsernameValue.Size = new Size(0, 15);
            lblUsernameValue.TabIndex = 5;
            // 
            // lblPasswordLabel
            // 
            lblPasswordLabel.AutoSize = true;
            lblPasswordLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPasswordLabel.Location = new Point(12, 90);
            lblPasswordLabel.Name = "lblPasswordLabel";
            lblPasswordLabel.Size = new Size(62, 15);
            lblPasswordLabel.TabIndex = 6;
            lblPasswordLabel.Text = "Password:";
            // 
            // lblPasswordValue
            // 
            lblPasswordValue.AutoSize = true;
            lblPasswordValue.Location = new Point(100, 90);
            lblPasswordValue.Name = "lblPasswordValue";
            lblPasswordValue.Size = new Size(0, 15);
            lblPasswordValue.TabIndex = 7;
            // 
            // grpEmailCompose
            // 
            grpEmailCompose.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpEmailCompose.Controls.Add(chkIsHtml);
            grpEmailCompose.Controls.Add(txtBody);
            grpEmailCompose.Controls.Add(lblBody);
            grpEmailCompose.Controls.Add(txtSubject);
            grpEmailCompose.Controls.Add(lblSubject);
            grpEmailCompose.Controls.Add(txtToEmail);
            grpEmailCompose.Controls.Add(lblToEmail);
            grpEmailCompose.Location = new Point(12, 120);
            grpEmailCompose.Name = "grpEmailCompose";
            grpEmailCompose.Size = new Size(776, 280);
            grpEmailCompose.TabIndex = 8;
            grpEmailCompose.TabStop = false;
            grpEmailCompose.Text = "Compose Email";
            // 
            // chkIsHtml
            // 
            chkIsHtml.AutoSize = true;
            chkIsHtml.Location = new Point(88, 247);
            chkIsHtml.Name = "chkIsHtml";
            chkIsHtml.Size = new Size(86, 19);
            chkIsHtml.TabIndex = 6;
            chkIsHtml.Text = "HTML Body";
            chkIsHtml.UseVisualStyleBackColor = true;
            // 
            // txtBody
            // 
            txtBody.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBody.Location = new Point(88, 108);
            txtBody.Multiline = true;
            txtBody.Name = "txtBody";
            txtBody.ScrollBars = ScrollBars.Vertical;
            txtBody.Size = new Size(672, 133);
            txtBody.TabIndex = 5;
            // 
            // lblBody
            // 
            lblBody.AutoSize = true;
            lblBody.Location = new Point(16, 111);
            lblBody.Name = "lblBody";
            lblBody.Size = new Size(38, 15);
            lblBody.TabIndex = 4;
            lblBody.Text = "Body:";
            // 
            // txtSubject
            // 
            txtSubject.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSubject.Location = new Point(88, 69);
            txtSubject.Name = "txtSubject";
            txtSubject.Size = new Size(672, 23);
            txtSubject.TabIndex = 3;
            // 
            // lblSubject
            // 
            lblSubject.AutoSize = true;
            lblSubject.Location = new Point(16, 72);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(49, 15);
            lblSubject.TabIndex = 2;
            lblSubject.Text = "Subject:";
            // 
            // txtToEmail
            // 
            txtToEmail.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtToEmail.Location = new Point(88, 32);
            txtToEmail.Name = "txtToEmail";
            txtToEmail.Size = new Size(672, 23);
            txtToEmail.TabIndex = 1;
            // 
            // lblToEmail
            // 
            lblToEmail.AutoSize = true;
            lblToEmail.Location = new Point(16, 35);
            lblToEmail.Name = "lblToEmail";
            lblToEmail.Size = new Size(19, 15);
            lblToEmail.TabIndex = 0;
            lblToEmail.Text = "To:";
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSend.Location = new Point(676, 410);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(112, 32);
            btnSend.TabIndex = 9;
            btnSend.Text = "Send Email";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSend);
            Controls.Add(grpEmailCompose);
            Controls.Add(lblPasswordValue);
            Controls.Add(lblPasswordLabel);
            Controls.Add(lblUsernameValue);
            Controls.Add(lblUsernameLabel);
            Controls.Add(lblPortValue);
            Controls.Add(lblPortLabel);
            Controls.Add(lblHostValue);
            Controls.Add(lblHostLabel);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Email Sender";
            Load += Form1_Load;
            grpEmailCompose.ResumeLayout(false);
            grpEmailCompose.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblHostLabel;
        private Label lblHostValue;
        private Label lblPortLabel;
        private Label lblPortValue;
        private Label lblUsernameLabel;
        private Label lblUsernameValue;
        private Label lblPasswordLabel;
        private Label lblPasswordValue;
        private GroupBox grpEmailCompose;
        private TextBox txtToEmail;
        private Label lblToEmail;
        private TextBox txtSubject;
        private Label lblSubject;
        private TextBox txtBody;
        private Label lblBody;
        private CheckBox chkIsHtml;
        private Button btnSend;
    }
}
