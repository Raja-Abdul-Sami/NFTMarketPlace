using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class LoginForm : Form
    {
        // UI Components
        private Panel mainPanel;
        private Panel loginPanel;
        private Label lblTitle;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblPassword;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblError;
        private PictureBox picLogo;
        private LinkLabel lblForgotPassword;
        private Label lblVersion;

        // Animation variables
        private Timer fadeTimer;
        private double fadeOpacity = 0.0;
        private const double FADE_INCREMENT = 0.05;

        public LoginForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
            InitializeAnimations();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // LoginForm
            this.ClientSize = new Size(1000, 700);
            this.Text = "TriApex NFT Marketplace - Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Main panel with gradient
            mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Paint += MainPanel_Paint;

            // Logo
            picLogo = new PictureBox();
            picLogo.Size = new Size(200, 200);
            picLogo.Location = new Point(400, 50);
            picLogo.BackColor = Color.Transparent;
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

            // Try to load logo from resources, otherwise create text logo
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

            // Login panel
            loginPanel = new Panel();
            loginPanel.Size = new Size(400, 450);
            loginPanel.Location = new Point(300, 250);
            UIHelper.StylePanel(loginPanel, true);

            // Title label
            lblTitle = new Label();
            lblTitle.Text = "WELCOME BACK";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(350, 50);
            lblTitle.Location = new Point(25, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Username label
            lblUsername = new Label();
            lblUsername.Text = "USERNAME";
            lblUsername.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblUsername.ForeColor = UIHelper.BlueAccent;
            lblUsername.Size = new Size(350, 25);
            lblUsername.Location = new Point(25, 100);
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;

            // Username textbox
            txtUsername = new TextBox();
            txtUsername.Size = new Size(350, 40);
            txtUsername.Location = new Point(25, 125);
            txtUsername.Font = new Font("Segoe UI", 11);
            txtUsername.BackColor = Color.FromArgb(40, 40, 50);
            txtUsername.ForeColor = Color.White;
            txtUsername.BorderStyle = BorderStyle.FixedSingle;
            txtUsername.Padding = new Padding(10);
            txtUsername.Enter += TextBox_Enter;
            txtUsername.Leave += TextBox_Leave;

            // Password label
            lblPassword = new Label();
            lblPassword.Text = "PASSWORD";
            lblPassword.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPassword.ForeColor = UIHelper.BlueAccent;
            lblPassword.Size = new Size(350, 25);
            lblPassword.Location = new Point(25, 180);
            lblPassword.TextAlign = ContentAlignment.MiddleLeft;

            // Password textbox
            txtPassword = new TextBox();
            txtPassword.Size = new Size(350, 40);
            txtPassword.Location = new Point(25, 205);
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.BackColor = Color.FromArgb(40, 40, 50);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '•';
            txtPassword.Padding = new Padding(10);
            txtPassword.Enter += TextBox_Enter;
            txtPassword.Leave += TextBox_Leave;

            // Remember me checkbox
            chkRemember = new CheckBox();
            chkRemember.Text = "Remember me";
            chkRemember.Font = new Font("Segoe UI", 9);
            chkRemember.ForeColor = UIHelper.TextSecondary;
            chkRemember.Size = new Size(150, 25);
            chkRemember.Location = new Point(25, 260);
            chkRemember.BackColor = Color.Transparent;
            chkRemember.CheckedChanged += ChkRemember_CheckedChanged;

            // Forgot password link
            lblForgotPassword = new LinkLabel();
            lblForgotPassword.Text = "Forgot Password?";
            lblForgotPassword.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblForgotPassword.LinkColor = UIHelper.BlueAccent;
            lblForgotPassword.ActiveLinkColor = Color.FromArgb(100, 200, 255);
            lblForgotPassword.Size = new Size(150, 25);
            lblForgotPassword.Location = new Point(225, 260);
            lblForgotPassword.TextAlign = ContentAlignment.MiddleRight;
            lblForgotPassword.LinkClicked += LblForgotPassword_LinkClicked;

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(350, 40);
            lblError.Location = new Point(25, 290);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Login button
            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(350, 45);
            btnLogin.Location = new Point(25, 340);
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            UIHelper.StyleButton(btnLogin, true);
            btnLogin.Click += BtnLogin_Click;

            // Register button
            btnRegister = new Button();
            btnRegister.Text = "CREATE ACCOUNT";
            btnRegister.Size = new Size(350, 45);
            btnRegister.Location = new Point(25, 395);
            UIHelper.StyleButton(btnRegister, false);
            btnRegister.Click += BtnRegister_Click;

            // Version label
            lblVersion = new Label();
            lblVersion.Text = "TriApex NFT Marketplace v1.0";
            lblVersion.Font = new Font("Segoe UI", 8);
            lblVersion.ForeColor = Color.FromArgb(100, 100, 120);
            lblVersion.Size = new Size(200, 20);
            lblVersion.Location = new Point(400, 670);
            lblVersion.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to login panel
            loginPanel.Controls.Add(lblTitle);
            loginPanel.Controls.Add(lblUsername);
            loginPanel.Controls.Add(txtUsername);
            loginPanel.Controls.Add(lblPassword);
            loginPanel.Controls.Add(txtPassword);
            loginPanel.Controls.Add(chkRemember);
            loginPanel.Controls.Add(lblForgotPassword);
            loginPanel.Controls.Add(lblError);
            loginPanel.Controls.Add(btnLogin);
            loginPanel.Controls.Add(btnRegister);

            // Add controls to main panel
            mainPanel.Controls.Add(picLogo);
            mainPanel.Controls.Add(loginPanel);
            mainPanel.Controls.Add(lblVersion);

            // Add main panel to form
            this.Controls.Add(mainPanel);

            // Set opacity for fade-in effect
            this.Opacity = 0;
        }

        private void CreateTextLogo()
        {
            // Create a simple text-based logo
            Bitmap logo = new Bitmap(200, 200);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);

                // Draw TriApex text with gradient
                Rectangle rect = new Rectangle(0, 0, 200, 200);
                using (var brush = UIHelper.CreateGradientBrush(rect, UIHelper.GoldAccent, UIHelper.BlueAccent))
                {
                    Font font = new Font("Segoe UI", 24, FontStyle.Bold);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString("TriApex", font, brush, rect, format);

                    // Add subtitle
                    font = new Font("Segoe UI", 10, FontStyle.Regular);
                    rect.Y += 40;
                    g.DrawString("NFT Marketplace", font, Brushes.White, rect, format);
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
            this.KeyDown += LoginForm_KeyDown;
        }

        private void InitializeAnimations()
        {
            // Fade-in animation timer
            fadeTimer = new Timer();
            fadeTimer.Interval = 20;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();
        }

        #region Event Handlers

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadeOpacity < 1.0)
            {
                fadeOpacity += FADE_INCREMENT;
                this.Opacity = fadeOpacity;
            }
            else
            {
                fadeTimer.Stop();
                this.Opacity = 1.0;

                // Focus username field after fade-in
                txtUsername.Focus();
            }
        }

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

            // Draw decorative elements
            DrawDecorativeElements(e.Graphics);
        }

        private void DrawDecorativeElements(Graphics g)
        {
            // Draw some abstract lines/patterns for visual appeal
            using (Pen goldPen = new Pen(UIHelper.GoldAccent, 2))
            using (Pen bluePen = new Pen(UIHelper.BlueAccent, 1))
            {
                // Draw some circles
                for (int i = 0; i < 5; i++)
                {
                    int x = 50 + i * 80;
                    int y = 100 + i * 60;
                    int size = 20 + i * 10;

                    if (i % 2 == 0)
                        g.DrawEllipse(goldPen, x, y, size, size);
                    else
                        g.DrawEllipse(bluePen, x, y, size, size);
                }
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.BackColor = Color.FromArgb(50, 50, 65);
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.BackColor = Color.FromArgb(40, 40, 50);
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void ChkRemember_CheckedChanged(object sender, EventArgs e)
        {
            // You can implement remember me functionality here
            // For now, just change visual state
            chkRemember.ForeColor = chkRemember.Checked ?
                UIHelper.GoldAccent : UIHelper.TextSecondary;
        }

        private void LblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lblForgotPassword.LinkVisited = true;
            UIHelper.ShowMessage("Please contact support at support@triapex.com to reset your password.",
                "Forgot Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // Show registration form
            RegisterForm registerForm = new RegisterForm();
            this.Hide();
            registerForm.ShowDialog();

            if (SessionManager.Instance.IsLoggedIn)
            {
                this.Close(); // Close login form if registration succeeded and auto-logged in
            }
            else
            {
                this.Show();
                txtUsername.Focus();
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // Make form draggable
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AttemptLogin();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
        }

        #endregion

        #region Login Logic

        private void AttemptLogin()
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowError("Please enter your username");
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowError("Please enter your password");
                txtPassword.Focus();
                return;
            }

            // Disable UI during login attempt
            SetLoginState(false);

            // Attempt login
            bool loginSuccess = ValidateLogin(txtUsername.Text, txtPassword.Text);

            if (loginSuccess)
            {
                OnLoginSuccessful();
            }
            else
            {
                SetLoginState(true);
                ShowError("Invalid username or password");
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
        }

        private bool ValidateLogin(string username, string password)
        {
            try
            {
                string query = @"
                    SELECT UserID, Username, Balance, Email 
                    FROM Users 
                    WHERE Username = @Username AND Password = @Password";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", password)
                };

                DataTable result = DBHelper.ExecuteQuery(query, parameters);

                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];

                    // Initialize session
                    SessionManager.Instance.InitializeSession(
                        Convert.ToInt32(row["UserID"]),
                        row["Username"].ToString(),
                        Convert.ToDecimal(row["Balance"]),
                        row["Email"].ToString()
                    );

                    // Record login transaction
                    RecordLoginTransaction();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Login error: {ex.Message}");
                return false;
            }
        }

        private void RecordLoginTransaction()
        {
            try
            {
                string query = @"
                    INSERT INTO Transactions (UserID, Amount, TransactionType, Description)
                    VALUES (@UserID, 0, 'LOGIN', 'User logged into system')";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
                };

                DBHelper.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                // Log but don't block login for transaction recording failure
                Console.WriteLine($"Failed to record login transaction: {ex.Message}");
            }
        }

        private void OnLoginSuccessful()
        {
            // Show success message
            lblError.Text = "✓ Login successful!";
            lblError.ForeColor = UIHelper.SuccessColor;
            lblError.Visible = true;

            // Start fade-out animation
            Timer closeTimer = new Timer();
            closeTimer.Interval = 1000;
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();

                // Fade out form
                Timer fadeOutTimer = new Timer();
                fadeOutTimer.Interval = 20;
                double opacity = 1.0;

                fadeOutTimer.Tick += (s2, e2) =>
                {
                    if (opacity > 0)
                    {
                        opacity -= 0.05;
                        this.Opacity = opacity;
                    }
                    else
                    {
                        fadeOutTimer.Stop();

                        // Show loading screen and open main dashboard
                        NavigationHelper.ShowLoadingThenNavigate(() =>
                        {
                            // Simulate loading
                            System.Threading.Thread.Sleep(1000);

                            // Open main dashboard on UI thread
                            this.Invoke(new Action(() =>
                            {
                                // Create and show main dashboard
                                var loadingForm = new LoadingForm();
                                loadingForm.Show();

                                // Simulate loading tasks
                                for (int i = 0; i <= 100; i += 10)
                                {
                                    loadingForm.UpdateProgress(i);
                                    loadingForm.UpdateStatus($"Loading dashboard... {i}%");
                                    System.Threading.Thread.Sleep(100);
                                }

                                loadingForm.CompleteLoading();

                                // Show main dashboard
                                MainDashboardForm dashboard = new MainDashboardForm();
                                dashboard.Show();
                                this.Hide(); // Hide login form instead of closing
                            }));
                        });
                    }
                };

                fadeOutTimer.Start();
            };

            closeTimer.Start();
        }

        private void ShowError(string message)
        {
            lblError.Text = $"✗ {message}";
            lblError.ForeColor = UIHelper.ErrorColor;
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

        private void SetLoginState(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnRegister.Enabled = enabled;
            chkRemember.Enabled = enabled;
            lblForgotPassword.Enabled = enabled;

            if (!enabled)
            {
                btnLogin.Text = "LOGGING IN...";
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                btnLogin.Text = "LOGIN";
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

        #region Form Closing

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Smooth fade out on close
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;

                Timer fadeTimer = new Timer();
                fadeTimer.Interval = 20;
                double opacity = 1.0;

                fadeTimer.Tick += (s, ev) =>
                {
                    if (opacity > 0)
                    {
                        opacity -= 0.1;
                        this.Opacity = opacity;
                    }
                    else
                    {
                        fadeTimer.Stop();
                        base.OnFormClosing(new FormClosingEventArgs(CloseReason.UserClosing, false));
                        Application.Exit();
                    }
                };

                fadeTimer.Start();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }

        #endregion
    }
}