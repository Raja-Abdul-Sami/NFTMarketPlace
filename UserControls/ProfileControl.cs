using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TriApex.Helpers;
using TriApex.Forms;
using System.Data.SqlClient;

namespace TriApex.UserControls
{
    public partial class ProfileControl : UserControl
    {
        // UI Components
        private Panel profileHeader;
        private Panel statsPanel;
        private Panel detailsPanel;
        private Panel activityPanel;

        // Profile header
        private PictureBox picProfile;
        private Label lblUsername;
        private Label lblMemberSince;
        private Button btnEditProfile;
        private Button btnChangePassword;

        // Stats
        private Label lblTotalNFTs;
        private Label lblTotalSales;
        private Label lblTotalSpent;
        private Label lblTotalEarned;

        // Details
        private Label lblEmail;
        private Label lblAccountBalance;
        private Label lblLastLogin;
        private Label lblTotalBids;

        // Activity
        private DataGridView dgvRecentActivity;
        private Button btnViewAllActivity;

        // Data
        private DataTable userActivity;
        private DataRow userData;

        public ProfileControl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadUserProfile();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // ProfileControl
            this.BackColor = Color.Transparent;
            this.Size = new Size(950, 730);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "MY PROFILE";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(300, 50);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Profile header
            profileHeader = new Panel();
            profileHeader.Size = new Size(910, 150);
            profileHeader.Location = new Point(20, 80);
            profileHeader.BackColor = Color.FromArgb(35, 35, 45);
            profileHeader.Paint += ProfileHeader_Paint;

            // Profile picture
            picProfile = new PictureBox();
            picProfile.Size = new Size(100, 100);
            picProfile.Location = new Point(30, 25);
            picProfile.BackColor = Color.FromArgb(60, 60, 75);
            picProfile.SizeMode = PictureBoxSizeMode.Zoom;
            picProfile.Cursor = Cursors.Hand;
            picProfile.Click += PicProfile_Click;
            picProfile.Paint += PicProfile_Paint;

            // Username
            lblUsername = new Label();
            lblUsername.Text = SessionManager.Instance.Username;
            lblUsername.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblUsername.ForeColor = UIHelper.TextPrimary;
            lblUsername.Size = new Size(300, 40);
            lblUsername.Location = new Point(150, 30);
            lblUsername.TextAlign = ContentAlignment.MiddleLeft;

            // Member since
            lblMemberSince = new Label();
            lblMemberSince.Text = "Member since: Loading...";
            lblMemberSince.Font = new Font("Segoe UI", 11);
            lblMemberSince.ForeColor = UIHelper.TextSecondary;
            lblMemberSince.Size = new Size(300, 30);
            lblMemberSince.Location = new Point(150, 75);
            lblMemberSince.TextAlign = ContentAlignment.MiddleLeft;

            // Edit profile button
            btnEditProfile = new Button();
            btnEditProfile.Text = "✏️ EDIT PROFILE";
            btnEditProfile.Size = new Size(150, 40);
            btnEditProfile.Location = new Point(700, 55);
            UIHelper.StyleButton(btnEditProfile, true);
            btnEditProfile.Click += BtnEditProfile_Click;

            // Change password button
            btnChangePassword = new Button();
            btnChangePassword.Text = "🔒 CHANGE PASSWORD";
            btnChangePassword.Size = new Size(180, 40);
            btnChangePassword.Location = new Point(500, 55);
            UIHelper.StyleButton(btnChangePassword, false);
            btnChangePassword.Click += BtnChangePassword_Click;

            // Stats panel
            statsPanel = new Panel();
            statsPanel.Size = new Size(910, 120);
            statsPanel.Location = new Point(20, 250);
            statsPanel.BackColor = Color.FromArgb(35, 35, 45);
            statsPanel.Paint += StatsPanel_Paint;

            // Stats title
            Label lblStatsTitle = new Label();
            lblStatsTitle.Text = "PROFILE STATISTICS";
            lblStatsTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblStatsTitle.ForeColor = UIHelper.GoldAccent;
            lblStatsTitle.Size = new Size(200, 40);
            lblStatsTitle.Location = new Point(20, 10);
            lblStatsTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Create 4 stat boxes
            lblTotalNFTs = CreateStatBox(statsPanel, 0, "TOTAL NFTs", "0", UIHelper.BlueAccent, "🎴");
            lblTotalSales = CreateStatBox(statsPanel, 1, "TOTAL SALES", "0", UIHelper.GoldAccent, "💰");
            lblTotalSpent = CreateStatBox(statsPanel, 2, "TOTAL SPENT", "$0.00", Color.FromArgb(46, 204, 113), "📤");
            lblTotalEarned = CreateStatBox(statsPanel, 3, "TOTAL EARNED", "$0.00", Color.FromArgb(155, 89, 182), "📥");

