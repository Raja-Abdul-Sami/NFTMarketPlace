using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class EditProfileForm : Form
    {
        private Panel mainPanel;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private Button btnSave;
        private Button btnCancel;
        private Label lblError;

        public EditProfileForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
            LoadCurrentUserData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // EditProfileForm
            this.ClientSize = new Size(500, 400);
            this.Text = "Edit Profile - TriApex";
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
            lblTitle.Text = "EDIT PROFILE";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(50, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Username
            Label lblUsername = new Label();
            lblUsername.Text = "USERNAME";
            lblUsername.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblUsername.ForeColor = UIHelper.TextPrimary;
            lblUsername.Size = new Size(200, 25);
            lblUsername.Location = new Point(50, 100);
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;

            txtUsername = new TextBox();
            txtUsername.Size = new Size(400, 40);
            txtUsername.Location = new Point(50, 125);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BackColor = Color.FromArgb(40, 40, 50);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.MaxLength = 50;

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "EMAIL ADDRESS";
            lblEmail.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEmail.ForeColor = UIHelper.TextPrimary;
            lblEmail.Size = new Size(200, 25);
            lblEmail.Location = new Point(50, 180);
            lblEmail.TextAlign = ContentAlignment.MiddleLeft;

            txtEmail = new TextBox();
            txtEmail.Size = new Size(400, 40);
            txtEmail.Location = new Point(50, 205);
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.BackColor = Color.FromArgb(40, 40, 50);
            txtEmail.ForeColor = Color.White;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;

            // Current user info
            Label lblCurrentInfo = new Label();
            lblCurrentInfo.Text = $"Current user: {SessionManager.Instance.Username}";
            lblCurrentInfo.Font = new Font("Segoe UI", 10);
            lblCurrentInfo.ForeColor = UIHelper.TextSecondary;
            lblCurrentInfo.Size = new Size(400, 30);
            lblCurrentInfo.Location = new Point(50, 255);
            lblCurrentInfo.TextAlign = ContentAlignment.MiddleCenter;

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(400, 30);
            lblError.Location = new Point(50, 290);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Save button
            btnSave = new Button();
            btnSave.Text = "SAVE CHANGES";
            btnSave.Size = new Size(180, 45);
            btnSave.Location = new Point(80, 330);
            UIHelper.StyleButton(btnSave, true);
            btnSave.Click += BtnSave_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(180, 45);
            btnCancel.Location = new Point(280, 330);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblUsername);
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(lblEmail);
            mainPanel.Controls.Add(txtEmail);
            mainPanel.Controls.Add(lblCurrentInfo);
            mainPanel.Controls.Add(lblError);
            mainPanel.Controls.Add(btnSave);
            mainPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += EditProfileForm_KeyDown;
        }

        private void LoadCurrentUserData()
        {
            txtUsername.Text = SessionManager.Instance.Username;
            txtEmail.Text = SessionManager.Instance.Email;
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveProfileChanges();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void EditProfileForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveProfileChanges();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Business Logic

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
            // Validate username
            string newUsername = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(newUsername))
            {
                ShowError("Please enter a username.");
                txtUsername.Focus();
                return false;
            }

            if (newUsername.Length < 3)
            {
                ShowError("Username must be at least 3 characters.");
                txtUsername.Focus();
                return false;
            }

            if (newUsername.Length > 50)
            {
                ShowError("Username cannot exceed 50 characters.");
                txtUsername.Focus();
                return false;
            }

            // Validate email
            string newEmail = txtEmail.Text.Trim();
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(newEmail);
                    if (addr.Address != newEmail)
                    {
                        ShowError("Please enter a valid email address.");
                        txtEmail.Focus();
                        return false;
                    }
                }
                catch
                {
                    ShowError("Please enter a valid email address.");
                    txtEmail.Focus();
                    return false;
                }
            }

            // Check if username already exists (if changed)
            if (newUsername != SessionManager.Instance.Username)
            {
                if (DBHelper.UserExists(newUsername))
                {
                    ShowError("Username already exists. Please choose another.");
                    txtUsername.Focus();
                    txtUsername.SelectAll();
                    return false;
                }
            }

            return true;
        }

        private void SaveProfileChanges()
        {
            if (!ValidateInput())
                return;

            string newUsername = txtUsername.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            // Check if anything actually changed
            bool usernameChanged = newUsername != SessionManager.Instance.Username;
            bool emailChanged = newEmail != SessionManager.Instance.Email;

            if (!usernameChanged && !emailChanged)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }

            // Confirm changes
            DialogResult confirm = MessageBox.Show(
                "Save profile changes?",
                "Confirm Changes",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                // Update user in database
                string query = @"
                    UPDATE Users 
                    SET Username = @Username, 
                        Email = @Email 
                    WHERE UserID = @UserID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", newUsername),
                    new SqlParameter("@Email", string.IsNullOrWhiteSpace(newEmail) ? DBNull.Value : (object)newEmail),
                    new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
                };

                int rowsAffected = DBHelper.ExecuteNonQuery(query, parameters);

                if (rowsAffected > 0)
                {
                    // Update session
                    if (usernameChanged || emailChanged)
                    {
                        // Always use InitializeSession to update session values
                        SessionManager.Instance.InitializeSession(
                            SessionManager.Instance.CurrentUserID,
                            newUsername,
                            SessionManager.Instance.Balance,
                            newEmail
                        );
                    }

                    // Record transaction
                    string transactionQuery = @"
                        INSERT INTO Transactions (UserID, Amount, TransactionType, Description)
                        VALUES (@UserID, 0, 'PROFILE_UPDATE', 'Updated profile information')";

                    SqlParameter[] transParams = new SqlParameter[]
                    {
                        new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
                    };

                    DBHelper.ExecuteNonQuery(transactionQuery, transParams);

                    UIHelper.ShowSuccess("Profile updated successfully!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ShowError("Failed to update profile. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error updating profile: {ex.Message}");
            }
        }

        #endregion
    }
}