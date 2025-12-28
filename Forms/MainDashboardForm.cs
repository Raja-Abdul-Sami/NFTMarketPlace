using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;
using TriApex.UserControls;

namespace TriApex.Forms
{
    public partial class MainDashboardForm : Form
    {
        // UI Components
        private Panel mainContainer;
        private Panel sidebarPanel;
        private Panel headerPanel;
        private Panel contentPanel;

        // Sidebar buttons
        private Button btnDashboard;
        private Button btnBrowseNFTs;
        private Button btnMyNFTs;
        private Button btnSellNFT;
        private Button btnProfile;
        private Button btnLogout;

        // Header components
        private Label lblWelcome;
        private Label lblBalance;
        private Button btnAddFunds;
        private Button btnRefresh;
        private PictureBox picProfile;

        // Current active control
        private UserControl currentContent;

        // Navigation
        private UserControl dashboardControl;
        private UserControl browseNFTsControl;
        private UserControl myNFTsControl;
        private UserControl sellNFTControl;
        private UserControl profileControl;

        public MainDashboardForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
            InitializeNavigation();
            LoadDashboard();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // MainDashboardForm
            this.ClientSize = new Size(1200, 800);
            this.Text = "TriApex NFT Marketplace";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;
            this.DoubleBuffered = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Main container
            mainContainer = new Panel();
            mainContainer.Dock = DockStyle.Fill;
            mainContainer.Paint += MainContainer_Paint;

            // Header panel
            headerPanel = new Panel();
            headerPanel.Size = new Size(1200, 70);
            headerPanel.Location = new Point(0, 0);
            headerPanel.BackColor = Color.FromArgb(28, 28, 36);
            headerPanel.Paint += HeaderPanel_Paint;

            // Welcome label
            lblWelcome = new Label();
            lblWelcome.Text = $"Welcome, {SessionManager.Instance.Username}!";
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblWelcome.ForeColor = UIHelper.GoldAccent;
            lblWelcome.Size = new Size(300, 40);
            lblWelcome.Location = new Point(100, 15);
            lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            // Balance label
            lblBalance = new Label();
            lblBalance.Text = $"Balance: {SessionManager.Instance.GetFormattedBalance()}";
            lblBalance.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblBalance.ForeColor = UIHelper.BlueAccent;
            lblBalance.Size = new Size(250, 30);
            lblBalance.Location = new Point(400, 20);
            lblBalance.TextAlign = ContentAlignment.MiddleLeft;

            // Add funds button
            btnAddFunds = new Button();
            btnAddFunds.Text = "➕ Add Funds";
            btnAddFunds.Size = new Size(120, 35);
            btnAddFunds.Location = new Point(670, 17);
            UIHelper.StyleButton(btnAddFunds, true);
            btnAddFunds.Click += BtnAddFunds_Click;

            // Refresh button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(100, 35);
            btnRefresh.Location = new Point(800, 17);
            UIHelper.StyleButton(btnRefresh, false);
            btnRefresh.Click += BtnRefresh_Click;

            // Profile picture
            picProfile = new PictureBox();
            picProfile.Size = new Size(40, 40);
            picProfile.Location = new Point(1100, 15);
            picProfile.BackColor = Color.FromArgb(50, 50, 65);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.Paint += PicProfile_Paint;
            picProfile.Click += PicProfile_Click;

            // Sidebar panel
            sidebarPanel = new Panel();
            sidebarPanel.Size = new Size(250, 730);
            sidebarPanel.Location = new Point(0, 70);
            sidebarPanel.BackColor = Color.FromArgb(22, 22, 30);
            sidebarPanel.Paint += SidebarPanel_Paint;

            // Logo in sidebar
            PictureBox sidebarLogo = new PictureBox();
            sidebarLogo.Size = new Size(200, 100);
            sidebarLogo.Location = new Point(25, 20);
            sidebarLogo.BackColor = Color.Transparent;
            sidebarLogo.SizeMode = PictureBoxSizeMode.Zoom;
            SetSidebarLogo(sidebarLogo);

            // Sidebar buttons
            int buttonY = 140;
            int buttonHeight = 50;
            int buttonSpacing = 10;

            btnDashboard = CreateSidebarButton("🏠 Dashboard", buttonY);
            buttonY += buttonHeight + buttonSpacing;

            btnBrowseNFTs = CreateSidebarButton("🔍 Browse NFTs", buttonY);
            buttonY += buttonHeight + buttonSpacing;

            btnMyNFTs = CreateSidebarButton("🎴 My NFTs", buttonY);
            buttonY += buttonHeight + buttonSpacing;