            // Details panel
            detailsPanel = new Panel();
            detailsPanel.Size = new Size(450, 250);
            detailsPanel.Location = new Point(20, 390);
            detailsPanel.BackColor = Color.FromArgb(35, 35, 45);
            detailsPanel.Paint += DetailsPanel_Paint;

            // Details title
            Label lblDetailsTitle = new Label();
            lblDetailsTitle.Text = "ACCOUNT DETAILS";
            lblDetailsTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblDetailsTitle.ForeColor = UIHelper.GoldAccent;
            lblDetailsTitle.Size = new Size(400, 40);
            lblDetailsTitle.Location = new Point(20, 10);
            lblDetailsTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Account details
            int detailY = 60;
            int detailSpacing = 40;

            lblEmail = CreateDetailItem(detailsPanel, detailY, "📧 EMAIL", SessionManager.Instance.Email ?? "Not set");
            detailY += detailSpacing;

            lblAccountBalance = CreateDetailItem(detailsPanel, detailY, "💰 ACCOUNT BALANCE", SessionManager.Instance.GetFormattedBalance());
            detailY += detailSpacing;

            lblLastLogin = CreateDetailItem(detailsPanel, detailY, "⏰ LAST LOGIN", "Loading...");
            detailY += detailSpacing;

            lblTotalBids = CreateDetailItem(detailsPanel, detailY, "🔨 TOTAL BIDS", "Loading...");

            // Refresh balance button
            Button btnRefreshBalance = new Button();
            btnRefreshBalance.Text = "🔄 REFRESH BALANCE";
            btnRefreshBalance.Size = new Size(180, 35);
            btnRefreshBalance.Location = new Point(240, 195);
            UIHelper.StyleButton(btnRefreshBalance, false);
            btnRefreshBalance.Click += BtnRefreshBalance_Click;

            // Activity panel
            activityPanel = new Panel();
            activityPanel.Size = new Size(430, 250);
            activityPanel.Location = new Point(490, 390);
            activityPanel.BackColor = Color.FromArgb(35, 35, 45);
            activityPanel.Paint += ActivityPanel_Paint;

