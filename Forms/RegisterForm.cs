using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class RegisterForm : Form
    {
        // UI Components
        private Panel mainPanel;
        private Panel registerPanel;
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblPassword;
        private TextBox txtPassword;
        private Label lblConfirmPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnBackToLogin;
        private Label lblError;
        private Label lblSuccess;
        private PictureBox picLogo;
        private ProgressBar progressBar;
        private Label lblPasswordStrength;

        public RegisterForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // RegisterForm
            this.ClientSize = new Size(1000, 700);
            this.Text = "TriApex NFT Marketplace - Register";
            this.StartPosition = FormStartPosition.CenterScreen;
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

            // Logo
            picLogo = new PictureBox();
            picLogo.Size = new Size(150, 150);
            picLogo.Location = new Point(425, 30);
            picLogo.BackColor = Color.Transparent;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

            try
            {
                if (Properties.Resources.TriApexLogo != null)
                {
                    picLogo.Image = Properties.Resources.TriApexLogo;
                }
                else
                {
                    CreateTextLogo();
                }
            }
            catch
            {
                CreateTextLogo();
            }

            // Register panel
            registerPanel = new Panel();
            registerPanel.Size = new Size(450, 500);
            registerPanel.Location = new Point(275, 180);
            UIHelper.StylePanel(registerPanel, true);

            // Title label
            lblTitle = new Label();
            lblTitle.Text = "CREATE ACCOUNT";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(25, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Username label
            lblUsername = new Label();
            lblUsername.Text = "USERNAME";
            lblUsername.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsername.ForeColor = UIHelper.BlueAccent;
            lblUsername.Size = new Size(400, 25);
            lblUsername.Location = new Point(25, 80);
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;

            // Username textbox
            txtUsername = new TextBox();
            txtUsername.Size = new Size(400, 40);
            txtUsername.Location = new Point(25, 105);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BackColor = Color.FromArgb(40, 40, 50);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Padding = new Padding(10);
            txtUsername.TextChanged += TxtUsername_TextChanged;

            // Email label
            lblEmail = new Label();
            lblEmail.Text = "EMAIL";
            lblEmail.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblEmail.ForeColor = UIHelper.BlueAccent;
            lblEmail.Size = new Size(400, 25);
            lblEmail.Location = new Point(25, 155);
            lblEmail.TextAlign = ContentAlignment.MiddleLeft;

            // Email textbox
            txtEmail = new TextBox();
            txtEmail.Size = new Size(400, 40);
            txtEmail.Location = new Point(25, 180);
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.BackColor = Color.FromArgb(40, 40, 50);
            txtEmail.ForeColor = Color.White;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Padding = new Padding(10);

            // Password label
            lblPassword = new Label();
            lblPassword.Text = "PASSWORD";
            lblPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPassword.ForeColor = UIHelper.BlueAccent;
            lblPassword.Size = new Size(400, 25);
            lblPassword.Location = new Point(25, 230);
            lblPassword.TextAlign = ContentAlignment.MiddleLeft;

            // Password textbox
            txtPassword = new TextBox();
            txtPassword.Size = new Size(400, 40);
            txtPassword.Location = new Point(25, 255);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '•';
            txtPassword.Padding = new Padding(10);
            txtPassword.TextChanged += TxtPassword_TextChanged;

            // Password strength label
            lblPasswordStrength = new Label();
            lblPasswordStrength.Text = "Password strength: Weak";
            lblPasswordStrength.Font = new Font("Segoe UI", 8);
            lblPasswordStrength.ForeColor = UIHelper.ErrorColor;
            lblPasswordStrength.Size = new Size(200, 20);
            lblPasswordStrength.Location = new Point(25, 295);
            lblPasswordStrength.TextAlign = ContentAlignment.MiddleLeft;

            // Password strength progress bar
            progressBar = new ProgressBar();
            progressBar.Size = new Size(400, 5);
            progressBar.Location = new Point(25, 315);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.ForeColor = UIHelper.ErrorColor;
            progressBar.BackColor = Color.FromArgb(50, 50, 60);

            // Confirm password label
            lblConfirmPassword = new Label();
            lblConfirmPassword.Text = "CONFIRM PASSWORD";
            lblConfirmPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblConfirmPassword.ForeColor = UIHelper.BlueAccent;
            lblConfirmPassword.Size = new Size(400, 25);
            lblConfirmPassword.Location = new Point(25, 330);
            lblConfirmPassword.TextAlign = ContentAlignment.MiddleLeft;

            // Confirm password textbox
            txtConfirmPassword = new TextBox();
            txtConfirmPassword.Size = new Size(400, 40);
            txtConfirmPassword.Location = new Point(25, 355);
            txtConfirmPassword.Font = new Font("Segoe UI", 11);
            txtConfirmPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtConfirmPassword.ForeColor = Color.White;
            txtConfirmPassword.BorderStyle = BorderStyle.FixedSingle;
            txtConfirmPassword.PasswordChar = '•';
            txtConfirmPassword.Padding = new Padding(10);
            txtConfirmPassword.TextChanged += TxtConfirmPassword_TextChanged;

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(400, 40);
            lblError.Location = new Point(25, 405);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Success label
            lblSuccess = new Label();
            lblSuccess.Text = "";
            lblSuccess.Font = new Font("Segoe UI", 9);
            lblSuccess.ForeColor = UIHelper.SuccessColor;
            lblSuccess.Size = new Size(400, 40);
            lblSuccess.Location = new Point(25, 405);
            lblSuccess.TextAlign = ContentAlignment.MiddleCenter;
            lblSuccess.Visible = false;

            // Register button
            btnRegister = new Button();
            btnRegister.Text = "CREATE ACCOUNT";
            btnRegister.Size = new Size(195, 45);
            btnRegister.Location = new Point(25, 450);
            UIHelper.StyleButton(btnRegister, true);
            btnRegister.Click += BtnRegister_Click;

            // Back to login button
            btnBackToLogin = new Button();
            btnBackToLogin.Text = "BACK TO LOGIN";
            btnBackToLogin.Size = new Size(195, 45);
            btnBackToLogin.Location = new Point(230, 450);
            UIHelper.StyleButton(btnBackToLogin, false);
            btnBackToLogin.Click += BtnBackToLogin_Click;

            // Add controls to register panel
            registerPanel.Controls.Add(lblTitle);
            registerPanel.Controls.Add(lblUsername);
            registerPanel.Controls.Add(txtUsername);
            registerPanel.Controls.Add(lblEmail);
            registerPanel.Controls.Add(txtEmail);
            registerPanel.Controls.Add(lblPassword);
            registerPanel.Controls.Add(txtPassword);
            registerPanel.Controls.Add(lblPasswordStrength);
            registerPanel.Controls.Add(progressBar);
            registerPanel.Controls.Add(lblConfirmPassword);
            registerPanel.Controls.Add(txtConfirmPassword);
            registerPanel.Controls.Add(lblError);
            registerPanel.Controls.Add(lblSuccess);
            registerPanel.Controls.Add(btnRegister);
            registerPanel.Controls.Add(btnBackToLogin);

            // Add controls to main panel
            mainPanel.Controls.Add(picLogo);
            mainPanel.Controls.Add(registerPanel);

            // Add main panel to form
            this.Controls.Add(mainPanel);

            // Set initial focus
            this.Shown += (s, e) => txtUsername.Focus();
        }

        private void CreateTextLogo()
        {
            Bitmap logo = new Bitmap(150, 150);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);

                Rectangle rect = new Rectangle(0, 0, 150, 150);
                using (var brush = UIHelper.CreateGradientBrush(rect, UIHelper.GoldAccent, UIHelper.BlueAccent))
                {
                    Font font = new Font("Segoe UI", 20, FontStyle.Bold);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString("TriApex", font, brush, rect, format);

                    font = new Font("Segoe UI", 8, FontStyle.Regular);
                    rect.Y += 35;
                    g.DrawString("Register", font, Brushes.White, rect, format);
                }
            }

            picLogo.Image = logo;
        }

        private void ApplyThemeAndStyling()
        {
            // Apply rounded corners to form
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Make form draggable
            this.MouseDown += Form_MouseDown;
            this.KeyPreview = true;
            this.KeyDown += RegisterForm_KeyDown;
        }

        #region Event Handlers

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw gradient background
            Rectangle rect = new Rectangle(0, 0, mainPanel.Width, mainPanel.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(15, 15, 20),
                Color.FromArgb(25, 25, 35)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {
            // Clear error when user types
            ClearErrors();

            // Check username availability in real-time (optional)
            if (txtUsername.Text.Length > 3)
            {
                CheckUsernameAvailability();
            }
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            ClearErrors();
            UpdatePasswordStrength();
            ValidatePasswordMatch();
        }

        private void TxtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            ClearErrors();
            ValidatePasswordMatch();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            AttemptRegistration();
        }

        private void BtnBackToLogin_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void RegisterForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AttemptRegistration();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        #endregion

        #region Registration Logic

        private void CheckUsernameAvailability()
        {
            // This could be enhanced with async call to check username availability
            // For now, just visual feedback
        }

        private void UpdatePasswordStrength()
        {
            string password = txtPassword.Text;
            int score = CalculatePasswordStrength(password);

            progressBar.Value = score;

            if (score < 30)
            {
                lblPasswordStrength.Text = "Password strength: Weak";
                lblPasswordStrength.ForeColor = UIHelper.ErrorColor;
                progressBar.ForeColor = UIHelper.ErrorColor;
            }
            else if (score < 70)
            {
                lblPasswordStrength.Text = "Password strength: Fair";
                lblPasswordStrength.ForeColor = Color.Orange;
                progressBar.ForeColor = Color.Orange;
            }
            else
            {
                lblPasswordStrength.Text = "Password strength: Strong";
                lblPasswordStrength.ForeColor = UIHelper.SuccessColor;
                progressBar.ForeColor = UIHelper.SuccessColor;
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
                txtPassword.Text != txtConfirmPassword.Text)
            {
                lblConfirmPassword.ForeColor = UIHelper.ErrorColor;
            }
            else
            {
                lblConfirmPassword.ForeColor = UIHelper.BlueAccent;
            }
        }

        private void ClearErrors()
        {
            lblError.Visible = false;
            lblSuccess.Visible = false;
        }

        private bool ValidateInput()
        {
            // Validate username
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Please enter a username");
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length < 3)
            {
                ShowError("Username must be at least 3 characters");
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Length > 50)
            {
                ShowError("Username cannot exceed 50 characters");
                txtUsername.Focus();
                return false;
            }

            // Validate email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowError("Please enter an email address");
                txtEmail.Focus();
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                if (addr.Address != txtEmail.Text)
                {
                    ShowError("Please enter a valid email address");
                    txtEmail.Focus();
                    return false;
                }
            }
            catch
            {
                ShowError("Please enter a valid email address");
                txtEmail.Focus();
                return false;
            }

            // Validate password
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Please enter a password");
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                ShowError("Password must be at least 6 characters");
                txtPassword.Focus();
                return false;
            }

            // Validate password confirmation
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                ShowError("Passwords do not match");
                txtConfirmPassword.Focus();
                return false;
            }

            return true;
        }

        private void AttemptRegistration()
        {
            if (!ValidateInput())
                return;

            // Check if username already exists
            if (DBHelper.UserExists(txtUsername.Text))
            {
                ShowError("Username already exists. Please choose another.");
                txtUsername.Focus();
                txtUsername.SelectAll();
                return;
            }

            // Disable UI during registration
            SetRegistrationState(false);

            try
            {
                // Insert new user
                string query = @"
                    INSERT INTO Users (Username, Password, Email, Balance)
                    OUTPUT INSERTED.UserID
                    VALUES (@Username, @Password, @Email, 1000.00)";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", txtUsername.Text.Trim()),
                    new SqlParameter("@Password", txtPassword.Text),
                    new SqlParameter("@Email", txtEmail.Text.Trim())
                };

                object result = DBHelper.ExecuteScalar(query, parameters);

                if (result != null)
                {
                    int newUserId = Convert.ToInt32(result);

                    // Initialize session
                    SessionManager.Instance.InitializeSession(
                        newUserId,
                        txtUsername.Text.Trim(),
                        1000.00m,
                        txtEmail.Text.Trim()
                    );

                    // Record registration transaction
                    RecordRegistrationTransaction(newUserId);

                    // Show success message
                    OnRegistrationSuccessful();
                }
                else
                {
                    ShowError("Registration failed. Please try again.");
                    SetRegistrationState(true);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Registration error: {ex.Message}");
                SetRegistrationState(true);
            }
        }

        private void RecordRegistrationTransaction(int userId)
        {
            try
            {
                string query = @"
                    INSERT INTO Transactions (UserID, Amount, TransactionType, Description)
                    VALUES (@UserID, 1000.00, 'REGISTRATION', 'New user registration with starting balance')";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId)
                };

                DBHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // Log but don't block registration
                Console.WriteLine($"Failed to record registration transaction: {ex.Message}");
            }
        }

        private void OnRegistrationSuccessful()
        {
            lblSuccess.Text = "✓ Registration successful! Welcome to TriApex!";
            lblSuccess.Visible = true;

            // Clear sensitive data
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";

            // Start success animation
            Timer successTimer = new Timer();
            successTimer.Interval = 2000;
            successTimer.Tick += (s, e) =>
            {
                successTimer.Stop();

                // Auto-login and close registration form
                this.DialogResult = DialogResult.OK;

                MainDashboardForm dashboard = new MainDashboardForm();
                dashboard.Show();
                this.Hide(); // hide login form


            };

            successTimer.Start();
        }

        private void ShowError(string message)
        {
            lblError.Text = $"✗ {message}";
            lblError.Visible = true;

            // Auto-hide error after 5 seconds
            Timer errorTimer = new Timer();
            errorTimer.Interval = 5000;
            errorTimer.Tick += (s, e) =>
            {
                lblError.Visible = false;
                errorTimer.Stop();
            };
            errorTimer.Start();
        }

        private void SetRegistrationState(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtEmail.Enabled = enabled;
            txtPassword.Enabled = enabled;
            txtConfirmPassword.Enabled = enabled;
            btnRegister.Enabled = enabled;
            btnBackToLogin.Enabled = enabled;

            if (!enabled)
            {
                btnRegister.Text = "CREATING ACCOUNT...";
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                btnRegister.Text = "CREATE ACCOUNT";
                Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Native Methods for Draggable Form

        private static class NativeMethods
        {
            public const int WM_NCLBUTTONDOWN = 0xA1;
            public const int HT_CAPTION = 0x2;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool ReleaseCapture();
        }

        #endregion
    }
}