            btnSellNFT = CreateSidebarButton("💎 Sell NFT", buttonY);
            buttonY += buttonHeight + buttonSpacing;

            btnProfile = CreateSidebarButton("👤 Profile", buttonY);
            buttonY += buttonHeight + buttonSpacing;

            btnLogout = CreateSidebarButton("🚪 Logout", buttonY);
            btnLogout.ForeColor = UIHelper.ErrorColor;

            // Content panel
            contentPanel = new Panel();
            contentPanel.Size = new Size(950, 730);
            contentPanel.Location = new Point(250, 70);
            contentPanel.BackColor = Color.Transparent;

            // Add controls to panels
            headerPanel.Controls.Add(lblWelcome);
            headerPanel.Controls.Add(lblBalance);
            headerPanel.Controls.Add(btnAddFunds);
            headerPanel.Controls.Add(btnRefresh);
            headerPanel.Controls.Add(picProfile);

            sidebarPanel.Controls.Add(sidebarLogo);
            sidebarPanel.Controls.Add(btnDashboard);
            sidebarPanel.Controls.Add(btnBrowseNFTs);
            sidebarPanel.Controls.Add(btnMyNFTs);
            sidebarPanel.Controls.Add(btnSellNFT);
            sidebarPanel.Controls.Add(btnProfile);
            sidebarPanel.Controls.Add(btnLogout);

            mainContainer.Controls.Add(headerPanel);
            mainContainer.Controls.Add(sidebarPanel);
            mainContainer.Controls.Add(contentPanel);

            this.Controls.Add(mainContainer);

            // Make form draggable via header
            headerPanel.MouseDown += HeaderPanel_MouseDown;
        }

        private void SetSidebarLogo(PictureBox pictureBox)
        {
            try
            {
                if (Properties.Resources.TriApexLogo != null)
                {
                    // Resize logo for sidebar
                    Bitmap original = Properties.Resources.TriApexLogo;
                    Bitmap resized = new Bitmap(original, new Size(200, 100));
                    pictureBox.Image = resized;
                }
                else
                {
                    CreateSidebarTextLogo(pictureBox);
                }
            }
            catch
            {
                CreateSidebarTextLogo(pictureBox);
            }
        }

        private void CreateSidebarTextLogo(PictureBox pictureBox)
        {
            Bitmap logo = new Bitmap(200, 100);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);