            // Activity title
            Label lblActivityTitle = new Label();
            lblActivityTitle.Text = "RECENT ACTIVITY";
            lblActivityTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblActivityTitle.ForeColor = UIHelper.GoldAccent;
            lblActivityTitle.Size = new Size(400, 40);
            lblActivityTitle.Location = new Point(20, 10);
            lblActivityTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Activity grid
            dgvRecentActivity = new DataGridView();
            dgvRecentActivity.Size = new Size(390, 170);
            dgvRecentActivity.Location = new Point(20, 60);
            dgvRecentActivity.BackgroundColor = Color.FromArgb(40, 40, 50);
            dgvRecentActivity.BorderStyle = BorderStyle.None;
            dgvRecentActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRecentActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecentActivity.RowHeadersVisible = false;
            dgvRecentActivity.AllowUserToAddRows = false;
            dgvRecentActivity.AllowUserToDeleteRows = false;
            dgvRecentActivity.ReadOnly = true;
            dgvRecentActivity.DefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvRecentActivity.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 60);
            dgvRecentActivity.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 70);
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvRecentActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRecentActivity.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 55);
            dgvRecentActivity.EnableHeadersVisualStyles = false;

            // View all activity button
            btnViewAllActivity = new Button();
            btnViewAllActivity.Text = "VIEW ALL ACTIVITY →";
            btnViewAllActivity.Size = new Size(150, 35);
            btnViewAllActivity.Location = new Point(260, 195);
            UIHelper.StyleButton(btnViewAllActivity, false);
            btnViewAllActivity.Click += BtnViewAllActivity_Click;

            // Add controls to panels
            profileHeader.Controls.Add(picProfile);
            profileHeader.Controls.Add(lblUsername);
            profileHeader.Controls.Add(lblMemberSince);
            profileHeader.Controls.Add(btnEditProfile);
            profileHeader.Controls.Add(btnChangePassword);

            statsPanel.Controls.Add(lblStatsTitle);

            detailsPanel.Controls.Add(lblDetailsTitle);
            detailsPanel.Controls.Add(btnRefreshBalance);

            activityPanel.Controls.Add(lblActivityTitle);
            activityPanel.Controls.Add(dgvRecentActivity);
            activityPanel.Controls.Add(btnViewAllActivity);

            // Add to main control
            this.Controls.Add(lblTitle);
            this.Controls.Add(profileHeader);
            this.Controls.Add(statsPanel);
            this.Controls.Add(detailsPanel);
            this.Controls.Add(activityPanel);
        }

        #region Panel Paint Events

        private void ProfileHeader_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, profileHeader.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);

            // Draw gradient background
            Rectangle rect = new Rectangle(0, 0, profileHeader.Width, profileHeader.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(40, 40, 50),
                Color.FromArgb(35, 35, 45)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, statsPanel.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
        }

        private void DetailsPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, detailsPanel.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
        }

        private void ActivityPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, activityPanel.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
        }

        private void PicProfile_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb == null) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw profile circle
            using (Brush brush = new SolidBrush(Color.FromArgb(60, 60, 75)))
            {
                e.Graphics.FillEllipse(brush, 0, 0, pb.Width - 1, pb.Height - 1);
            }

            // Draw border
            using (Pen pen = new Pen(UIHelper.GoldAccent, 3))
            {
                e.Graphics.DrawEllipse(pen, 1, 1, pb.Width - 3, pb.Height - 3);
            }

            // Draw user initials
            string initials = GetUserInitials();
            using (Font font = new Font("Segoe UI", 24, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(UIHelper.GoldAccent))
            {
                SizeF textSize = e.Graphics.MeasureString(initials, font);
                float x = (pb.Width - textSize.Width) / 2;
                float y = (pb.Height - textSize.Height) / 2;
                e.Graphics.DrawString(initials, font, textBrush, x, y);
            }

            // Draw camera icon for editing
            using (Font font = new Font("Segoe UI", 14))
            using (Brush brush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                e.Graphics.DrawString("📷", font, brush, pb.Width - 30, pb.Height - 30);
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

        #endregion

        #region UI Creation Methods

        private Label CreateStatBox(Panel container, int index, string title, string value, Color color, string icon)
        {
            int boxWidth = 220;
            int boxHeight = 80;
            int spacing = 5;
            int x = 20 + (index * (boxWidth + spacing));

            Panel statBox = new Panel();
            statBox.Size = new Size(boxWidth, boxHeight);
            statBox.Location = new Point(x, 50);
            statBox.BackColor = Color.FromArgb(40, 40, 50);
            statBox.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(color, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, statBox.Width - 3, statBox.Height - 3);
                }
            };

            // Icon
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 16);
            lblIcon.ForeColor = color;
            lblIcon.Size = new Size(40, 40);
            lblIcon.Location = new Point(10, 20);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.TextSecondary;
            lblTitle.Size = new Size(120, 20);
            lblTitle.Location = new Point(60, 15);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Value
            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Size = new Size(140, 30);
            lblValue.Location = new Point(60, 35);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            statBox.Controls.Add(lblIcon);
            statBox.Controls.Add(lblTitle);
            statBox.Controls.Add(lblValue);

            container.Controls.Add(statBox);

            return lblValue;
        }

        private Label CreateDetailItem(Panel container, int y, string label, string value)
        {
            // Label
            Label lblLabel = new Label();
            lblLabel.Text = label;
            lblLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblLabel.ForeColor = UIHelper.TextSecondary;
            lblLabel.Size = new Size(200, 25);
            lblLabel.Location = new Point(30, y);
            lblLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Value
            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblValue.ForeColor = UIHelper.TextPrimary;
            lblValue.Size = new Size(200, 25);
            lblValue.Location = new Point(230, y);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            container.Controls.Add(lblLabel);
            container.Controls.Add(lblValue);

            return lblValue;
        }

        #endregion

        #region Data Loading

        private void LoadUserProfile()
        {
            try
            {
                // Load user data
                LoadUserData();

                // Load statistics
                LoadStatistics();

                // Load recent activity
                LoadRecentActivity();

                // Update UI
                UpdateProfileUI();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to load profile: {ex.Message}");
            }
        }

        private void LoadUserData()
        {
            string query = @"
                SELECT 
                    Username,
                    Email,
                    Balance,
                    CreatedDate,
                    (SELECT MAX(TransactionDate) FROM Transactions WHERE UserID = Users.UserID AND TransactionType = 'LOGIN') as LastLogin
                FROM Users 
                WHERE UserID = @UserID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            DataTable result = DBHelper.ExecuteQuery(query, parameters);

            if (result.Rows.Count > 0)
            {
                userData = result.Rows[0];
            }
        }

        private void LoadStatistics()
        {
            int userId = SessionManager.Instance.CurrentUserID;

            // Total NFTs owned
            string queryNFTs = @"
                SELECT COUNT(*) as Count 
                FROM NFTs 
                WHERE OwnerID = @UserID AND IsSold = 0";

            // Total sales
            string querySales = @"
                SELECT COUNT(DISTINCT NFTID) as Count 
                FROM NFTs 
                WHERE CreatedBy = @Username AND IsSold = 1";

            // Total spent
            string querySpent = @"
                SELECT ISNULL(SUM(Amount), 0) as Total 
                FROM Transactions 
                WHERE UserID = @UserID 
                AND TransactionType IN ('PURCHASE', 'BID', 'ADD_FUNDS') 
                AND Amount > 0";

            // Total earned
            string queryEarned = @"
                SELECT ISNULL(SUM(Amount), 0) as Total 
                FROM Transactions 
                WHERE UserID = @UserID 
                AND TransactionType IN ('SALE')";

            // Total bids
            string queryBids = @"
                SELECT COUNT(*) as Count 
                FROM Bids 
                WHERE UserID = @UserID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId),
                new SqlParameter("@Username", SessionManager.Instance.Username)
            };

            int totalNFTs = Convert.ToInt32(DBHelper.ExecuteScalar(queryNFTs, parameters));
            int totalSales = Convert.ToInt32(DBHelper.ExecuteScalar(querySales, parameters));
            decimal totalSpent = Convert.ToDecimal(DBHelper.ExecuteScalar(querySpent, parameters));
            decimal totalEarned = Convert.ToDecimal(DBHelper.ExecuteScalar(queryEarned, parameters));
            int totalBids = Convert.ToInt32(DBHelper.ExecuteScalar(queryBids, parameters));

            // Update labels
            lblTotalNFTs.Text = totalNFTs.ToString();
            lblTotalSales.Text = totalSales.ToString();
            lblTotalSpent.Text = totalSpent.ToString("C2");
            lblTotalEarned.Text = totalEarned.ToString("C2");
            lblTotalBids.Text = totalBids.ToString();
        }

        private void LoadRecentActivity()
        {
            string query = @"
        SELECT TOP 10 
            TransactionType,
            Amount,
            Description,
            TransactionDate,
            CASE 
                WHEN NFTID IS NOT NULL THEN (SELECT Title FROM NFTs WHERE NFTID = t.NFTID)
                ELSE ''
            END as NFTTitle
        FROM Transactions t
        WHERE UserID = @UserID
        ORDER BY TransactionDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            userActivity = DBHelper.ExecuteQuery(query, parameters);

            // Clear previous data and columns
            dgvRecentActivity.DataSource = null;
            dgvRecentActivity.Columns.Clear();

            // Add columns manually
            dgvRecentActivity.Columns.Add("TransactionType", "Type");
            dgvRecentActivity.Columns.Add("Amount", "Amount");
            dgvRecentActivity.Columns.Add("Description", "Description");
            dgvRecentActivity.Columns.Add("TransactionDate", "Date");
            dgvRecentActivity.Columns.Add("NFTTitle", "NFTTitle");

            dgvRecentActivity.Columns["NFTTitle"].Visible = false;
            dgvRecentActivity.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRecentActivity.Columns["Amount"].DefaultCellStyle.Format = "C2";

            // Add rows
            if (userActivity != null && userActivity.Rows.Count > 0)
            {
                foreach (DataRow row in userActivity.Rows)
                {
                    dgvRecentActivity.Rows.Add(
                        row["TransactionType"],
                        row["Amount"],
                        row["Description"],
                        row["TransactionDate"],
                        row["NFTTitle"]
                    );
                }
            }
        }


        private void DgvRecentActivity_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvRecentActivity.Columns["TransactionType"].Index)
            {
                string transactionType = e.Value?.ToString();

                switch (transactionType)
                {
                    case "PURCHASE":
                        e.CellStyle.ForeColor = UIHelper.ErrorColor;
                        break;
                    case "SALE":
                        e.CellStyle.ForeColor = UIHelper.SuccessColor;
                        break;
                    case "ADD_FUNDS":
                        e.CellStyle.ForeColor = UIHelper.GoldAccent;
                        break;
                    case "BID":
                        e.CellStyle.ForeColor = UIHelper.BlueAccent;
                        break;
                    case "CREATE":
                        e.CellStyle.ForeColor = Color.FromArgb(155, 89, 182);
                        break;
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == dgvRecentActivity.Columns["Amount"].Index)
            {
                if (decimal.TryParse(e.Value?.ToString(), out decimal amount))
                {
                    string transactionType = dgvRecentActivity.Rows[e.RowIndex].Cells["TransactionType"].Value?.ToString();

                    if (transactionType == "PURCHASE" || transactionType == "BID")
                    {
                        e.CellStyle.ForeColor = UIHelper.ErrorColor;
                    }
                    else if (transactionType == "SALE" || transactionType == "ADD_FUNDS")
                    {
                        e.CellStyle.ForeColor = UIHelper.SuccessColor;
                    }
                }
            }
        }

        private void UpdateProfileUI()
        {
            // Update member since
            if (userData != null && userData["CreatedDate"] != DBNull.Value)
            {
                DateTime createdDate = Convert.ToDateTime(userData["CreatedDate"]);
                lblMemberSince.Text = $"Member since: {createdDate:MMMM dd, yyyy}";
            }

            // Update last login
            if (userData != null && userData["LastLogin"] != DBNull.Value)
            {
                DateTime lastLogin = Convert.ToDateTime(userData["LastLogin"]);
                TimeSpan timeSince = DateTime.Now - lastLogin;

                string timeText;
                if (timeSince.TotalMinutes < 1)
                    timeText = "Just now";
                else if (timeSince.TotalHours < 1)
                    timeText = $"{(int)timeSince.TotalMinutes} minutes ago";
                else if (timeSince.TotalDays < 1)
                    timeText = $"{(int)timeSince.TotalHours} hours ago";
                else if (timeSince.TotalDays < 7)
                    timeText = $"{(int)timeSince.TotalDays} days ago";
                else
                    timeText = lastLogin.ToString("MMMM dd, yyyy");

                lblLastLogin.Text = timeText;
            }

            // Update balance
            lblAccountBalance.Text = SessionManager.Instance.GetFormattedBalance();
        }

        #endregion

        #region Event Handlers

        private void PicProfile_Click(object sender, EventArgs e)
        {
            // Open change profile picture dialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";
                openFileDialog.Title = "Select Profile Picture";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load and set profile picture
                        Image profileImage = Image.FromFile(openFileDialog.FileName);

                        // Resize to fit
                        Image resizedImage = ImageHelper.ResizeImage(profileImage, 100, 100);

                        // Update picture box
                        picProfile.Image = resizedImage;

                        // Save to database (in a real app, you'd save to server)
                        // For now, just show success message
                        UIHelper.ShowSuccess("Profile picture updated! (Note: In a full implementation, this would be saved to your account)");
                    }
                    catch (Exception ex)
                    {
                        UIHelper.ShowError($"Failed to load image: {ex.Message}");
                    }
                }
            }
        }

        private void BtnEditProfile_Click(object sender, EventArgs e)
        {
            using (EditProfileForm editForm = new EditProfileForm())
            {
                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Refresh profile data
                    LoadUserProfile();
                }
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            using (ChangePasswordForm changeForm = new ChangePasswordForm())
            {
                changeForm.ShowDialog(this);
            }
        }

        private void BtnRefreshBalance_Click(object sender, EventArgs e)
        {
            SessionManager.Instance.RefreshBalance();
            lblAccountBalance.Text = SessionManager.Instance.GetFormattedBalance();
            UIHelper.ShowMessage("Balance refreshed successfully!", "Refresh Complete");
        }

        private void BtnViewAllActivity_Click(object sender, EventArgs e)
        {
            using (AllActivityForm activityForm = new AllActivityForm())
            {
                activityForm.ShowDialog(this);
            }
        }

        #endregion

        #region Public Methods

        public void RefreshProfile()
        {
            LoadUserProfile();
            UIHelper.ShowMessage("Profile refreshed successfully!", "Refresh Complete");
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw decorative lines
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 70), 1))
            {
                // Horizontal separators
                e.Graphics.DrawLine(pen, 20, 240, 930, 240);
                e.Graphics.DrawLine(pen, 20, 380, 930, 380);

                // Vertical separator between details and activity
                e.Graphics.DrawLine(pen, 480, 390, 480, 640);
            }
        }
    }
}