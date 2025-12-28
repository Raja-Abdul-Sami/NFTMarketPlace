using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class ChangePasswordForm : Form
    {
        private Panel mainPanel;
        private TextBox txtCurrentPassword;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnChange;
        private Button btnCancel;
        private Label lblError;
        private Label lblPasswordStrength;
        private ProgressBar progressStrength;

        public ChangePasswordForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ChangePasswordForm
            this.ClientSize = new Size(500, 500);
            this.Text = "Change Password - TriApex";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Main panel
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Paint += MainPanel_Paint;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "CHANGE PASSWORD";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(50, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Current password
            Label lblCurrentPassword = new Label();
            lblCurrentPassword.Text = "CURRENT PASSWORD";
            lblCurrentPassword.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCurrentPassword.ForeColor = UIHelper.TextPrimary;
            lblCurrentPassword.Size = new Size(200, 25);
            lblCurrentPassword.Location = new Point(50, 100);
            lblCurrentPassword.TextAlign = ContentAlignment.MiddleLeft;

            txtCurrentPassword = new TextBox();
            txtCurrentPassword.Size = new Size(400, 40);
            txtCurrentPassword.Location = new Point(50, 125);
            txtCurrentPassword.Font = new Font("Segoe UI", 11);
            txtCurrentPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtCurrentPassword.ForeColor = Color.White;
            txtCurrentPassword.BorderStyle = BorderStyle.FixedSingle;
            txtCurrentPassword.PasswordChar = '•';

            // New password
            Label lblNewPassword = new Label();
            lblNewPassword.Text = "NEW PASSWORD";
            lblNewPassword.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblNewPassword.ForeColor = UIHelper.TextPrimary;
            lblNewPassword.Size = new Size(200, 25);
            lblNewPassword.Location = new Point(50, 180);
            lblNewPassword.TextAlign = ContentAlignment.MiddleLeft;

            txtNewPassword = new TextBox();
            txtNewPassword.Size = new Size(400, 40);
            txtNewPassword.Location = new Point(50, 205);
            txtNewPassword.Font = new Font("Segoe UI", 11);
            txtNewPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtNewPassword.ForeColor = Color.White;
            txtNewPassword.BorderStyle = BorderStyle.FixedSingle;
            txtNewPassword.PasswordChar = '•';
            txtNewPassword.TextChanged += TxtNewPassword_TextChanged;

            // Password strength
            lblPasswordStrength = new Label();
            lblPasswordStrength.Text = "Password strength: Weak";
            lblPasswordStrength.Font = new Font("Segoe UI", 9);
            lblPasswordStrength.ForeColor = UIHelper.ErrorColor;
            lblPasswordStrength.Size = new Size(200, 20);
            lblPasswordStrength.Location = new Point(50, 250);
            lblPasswordStrength.TextAlign = ContentAlignment.MiddleLeft;

            progressStrength = new ProgressBar();
            progressStrength.Size = new Size(400, 5);
            progressStrength.Location = new Point(50, 275);
            progressStrength.Minimum = 0;
            progressStrength.Maximum = 100;
            progressStrength.Value = 0;
            progressStrength.ForeColor = UIHelper.ErrorColor;
            progressStrength.BackColor = Color.FromArgb(50, 50, 60);

            // Confirm password
            Label lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "CONFIRM NEW PASSWORD";
            lblConfirmPassword.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblConfirmPassword.ForeColor = UIHelper.TextPrimary;
            lblConfirmPassword.Size = new Size(200, 25);
            lblConfirmPassword.Location = new Point(50, 300);
            lblConfirmPassword.TextAlign = ContentAlignment.MiddleLeft;

            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Size = new Size(400, 40);
            txtConfirmPassword.Location = new Point(50, 325);
            txtConfirmPassword.Font = new Font("Segoe UI", 11);
            txtConfirmPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtConfirmPassword.ForeColor = Color.White;
            txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtConfirmPassword.PasswordChar = '•';
            txtConfirmPassword.TextChanged += TxtConfirmPassword_TextChanged;

            // Password requirements
            Label lblRequirements = new Label();
            lblRequirements.Text = "• At least 8 characters\n• Contains uppercase and lowercase letters\n• Contains at least one number\n• Contains at least one special character";
            lblRequirements.Font = new Font("Segoe UI", 9);
            lblRequirements.ForeColor = UIHelper.TextSecondary;
            lblRequirements.Size = new Size(400, 80);
            lblRequirements.Location = new Point(50, 370);
            lblRequirements.TextAlign = ContentAlignment.MiddleLeft;

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(400, 30);
            lblError.Location = new Point(50, 420);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Change button
            btnChange = new Button();
            btnChange.Text = "CHANGE PASSWORD";
            btnChange.Size = new Size(180, 45);
            btnChange.Location = new Point(80, 460);
            UIHelper.StyleButton(btnChange, true);
            btnChange.Click += BtnChange_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(180, 45);
            btnCancel.Location = new Point(280, 460);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblCurrentPassword);
            mainPanel.Controls.Add(txtCurrentPassword);
            mainPanel.Controls.Add(lblNewPassword);
            mainPanel.Controls.Add(txtNewPassword);
            mainPanel.Controls.Add(lblPasswordStrength);
            mainPanel.Controls.Add(progressStrength);
            mainPanel.Controls.Add(lblConfirmPassword);
            mainPanel.Controls.Add(txtConfirmPassword);
            mainPanel.Controls.Add(lblRequirements);
            mainPanel.Controls.Add(lblError);
            mainPanel.Controls.Add(btnChange);
            mainPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += ChangePasswordForm_KeyDown;
        }

        #region Event Handlers

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw gradient background
            Rectangle rect = new Rectangle(0, 0, mainPanel.Width, mainPanel.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(25, 25, 35),
                Color.FromArgb(20, 20, 28)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw border
            using (Pen pen = new Pen(UIHelper.GoldAccent, 2))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, mainPanel.Width - 3, mainPanel.Height - 3);
            }
        }

        private void TxtNewPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordStrength();
            ValidatePasswordMatch();
            ClearError();
        }

        private void TxtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePasswordMatch();
            ClearError();
        }

        private void BtnChange_Click(object sender, EventArgs e)
        {
            ChangePassword();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ChangePasswordForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChangePassword();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Business Logic

        private void UpdatePasswordStrength()
        {
            string password = txtNewPassword.Text;
            int score = CalculatePasswordStrength(password);

            progressStrength.Value = score;

            if (score < 30)
            {
                lblPasswordStrength.Text = "Password strength: Weak";
                lblPasswordStrength.ForeColor = UIHelper.ErrorColor;
                progressStrength.ForeColor = UIHelper.ErrorColor;
            }
            else if (score < 70)
            {
                lblPasswordStrength.Text = "Password strength: Fair";
                lblPasswordStrength.ForeColor = Color.Orange;
                progressStrength.ForeColor = Color.Orange;
            }
            else
            {
                lblPasswordStrength.Text = "Password strength: Strong";
                lblPasswordStrength.ForeColor = UIHelper.SuccessColor;
                progressStrength.ForeColor = UIHelper.SuccessColor;
            }
        }

        private int CalculatePasswordStrength(string password)
        {
            int score = 0;

            if (string.IsNullOrEmpty(password))
                return 0;

            // Length score
            if (password.Length >= 8) score += 25;
            if (password.Length >= 12) score += 15;

            // Complexity score
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[a-z]")) score += 10;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]")) score += 10;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]")) score += 10;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, "[^a-zA-Z0-9]")) score += 20;

            return Math.Min(score, 100);
        }

        private void ValidatePasswordMatch()
        {
            if (!string.IsNullOrEmpty(txtConfirmPassword.Text) &&
                txtNewPassword.Text != txtConfirmPassword.Text)
            {
                txtConfirmPassword.ForeColor = UIHelper.ErrorColor;
            }
            else
            {
                txtConfirmPassword.ForeColor = UIHelper.TextPrimary;
            }
        }

        private void ClearError()
        {
            lblError.Visible = false;
        }

        private void ShowError(string message)
        {
            lblError.Text = $"✗ {message}";
            lblError.Visible = true;
        }

        private bool ValidateInput()
        {
            // Validate current password
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text))
            {
                ShowError("Please enter your current password.");
                txtCurrentPassword.Focus();
                return false;
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                ShowError("Please enter a new password.");
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Text.Length < 8)
            {
                ShowError("New password must be at least 8 characters.");
                txtNewPassword.Focus();
                return false;
            }

            // Check password strength
            if (CalculatePasswordStrength(txtNewPassword.Text) < 30)
            {
                ShowError("Password is too weak. Please choose a stronger password.");
                txtNewPassword.Focus();
                return false;
            }

            // Validate password confirmation
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("New passwords do not match.");
                txtConfirmPassword.Focus();
                return false;
            }

            // Check if new password is same as current
            if (txtNewPassword.Text == txtCurrentPassword.Text)
            {
                ShowError("New password cannot be the same as current password.");
                txtNewPassword.Focus();
                return false;
            }

            return true;
        }

        private bool VerifyCurrentPassword()
        {
            string query = "SELECT COUNT(*) FROM Users WHERE UserID = @UserID AND Password = @Password";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                new SqlParameter("@Password", txtCurrentPassword.Text)
            };

            int count = Convert.ToInt32(DBHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        private void ChangePassword()
        {
            if (!ValidateInput())
                return;

            // Verify current password
            if (!VerifyCurrentPassword())
            {
                ShowError("Current password is incorrect.");
                txtCurrentPassword.Focus();
                txtCurrentPassword.SelectAll();
                return;
            }

            // Confirm password change
            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to change your password?",
                "Confirm Password Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                // Update password in database
                string query = "UPDATE Users SET Password = @NewPassword WHERE UserID = @UserID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@NewPassword", txtNewPassword.Text),
                    new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
                };

                int rowsAffected = DBHelper.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    // Record transaction
                    string transactionQuery = @"
                        INSERT INTO Transactions (UserID, Amount, TransactionType, Description)
                        VALUES (@UserID, 0, 'PASSWORD_CHANGE', 'Changed account password')";

                    SqlParameter[] transParams = new SqlParameter[]
                    {
                        new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
                    };

                    DBHelper.ExecuteNonQuery(transactionQuery, transParams);

                    UIHelper.ShowSuccess("Password changed successfully!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to change password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error changing password: {ex.Message}");
            }
        }

        #endregion
    }
}