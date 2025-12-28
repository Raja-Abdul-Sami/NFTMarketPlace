using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.UserControls
{
    public partial class DashboardControl : UserControl
    {
        // UI Components
        private Label lblTitle;
        private Label lblWelcome;
        private Panel statsPanel;
        private FlowLayoutPanel recentActivitiesPanel;
        private FlowLayoutPanel trendingNFTsPanel;
        private Button btnRefresh;

        // Statistics labels
        private Label lblTotalBalance;
        private Label lblOwnedNFTs;
        private Label lblActiveBids;
        private Label lblTotalSpent;

        // Data
        private DataTable recentActivities;
        private DataTable trendingNFTs;

        public DashboardControl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DashboardControl
            this.BackColor = Color.Transparent;
            this.Size = new Size(950, 730);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            lblTitle = new Label();
            lblTitle.Text = "DASHBOARD OVERVIEW";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Welcome message
            lblWelcome = new Label();
            lblWelcome.Text = $"Welcome back, {SessionManager.Instance.Username}!";
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Regular);
            lblWelcome.ForeColor = UIHelper.TextPrimary;
            lblWelcome.Size = new Size(400, 30);
            lblWelcome.Location = new Point(20, 75);
            lblWelcome.TextAlign = ContentAlignment.MiddleLeft;

            // Refresh button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(810, 25);
            UIHelper.StyleButton(btnRefresh, false);
            btnRefresh.Click += BtnRefresh_Click;

            // Stats panel container
            Panel statsContainer = new Panel();
            statsContainer.Size = new Size(910, 180);
            statsContainer.Location = new Point(20, 120);
            statsContainer.BackColor = Color.Transparent;

            // Create 4 stat cards
            lblTotalBalance = CreateStatCard(statsContainer, 0,
                "TOTAL BALANCE",
                SessionManager.Instance.GetFormattedBalance(),
                UIHelper.GoldAccent, "💰");

            lblOwnedNFTs = CreateStatCard(statsContainer, 1,
                "OWNED NFTs",
                "0",
                UIHelper.BlueAccent, "🎴");

            lblActiveBids = CreateStatCard(statsContainer, 2,
                "ACTIVE BIDS",
                "0",
                Color.FromArgb(46, 204, 113), "⚡");

            lblTotalSpent = CreateStatCard(statsContainer, 3,
                "TOTAL SPENT",
                "$0.00",
                Color.FromArgb(155, 89, 182), "💎");

            // Recent Activities section
            Label lblRecentActivities = new Label();
            lblRecentActivities.Text = "RECENT ACTIVITIES";
            lblRecentActivities.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblRecentActivities.ForeColor = UIHelper.TextPrimary;
            lblRecentActivities.Size = new Size(300, 40);
            lblRecentActivities.Location = new Point(20, 320);
            lblRecentActivities.TextAlign = ContentAlignment.MiddleLeft;

            // Recent Activities panel
            recentActivitiesPanel = new FlowLayoutPanel();
            recentActivitiesPanel.Size = new Size(450, 350);
            recentActivitiesPanel.Location = new Point(20, 370);
            recentActivitiesPanel.BackColor = Color.FromArgb(30, 30, 40);
            recentActivitiesPanel.AutoScroll = true;
            recentActivitiesPanel.Padding = new Padding(10);
            recentActivitiesPanel.BorderStyle = BorderStyle.None;
            recentActivitiesPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, recentActivitiesPanel.ClientRectangle,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid);
            };

            // Trending NFTs section
            Label lblTrendingNFTs = new Label();
            lblTrendingNFTs.Text = "TRENDING NFTs";
            lblTrendingNFTs.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTrendingNFTs.ForeColor = UIHelper.TextPrimary;
            lblTrendingNFTs.Size = new Size(300, 40);
            lblTrendingNFTs.Location = new Point(490, 320);
            lblTrendingNFTs.TextAlign = ContentAlignment.MiddleLeft;

            // Trending NFTs panel
            trendingNFTsPanel = new FlowLayoutPanel();
            trendingNFTsPanel.Size = new Size(450, 350);
            trendingNFTsPanel.Location = new Point(490, 370);
            trendingNFTsPanel.BackColor = Color.FromArgb(30, 30, 40);
            trendingNFTsPanel.AutoScroll = true;
            trendingNFTsPanel.Padding = new Padding(10);
            trendingNFTsPanel.BorderStyle = BorderStyle.None;
            trendingNFTsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, trendingNFTsPanel.ClientRectangle,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(50, 50, 60), 1, ButtonBorderStyle.Solid);
            };

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(statsContainer);
            this.Controls.Add(lblRecentActivities);
            this.Controls.Add(recentActivitiesPanel);
            this.Controls.Add(lblTrendingNFTs);
            this.Controls.Add(trendingNFTsPanel);
        }

        private Label CreateStatCard(Panel container, int index, string title, string value, Color color, string icon)
        {
            int cardWidth = 220;
            int cardHeight = 150;
            int spacing = 10;
            int x = index * (cardWidth + spacing);

            Panel cardPanel = new Panel();
            cardPanel.Size = new Size(cardWidth, cardHeight);
            cardPanel.Location = new Point(x, 0);
            cardPanel.BackColor = Color.FromArgb(35, 35, 45);
            cardPanel.Paint += (s, e) =>
            {
                // Draw border
                using (Pen pen = new Pen(color, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, cardPanel.Width - 3, cardPanel.Height - 3);
                }
            };

            // Icon
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 24);
            lblIcon.ForeColor = color;
            lblIcon.Size = new Size(50, 50);
            lblIcon.Location = new Point(20, 20);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Title
            Label lblCardTitle = new Label();
            lblCardTitle.Text = title;
            lblCardTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCardTitle.ForeColor = UIHelper.TextSecondary;
            lblCardTitle.Size = new Size(180, 25);
            lblCardTitle.Location = new Point(20, 75);
            lblCardTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Value
            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Size = new Size(180, 40);
            lblValue.Location = new Point(20, 100);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            // Add to card
            cardPanel.Controls.Add(lblIcon);
            cardPanel.Controls.Add(lblCardTitle);
            cardPanel.Controls.Add(lblValue);

            // Add card to container
            container.Controls.Add(cardPanel);

            return lblValue;
        }

        private void LoadDashboardData()
        {
            try
            {
                // Load statistics
                LoadStatistics();

                // Load recent activities
                LoadRecentActivities();

                // Load trending NFTs
                LoadTrendingNFTs();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to load dashboard data: {ex.Message}");
            }
        }

        private void LoadStatistics()
        {
            int userId = SessionManager.Instance.CurrentUserID;

            // Get owned NFTs count
            string queryNFTs = @"
                SELECT COUNT(*) as Count 
                FROM NFTs 
                WHERE OwnerID = @UserID AND IsSold = 0";

            SqlParameter[] nftParams = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId)
            };

            int ownedNFTs = Convert.ToInt32(DBHelper.ExecuteScalar(queryNFTs, nftParams));
            lblOwnedNFTs.Text = ownedNFTs.ToString();

            // Get active bids count
            string queryBids = @"
                SELECT COUNT(DISTINCT b.NFTID) as Count 
                FROM Bids b
                INNER JOIN NFTs n ON b.NFTID = n.NFTID
                WHERE b.UserID = @UserID AND n.IsSold = 0";

            int activeBids = Convert.ToInt32(DBHelper.ExecuteScalar(queryBids, nftParams));
            lblActiveBids.Text = activeBids.ToString();

            // Get total spent
            string querySpent = @"
                SELECT ISNULL(SUM(Amount), 0) as TotalSpent 
                FROM Transactions 
                WHERE UserID = @UserID 
                AND TransactionType IN ('PURCHASE', 'BID')";

            decimal totalSpent = Convert.ToDecimal(DBHelper.ExecuteScalar(querySpent, nftParams));
            lblTotalSpent.Text = totalSpent.ToString("C2");

            // Update balance (already in session)
            lblTotalBalance.Text = SessionManager.Instance.GetFormattedBalance();
        }

        private void LoadRecentActivities()
        {
            recentActivitiesPanel.Controls.Clear();

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

            recentActivities = DBHelper.ExecuteQuery(query, parameters);

            if (recentActivities.Rows.Count == 0)
            {
                Label noDataLabel = new Label();
                noDataLabel.Text = "No recent activities";
                noDataLabel.Font = new Font("Segoe UI", 11);
                noDataLabel.ForeColor = UIHelper.TextSecondary;
                noDataLabel.Size = new Size(400, 40);
                noDataLabel.TextAlign = ContentAlignment.MiddleCenter;
                recentActivitiesPanel.Controls.Add(noDataLabel);
                return;
            }

            foreach (DataRow row in recentActivities.Rows)
            {
                Panel activityCard = CreateActivityCard(row);
                recentActivitiesPanel.Controls.Add(activityCard);
            }
        }

        private Panel CreateActivityCard(DataRow row)
        {
            Panel card = new Panel();
            card.Size = new Size(410, 70);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Margin = new Padding(0, 0, 0, 10);
            card.Padding = new Padding(10);

            string transactionType = row["TransactionType"].ToString();
            decimal amount = Convert.ToDecimal(row["Amount"]);
            string description = row["Description"].ToString();
            DateTime date = Convert.ToDateTime(row["TransactionDate"]);
            string nftTitle = row["NFTTitle"].ToString();

            // Determine icon and color based on transaction type
            string icon = GetTransactionIcon(transactionType);
            Color color = GetTransactionColor(transactionType);
            string amountPrefix = GetAmountPrefix(transactionType);

            // Icon
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 14);
            lblIcon.ForeColor = color;
            lblIcon.Size = new Size(40, 40);
            lblIcon.Location = new Point(10, 15);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = !string.IsNullOrEmpty(nftTitle) ? $"{description}: {nftTitle}" : description;
            lblDescription.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblDescription.ForeColor = UIHelper.TextPrimary;
            lblDescription.Size = new Size(250, 25);
            lblDescription.Location = new Point(60, 10);
            lblDescription.TextAlign = ContentAlignment.MiddleLeft;

            // Amount
            Label lblAmount = new Label();
            lblAmount.Text = $"{amountPrefix}{amount:C2}";
            lblAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAmount.ForeColor = color;
            lblAmount.Size = new Size(100, 25);
            lblAmount.Location = new Point(320, 10);
            lblAmount.TextAlign = ContentAlignment.MiddleRight;

            // Date
            Label lblDate = new Label();
            lblDate.Text = FormatDate(date);
            lblDate.Font = new Font("Segoe UI", 9);
            lblDate.ForeColor = UIHelper.TextSecondary;
            lblDate.Size = new Size(250, 20);
            lblDate.Location = new Point(60, 35);
            lblDate.TextAlign = ContentAlignment.MiddleLeft;

            // Add controls
            card.Controls.Add(lblIcon);
            card.Controls.Add(lblDescription);
            card.Controls.Add(lblAmount);
            card.Controls.Add(lblDate);

            // Add hover effect
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(50, 50, 60);
            card.MouseLeave += (s, e) => card.BackColor = Color.FromArgb(40, 40, 50);

            return card;
        }

        private string GetTransactionIcon(string transactionType)
        {
            switch (transactionType)
            {
                case "PURCHASE": return "🛒";
                case "SALE": return "💰";
                case "BID": return "⚡";
                case "ADD_FUNDS": return "➕";
                case "REGISTRATION": return "👤";
                case "LOGIN": return "🔑";
                default: return "📄";
            }
        }

        private Color GetTransactionColor(string transactionType)
        {
            switch (transactionType)
            {
                case "PURCHASE": return UIHelper.ErrorColor;
                case "SALE": return UIHelper.SuccessColor;
                case "BID": return UIHelper.BlueAccent;
                case "ADD_FUNDS": return UIHelper.GoldAccent;
                default: return UIHelper.TextSecondary;
            }
        }

        private string GetAmountPrefix(string transactionType)
        {
            switch (transactionType)
            {
                case "PURCHASE": return "-";
                case "SALE": return "+";
                case "ADD_FUNDS": return "+";
                default: return "";
            }
        }

        private string FormatDate(DateTime date)
        {
            TimeSpan timeDiff = DateTime.Now - date;

            if (timeDiff.TotalMinutes < 1)
                return "Just now";
            else if (timeDiff.TotalHours < 1)
                return $"{(int)timeDiff.TotalMinutes} minutes ago";
            else if (timeDiff.TotalDays < 1)
                return $"{(int)timeDiff.TotalHours} hours ago";
            else if (timeDiff.TotalDays < 7)
                return $"{(int)timeDiff.TotalDays} days ago";
            else
                return date.ToString("MMM dd, yyyy");
        }

        private void LoadTrendingNFTs()
        {
            trendingNFTsPanel.Controls.Clear();

            string query = @"
                SELECT TOP 6 
                    n.NFTID,
                    n.Title,
                    n.Price,
                    n.CurrentBid,
                    n.ImagePath,
                    n.CreatedBy,
                    n.Category,
                    (SELECT COUNT(*) FROM Bids b WHERE b.NFTID = n.NFTID) as BidCount,
                    n.Views
                FROM NFTs n
                WHERE n.IsSold = 0
                ORDER BY n.Views DESC, BidCount DESC, n.CreatedDate DESC";

            trendingNFTs = DBHelper.ExecuteQuery(query);

            if (trendingNFTs.Rows.Count == 0)
            {
                Label noDataLabel = new Label();
                noDataLabel.Text = "No trending NFTs available";
                noDataLabel.Font = new Font("Segoe UI", 11);
                noDataLabel.ForeColor = UIHelper.TextSecondary;
                noDataLabel.Size = new Size(400, 40);
                noDataLabel.TextAlign = ContentAlignment.MiddleCenter;
                trendingNFTsPanel.Controls.Add(noDataLabel);
                return;
            }

            foreach (DataRow row in trendingNFTs.Rows)
            {
                Panel nftCard = CreateTrendingNFTCard(row);
                trendingNFTsPanel.Controls.Add(nftCard);
            }
        }

        private Panel CreateTrendingNFTCard(DataRow row)
        {
            int cardWidth = 200;
            int cardHeight = 250;

            Panel card = new Panel();
            card.Size = new Size(cardWidth, cardHeight);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Margin = new Padding(0, 0, 10, 10);
            card.Padding = new Padding(10);
            card.Cursor = Cursors.Hand;

            string title = row["Title"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            decimal? currentBid = row["CurrentBid"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBid"]) : (decimal?)null;
            string creator = row["CreatedBy"].ToString();
            string category = row["Category"].ToString();
            int bidCount = Convert.ToInt32(row["BidCount"]);
            int views = Convert.ToInt32(row["Views"]);
            int nftId = Convert.ToInt32(row["NFTID"]);

            // Title (truncate if too long)
            string displayTitle = title.Length > 20 ? title.Substring(0, 17) + "..." : title;

            Label lblTitle = new Label();
            lblTitle.Text = displayTitle;
            lblTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.TextPrimary;
            lblTitle.Size = new Size(180, 25);
            lblTitle.Location = new Point(10, 10);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Category badge
            Label lblCategory = new Label();
            lblCategory.Text = category;
            lblCategory.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblCategory.ForeColor = UIHelper.BlueAccent;
            lblCategory.BackColor = Color.FromArgb(30, 30, 45);
            lblCategory.Size = new Size(60, 20);
            lblCategory.Location = new Point(10, 40);
            lblCategory.TextAlign = ContentAlignment.MiddleCenter;
            lblCategory.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, lblCategory.ClientRectangle,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid);
            };

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = $"Price: {price:C2}";
            lblPrice.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPrice.ForeColor = UIHelper.GoldAccent;
            lblPrice.Size = new Size(180, 25);
            lblPrice.Location = new Point(10, 150);
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;

            // Bid info
            Label lblBidInfo = new Label();
            if (currentBid.HasValue)
            {
                lblBidInfo.Text = $"Current: {currentBid.Value:C2}";
                lblBidInfo.ForeColor = UIHelper.BlueAccent;
            }
            else
            {
                lblBidInfo.Text = "No bids yet";
                lblBidInfo.ForeColor = UIHelper.TextSecondary;
            }
            lblBidInfo.Font = new Font("Segoe UI", 9);
            lblBidInfo.Size = new Size(180, 20);
            lblBidInfo.Location = new Point(10, 175);
            lblBidInfo.TextAlign = ContentAlignment.MiddleLeft;

            // Stats
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(180, 40);
            statsPanel.Location = new Point(10, 200);
            statsPanel.BackColor = Color.Transparent;

            // Bids
            Label lblBids = new Label();
            lblBids.Text = $"🔨 {bidCount}";
            lblBids.Font = new Font("Segoe UI", 9);
            lblBids.ForeColor = UIHelper.TextSecondary;
            lblBids.Size = new Size(60, 20);
            lblBids.Location = new Point(0, 10);
            lblBids.TextAlign = ContentAlignment.MiddleLeft;

            // Views
            Label lblViews = new Label();
            lblViews.Text = $"👁️ {views}";
            lblViews.Font = new Font("Segoe UI", 9);
            lblViews.ForeColor = UIHelper.TextSecondary;
            lblViews.Size = new Size(60, 20);
            lblViews.Location = new Point(70, 10);
            lblViews.TextAlign = ContentAlignment.MiddleLeft;

            statsPanel.Controls.Add(lblBids);
            statsPanel.Controls.Add(lblViews);

            // Creator
            Label lblCreator = new Label();
            lblCreator.Text = $"By: {creator}";
            lblCreator.Font = new Font("Segoe UI", 8);
            lblCreator.ForeColor = UIHelper.TextSecondary;
            lblCreator.Size = new Size(180, 20);
            lblCreator.Location = new Point(10, 65);
            lblCreator.TextAlign = ContentAlignment.MiddleLeft;

            // Add controls
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblCategory);
            card.Controls.Add(lblCreator);
            card.Controls.Add(lblPrice);
            card.Controls.Add(lblBidInfo);
            card.Controls.Add(statsPanel);

            // Image placeholder (simulated)
            Panel imagePlaceholder = new Panel();
            imagePlaceholder.Size = new Size(180, 80);
            imagePlaceholder.Location = new Point(10, 90);
            imagePlaceholder.BackColor = Color.FromArgb(50, 50, 65);

            // Draw simulated image
            imagePlaceholder.Paint += (s, e) =>
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 75)),
                    imagePlaceholder.ClientRectangle);

                // Draw abstract pattern
                using (Pen pen = new Pen(Color.FromArgb(100, UIHelper.GoldAccent), 1))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int x1 = i * 20;
                        int y1 = 0;
                        int x2 = i * 20;
                        int y2 = 80;
                        e.Graphics.DrawLine(pen, x1, y1, x2, y2);
                    }
                }

                // Draw NFT text
                using (Font font = new Font("Segoe UI", 9, FontStyle.Bold))
                using (Brush brush = new SolidBrush(UIHelper.BlueAccent))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString("NFT IMAGE", font, brush,
                        new Rectangle(0, 0, 180, 80), format);
                }
            };

            card.Controls.Add(imagePlaceholder);

            // Hover effect
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(50, 50, 60);
                card.Size = new Size(cardWidth + 2, cardHeight + 2);
                card.Location = new Point(card.Location.X - 1, card.Location.Y - 1);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.FromArgb(40, 40, 50);
                card.Size = new Size(cardWidth, cardHeight);
                card.Location = new Point(card.Location.X + 1, card.Location.Y + 1);
            };

            // Click event
            card.Click += (s, e) =>
            {
                // Will be implemented when we create NFT detail view
                MessageBox.Show($"Viewing NFT: {title}\n\nPrice: {price:C2}\nCreator: {creator}\nCategory: {category}\nBids: {bidCount}\nViews: {views}",
                    "NFT Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            return card;
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        public void RefreshData()
        {
            // Show loading
            btnRefresh.Enabled = false;
            btnRefresh.Text = "Loading...";

            // Refresh data
            LoadDashboardData();

            // Restore button
            btnRefresh.Enabled = true;
            btnRefresh.Text = "🔄 Refresh";

            // Show confirmation
            UIHelper.ShowMessage("Dashboard refreshed successfully!", "Refresh Complete");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw decorative lines
            using (Pen pen = new Pen(Color.FromArgb(40, 40, 50), 2))
            {
                // Horizontal line under stats
                e.Graphics.DrawLine(pen, 20, 310, 930, 310);

                // Vertical line between sections
                e.Graphics.DrawLine(pen, 480, 370, 480, 720);
            }
        }
    }
}