                Rectangle rect = new Rectangle(0, 0, 200, 100);
                using (var brush = UIHelper.CreateGradientBrush(rect, UIHelper.GoldAccent, UIHelper.BlueAccent))
                {
                    Font font = new Font("Segoe UI", 18, FontStyle.Bold);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString("TriApex", font, brush, rect, format);

                    font = new Font("Segoe UI", 9, FontStyle.Regular);
                    rect.Y += 30;
                    g.DrawString("NFT Marketplace", font, Brushes.White, rect, format);
                }
            }

            pictureBox.Image = logo;
        }

        private Button CreateSidebarButton(string text, int yPosition)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(230, 50);
            button.Location = new Point(10, yPosition);
            button.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            button.ForeColor = UIHelper.TextPrimary;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.Transparent;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.Padding = new Padding(20, 0, 0, 0);
            button.Cursor = Cursors.Hand;

            // Hover effects
            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Color.FromArgb(40, 40, 50);
                button.ForeColor = UIHelper.GoldAccent;
            };

            button.MouseLeave += (s, e) =>
            {
                if (button != btnDashboard) // Keep dashboard highlighted when active
                {
                    button.BackColor = Color.Transparent;
                    button.ForeColor = UIHelper.TextPrimary;
                }
            };

            // Click event will be assigned in InitializeNavigation

            return button;
        }

        private void ApplyThemeAndStyling()
        {
            // Apply rounded corners to form
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += MainDashboardForm_KeyDown;
        }

        private void InitializeNavigation()
        {
            // Assign click events
            btnDashboard.Click += (s, e) => NavigateTo("DASHBOARD");
            btnBrowseNFTs.Click += (s, e) => NavigateTo("BROWSE");
            btnMyNFTs.Click += (s, e) => NavigateTo("MYNFTS");
            btnSellNFT.Click += (s, e) => NavigateTo("SELL");
            btnProfile.Click += (s, e) => NavigateTo("PROFILE");
            btnLogout.Click += BtnLogout_Click;

            // Initialize user controls (lazy loading)
            dashboardControl = null;
            browseNFTsControl = null;
            myNFTsControl = null;
            sellNFTControl = null;
            profileControl = null;
        }

        #region Event Handlers

        private void MainContainer_Paint(object sender, PaintEventArgs e)
        {
            // Draw main background
            Rectangle rect = new Rectangle(0, 0, mainContainer.Width, mainContainer.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(18, 18, 24),
                Color.FromArgb(25, 25, 32)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void HeaderPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw header gradient
            Rectangle rect = new Rectangle(0, 0, headerPanel.Width, headerPanel.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(30, 30, 38),
                Color.FromArgb(25, 25, 32)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw bottom border
            using (Pen pen = new Pen(UIHelper.GoldAccent, 2))
            {
                e.Graphics.DrawLine(pen, 0, headerPanel.Height - 2, headerPanel.Width, headerPanel.Height - 2);
            }
        }

        private void SidebarPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw sidebar gradient
            Rectangle rect = new Rectangle(0, 0, sidebarPanel.Width, sidebarPanel.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(22, 22, 30),
                Color.FromArgb(20, 20, 28)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw right border
            using (Pen pen = new Pen(Color.FromArgb(40, 40, 50), 1))
            {
                e.Graphics.DrawLine(pen, sidebarPanel.Width - 1, 0, sidebarPanel.Width - 1, sidebarPanel.Height);
            }
        }

        private void PicProfile_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb == null) return;

            // Draw profile circle
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Fill circle
            using (Brush brush = new SolidBrush(Color.FromArgb(60, 60, 75)))
            {
                e.Graphics.FillEllipse(brush, 0, 0, pb.Width - 1, pb.Height - 1);
            }

            // Draw border
            using (Pen pen = new Pen(UIHelper.GoldAccent, 2))
            {
                e.Graphics.DrawEllipse(pen, 1, 1, pb.Width - 3, pb.Height - 3);
            }

            // Draw user initials
            string initials = GetUserInitials();
            using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(UIHelper.GoldAccent))
            {
                SizeF textSize = e.Graphics.MeasureString(initials, font);
                float x = (pb.Width - textSize.Width) / 2;
                float y = (pb.Height - textSize.Height) / 2;
                e.Graphics.DrawString(initials, font, textBrush, x, y);
            }
        }

        private string GetUserInitials()
        {
            string username = SessionManager.Instance.Username;
            if (string.IsNullOrEmpty(username))
                return "??";

            string[] parts = username.Split(new char[] { ' ', '.', '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();

            if (username.Length >= 2)
                return username.Substring(0, 2).ToUpper();

            return username.Length > 0 ? username[0].ToString().ToUpper() : "U";
        }

        private void PicProfile_Click(object sender, EventArgs e)
        {
            NavigateTo("PROFILE");
        }

        private void BtnAddFunds_Click(object sender, EventArgs e)
        {
            ShowAddFundsDialog();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDashboard();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Logout();
        }

        private void HeaderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void MainDashboardForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.D:
                        NavigateTo("DASHBOARD");
                        break;
                    case Keys.B:
                        NavigateTo("BROWSE");
                        break;
                    case Keys.M:
                        NavigateTo("MYNFTS");
                        break;
                    case Keys.S:
                        NavigateTo("SELL");
                        break;
                    case Keys.P:
                        NavigateTo("PROFILE");
                        break;
                    case Keys.L:
                        Logout();
                        break;
                    case Keys.R:
                        RefreshDashboard();
                        break;
                    case Keys.F:
                        ShowAddFundsDialog();
                        break;
                    case Keys.Q:
                        CloseApplication();
                        break;
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                CloseApplication();
            }
        }

        #endregion

        #region Navigation Methods

        private void NavigateTo(string destination)
        {
            // Reset all button styles
            ResetSidebarButtons();

            // Highlight active button
            switch (destination)
            {
                case "DASHBOARD":
                    HighlightButton(btnDashboard);
                    LoadDashboard();
                    break;
                case "BROWSE":
                    HighlightButton(btnBrowseNFTs);
                    LoadBrowseNFTs();
                    break;
                case "MYNFTS":
                    HighlightButton(btnMyNFTs);
                    LoadMyNFTs();
                    break;
                case "SELL":
                    HighlightButton(btnSellNFT);
                    LoadSellNFT();
                    break;
                case "PROFILE":
                    HighlightButton(btnProfile);
                    LoadProfile();
                    break;
            }
        }

        private void ResetSidebarButtons()
        {
            foreach (Control control in sidebarPanel.Controls)
            {
                if (control is Button button && button != btnLogout)
                {
                    button.BackColor = Color.Transparent;
                    button.ForeColor = UIHelper.TextPrimary;
                    button.Font = new Font("Segoe UI", 11, FontStyle.Regular);
                }
            }
        }

        private void HighlightButton(Button button)
        {
            button.BackColor = Color.FromArgb(40, 40, 50);
            button.ForeColor = UIHelper.GoldAccent;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        }

        private void LoadDashboard()
        {
            NavigationHelper.ShowLoadingThenNavigate(() =>
            {
                if (dashboardControl == null)
                {
                    dashboardControl = new DashboardControl();
                    dashboardControl.Dock = DockStyle.Fill;
                }

                this.Invoke(new Action(() =>
                {
                    SwitchContentControl(dashboardControl);
                }));
            });
        }

        private void LoadBrowseNFTs()
        {
            NavigationHelper.ShowLoadingThenNavigate(() =>
            {
                if (browseNFTsControl == null)
                {
                    browseNFTsControl = new BrowseNFTsControl();
                    browseNFTsControl.Dock = DockStyle.Fill;
                }

                this.Invoke(new Action(() =>
                {
                    SwitchContentControl(browseNFTsControl);
                }));
            });
        }

        private void LoadMyNFTs()
        {
            NavigationHelper.ShowLoadingThenNavigate(() =>
            {
                if (myNFTsControl == null)
                {
                    myNFTsControl = new MyNFTsControl();
                    myNFTsControl.Dock = DockStyle.Fill;
                }

                this.Invoke(new Action(() =>
                {
                    SwitchContentControl(myNFTsControl);
                }));
            });
        }

        private void LoadSellNFT()
        {
            NavigationHelper.ShowLoadingThenNavigate(() =>
            {
                if (sellNFTControl == null)
                {
                    sellNFTControl = new SellNFTControl();
                    sellNFTControl.Dock = DockStyle.Fill;
                }

                this.Invoke(new Action(() =>
                {
                    SwitchContentControl(sellNFTControl);
                }));
            });
        }

        private void LoadProfile()
        {
            NavigationHelper.ShowLoadingThenNavigate(() =>
            {
                if (profileControl == null)
                {
                    profileControl = new ProfileControl();
                    profileControl.Dock = DockStyle.Fill;
                }

                this.Invoke(new Action(() =>
                {
                    SwitchContentControl(profileControl);
                }));
            });
        }

        private void SwitchContentControl(UserControl newControl)
        {
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(newControl);
            currentContent = newControl;
        }

        #endregion

        #region Business Logic Methods

        private void RefreshDashboard()
        {
            // Refresh balance
            SessionManager.Instance.RefreshBalance();
            lblBalance.Text = $"Balance: {SessionManager.Instance.GetFormattedBalance()}";

            // Refresh current content if it supports refresh
            if (currentContent is DashboardControl dashboard)
            {
                dashboard.RefreshData();
            }
            else if (currentContent is BrowseNFTsControl browse)
            {
                browse.RefreshNFTs();
            }
            else if (currentContent is MyNFTsControl myNFTs)
            {
                myNFTs.RefreshNFTs();
            }

            UIHelper.ShowMessage("Dashboard refreshed successfully!", "Refresh Complete");
        }

        private void ShowAddFundsDialog()
        {
            using (AddFundsForm addFundsForm = new AddFundsForm())
            {
                if (addFundsForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh balance display
                    SessionManager.Instance.RefreshBalance();
                    lblBalance.Text = $"Balance: {SessionManager.Instance.GetFormattedBalance()}";

                    // Refresh current content
                    RefreshDashboard();
                }
            }
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Clear session
                SessionManager.Instance.ClearSession();

                // Show login form
                LoginForm loginForm = new LoginForm();
                loginForm.Show();

                // Close dashboard
                this.Close();
            }
        }

        private void CloseApplication()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit TriApex?",
                "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
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

        #region Form Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Initialize NavigationHelper
            NavigationHelper.Initialize(this);

            // Update UI with current user data
            UpdateUserDisplay();
        }

        private void UpdateUserDisplay()
        {
            lblWelcome.Text = $"Welcome, {SessionManager.Instance.Username}!";
            lblBalance.Text = $"Balance: {SessionManager.Instance.GetFormattedBalance()}";
            picProfile.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Check if this is the main form closing
            if (Application.OpenForms.Count <= 1 && e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to exit TriApex?",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    SessionManager.Instance.ClearSession();
                }
            }

            base.OnFormClosing(e);
        }

        #endregion
    }
}