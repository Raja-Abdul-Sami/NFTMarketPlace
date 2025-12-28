using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TriApex.Forms;
using TriApex.Helpers;

namespace TriApex.UserControls
{
    public partial class MyNFTsControl : UserControl
    {
        // UI Components
        private TabControl tabControl;
        private TabPage tabOwnedNFTs;
        private TabPage tabSoldNFTs;
        private TabPage tabBidHistory;
        private TabPage tabListedNFTs;

        // Owned NFTs components
        private FlowLayoutPanel ownedNFTsPanel;
        private Label lblOwnedCount;
        private Label lblOwnedValue;

        // Sold NFTs components
        private FlowLayoutPanel soldNFTsPanel;
        private Label lblSoldCount;
        private Label lblTotalEarnings;

        // Bid History components
        private DataGridView dgvBidHistory;
        private Label lblActiveBids;
        private Label lblWinningBids;

        // Listed NFTs components
        private FlowLayoutPanel listedNFTsPanel;
        private Button btnListNewNFT;

        // Data
        private DataTable ownedNFTs;
        private DataTable soldNFTs;
        private DataTable bidHistory;
        private DataTable listedNFTs;

        // Constants
        private const int CARD_WIDTH = 220;
        private const int CARD_HEIGHT = 320;

        public MyNFTsControl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadAllData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // MyNFTsControl
            this.BackColor = Color.Transparent;
            this.Size = new Size(950, 730);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "MY NFTs & PORTFOLIO";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Tab control
            tabControl = new TabControl();
            tabControl.Size = new Size(910, 650);
            tabControl.Location = new Point(20, 80);
            tabControl.Font = new Font("Segoe UI", 10);
            tabControl.Appearance = TabAppearance.FlatButtons;
            tabControl.ItemSize = new Size(120, 40);
            tabControl.SelectedIndex = 0;

            // Style tab control
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.DrawItem += TabControl_DrawItem;

            // Tab 1: Owned NFTs
            tabOwnedNFTs = new TabPage("OWNED NFTs");
            tabOwnedNFTs.BackColor = UIHelper.PanelBackground;
            tabOwnedNFTs.Padding = new Padding(10);
            InitializeOwnedNFTsTab();

            // Tab 2: Sold NFTs
            tabSoldNFTs = new TabPage("SOLD NFTs");
            tabSoldNFTs.BackColor = UIHelper.PanelBackground;
            tabSoldNFTs.Padding = new Padding(10);
            InitializeSoldNFTsTab();

            // Tab 3: Bid History
            tabBidHistory = new TabPage("BID HISTORY");
            tabBidHistory.BackColor = UIHelper.PanelBackground;
            tabBidHistory.Padding = new Padding(10);
            InitializeBidHistoryTab();

            // Tab 4: Listed NFTs
            tabListedNFTs = new TabPage("LISTED NFTs");
            tabListedNFTs.BackColor = UIHelper.PanelBackground;
            tabListedNFTs.Padding = new Padding(10);
            InitializeListedNFTsTab();

            // Add tabs
            tabControl.TabPages.Add(tabOwnedNFTs);
            tabControl.TabPages.Add(tabSoldNFTs);
            tabControl.TabPages.Add(tabBidHistory);
            tabControl.TabPages.Add(tabListedNFTs);

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(tabControl);
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tc = sender as TabControl;
            TabPage tp = tc.TabPages[e.Index];

            using (Brush backBrush = new SolidBrush(UIHelper.PanelBackground))
            using (Brush textBrush = new SolidBrush(e.Index == tc.SelectedIndex ? UIHelper.GoldAccent : UIHelper.TextSecondary))
            using (Font font = new Font("Segoe UI", 10, e.Index == tc.SelectedIndex ? FontStyle.Bold : FontStyle.Regular))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);

                // Draw border for selected tab
                if (e.Index == tc.SelectedIndex)
                {
                    using (Pen borderPen = new Pen(UIHelper.GoldAccent, 2))
                    {
                        e.Graphics.DrawRectangle(borderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);
                    }
                }

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawString(tp.Text, font, textBrush, e.Bounds, sf);
            }
        }

        #region Owned NFTs Tab

        private void InitializeOwnedNFTsTab()
        {
            // Stats panel
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(880, 80);
            statsPanel.Location = new Point(10, 10);
            statsPanel.BackColor = Color.FromArgb(35, 35, 45);
            statsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, statsPanel.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Owned count
            Label lblCountTitle = new Label();
            lblCountTitle.Text = "OWNED NFTs";
            lblCountTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCountTitle.ForeColor = UIHelper.TextSecondary;
            lblCountTitle.Size = new Size(150, 30);
            lblCountTitle.Location = new Point(30, 10);
            lblCountTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblOwnedCount = new Label();
            lblOwnedCount.Text = "0";
            lblOwnedCount.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblOwnedCount.ForeColor = UIHelper.BlueAccent;
            lblOwnedCount.Size = new Size(150, 40);
            lblOwnedCount.Location = new Point(30, 35);
            lblOwnedCount.TextAlign = ContentAlignment.MiddleLeft;

            // Total value
            Label lblValueTitle = new Label();
            lblValueTitle.Text = "TOTAL VALUE";
            lblValueTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblValueTitle.ForeColor = UIHelper.TextSecondary;
            lblValueTitle.Size = new Size(150, 30);
            lblValueTitle.Location = new Point(230, 10);
            lblValueTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblOwnedValue = new Label();
            lblOwnedValue.Text = "$0.00";
            lblOwnedValue.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblOwnedValue.ForeColor = UIHelper.GoldAccent;
            lblOwnedValue.Size = new Size(200, 40);
            lblOwnedValue.Location = new Point(230, 35);
            lblOwnedValue.TextAlign = ContentAlignment.MiddleLeft;

            // Action buttons
            Button btnSellNFT = new Button();
            btnSellNFT.Text = "SELL NFT";
            btnSellNFT.Size = new Size(120, 40);
            btnSellNFT.Location = new Point(730, 20);
            UIHelper.StyleButton(btnSellNFT, true);
            btnSellNFT.Click += BtnSellNFT_Click;

            Button btnRefreshOwned = new Button();
            btnRefreshOwned.Text = "🔄 Refresh";
            btnRefreshOwned.Size = new Size(120, 40);
            btnRefreshOwned.Location = new Point(600, 20);
            UIHelper.StyleButton(btnRefreshOwned, false);
            btnRefreshOwned.Click += BtnRefreshOwned_Click;

            // NFT grid
            ownedNFTsPanel = new FlowLayoutPanel();
            ownedNFTsPanel.Size = new Size(880, 500);
            ownedNFTsPanel.Location = new Point(10, 100);
            ownedNFTsPanel.BackColor = Color.Transparent;
            ownedNFTsPanel.AutoScroll = true;
            ownedNFTsPanel.WrapContents = true;
            ownedNFTsPanel.Padding = new Padding(10);

            // Add controls
            statsPanel.Controls.Add(lblCountTitle);
            statsPanel.Controls.Add(lblOwnedCount);
            statsPanel.Controls.Add(lblValueTitle);
            statsPanel.Controls.Add(lblOwnedValue);
            statsPanel.Controls.Add(btnSellNFT);
            statsPanel.Controls.Add(btnRefreshOwned);

            tabOwnedNFTs.Controls.Add(statsPanel);
            tabOwnedNFTs.Controls.Add(ownedNFTsPanel);
        }

        private void LoadOwnedNFTs()
        {
            ownedNFTsPanel.Controls.Clear();

            string query = @"
                SELECT 
                    n.NFTID,
                    n.Title,
                    n.Description,
                    n.Price,
                    n.CurrentBid,
                    n.ImagePath,
                    n.ImageData,
                    n.CreatedBy,
                    n.Category,
                    n.Views,
                    n.CreatedDate,
                    (SELECT COUNT(*) FROM Bids b WHERE b.NFTID = n.NFTID) as BidCount
                FROM NFTs n
                WHERE n.OwnerID = @UserID AND n.IsSold = 0
                ORDER BY n.CreatedDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            ownedNFTs = DBHelper.ExecuteQuery(query, parameters);

            if (ownedNFTs.Rows.Count == 0)
            {
                ShowNoNFTsMessage(ownedNFTsPanel, "You don't own any NFTs yet.", "Start browsing to build your collection!");
                lblOwnedCount.Text = "0";
                lblOwnedValue.Text = "$0.00";
                return;
            }

            decimal totalValue = 0;

            foreach (DataRow row in ownedNFTs.Rows)
            {
                decimal price = Convert.ToDecimal(row["Price"]);
                totalValue += price;

                Panel card = CreateOwnedNFTCard(row);
                ownedNFTsPanel.Controls.Add(card);
            }

            // Update stats
            lblOwnedCount.Text = ownedNFTs.Rows.Count.ToString();
            lblOwnedValue.Text = totalValue.ToString("C2");
        }

        private Panel CreateOwnedNFTCard(DataRow row)
        {
            int nftId = Convert.ToInt32(row["NFTID"]);
            string title = row["Title"].ToString();
            string description = row["Description"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            decimal? currentBid = row["CurrentBid"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBid"]) : (decimal?)null;
            string creator = row["CreatedBy"].ToString();
            string category = row["Category"].ToString();
            int views = Convert.ToInt32(row["Views"]);
            int bidCount = Convert.ToInt32(row["BidCount"]);
            DateTime createdDate = Convert.ToDateTime(row["CreatedDate"]);

            Panel card = new Panel();
            card.Size = new Size(CARD_WIDTH, CARD_HEIGHT);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Padding = new Padding(10);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Rounded corners
            card.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, card.Width, card.Height, 12, 12));

            // Title
            string displayTitle = title.Length > 20 ? title.Substring(0, 17) + "..." : title;
            Label titleLabel = new Label();
            titleLabel.Text = displayTitle;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.ForeColor = UIHelper.TextPrimary;
            titleLabel.Size = new Size(200, 25);
            titleLabel.Location = new Point(10, 10);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Owned badge
            Label ownedBadge = new Label();
            ownedBadge.Text = "OWNED";
            ownedBadge.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            ownedBadge.ForeColor = UIHelper.SuccessColor;
            ownedBadge.BackColor = Color.FromArgb(30, 60, 40);
            ownedBadge.Size = new Size(60, 20);
            ownedBadge.Location = new Point(140, 10);
            ownedBadge.TextAlign = ContentAlignment.MiddleCenter;

            // Category
            Label categoryLabel = new Label();
            categoryLabel.Text = category;
            categoryLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            categoryLabel.ForeColor = UIHelper.BlueAccent;
            categoryLabel.Size = new Size(80, 20);
            categoryLabel.Location = new Point(10, 40);
            categoryLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Image placeholder
            Panel imagePanel = new Panel();
            imagePanel.Size = new Size(180, 120);
            imagePanel.Location = new Point(10, 65);
            imagePanel.BackColor = Color.FromArgb(50, 50, 65);
            DataRow nftRow = row; // capture the current row for the lambda
            imagePanel.Paint += (s, e) => DrawOwnedNFTImage(e.Graphics, imagePanel.ClientRectangle, nftRow);

            // Price
            Label priceLabel = new Label();
            priceLabel.Text = $"Value: {price:C2}";
            priceLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            priceLabel.ForeColor = UIHelper.GoldAccent;
            priceLabel.Size = new Size(200, 25);
            priceLabel.Location = new Point(10, 195);
            priceLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Current bid (if any)
            Label bidLabel = new Label();
            if (currentBid.HasValue)
            {
                bidLabel.Text = $"Highest Bid: {currentBid.Value:C2}";
                bidLabel.ForeColor = UIHelper.BlueAccent;
            }
            else
            {
                bidLabel.Text = "No active bids";
                bidLabel.ForeColor = UIHelper.TextSecondary;
            }
            bidLabel.Font = new Font("Segoe UI", 9);
            bidLabel.Size = new Size(200, 20);
            bidLabel.Location = new Point(10, 220);
            bidLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Stats
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(200, 40);
            statsPanel.Location = new Point(10, 245);
            statsPanel.BackColor = Color.Transparent;

            // Views
            Label viewsLabel = new Label();
            viewsLabel.Text = $"👁️ {views}";
            viewsLabel.Font = new Font("Segoe UI", 9);
            viewsLabel.ForeColor = UIHelper.TextSecondary;
            viewsLabel.Size = new Size(60, 20);
            viewsLabel.Location = new Point(0, 10);
            viewsLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Bids
            Label bidsLabel = new Label();
            bidsLabel.Text = $"🔨 {bidCount}";
            bidsLabel.Font = new Font("Segoe UI", 9);
            bidsLabel.ForeColor = UIHelper.TextSecondary;
            bidsLabel.Size = new Size(60, 20);
            bidsLabel.Location = new Point(70, 10);
            bidsLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Created date
            Label dateLabel = new Label();
            dateLabel.Text = createdDate.ToString("MMM dd, yyyy");
            dateLabel.Font = new Font("Segoe UI", 8);
            dateLabel.ForeColor = UIHelper.TextSecondary;
            dateLabel.Size = new Size(200, 20);
            dateLabel.Location = new Point(10, 270);
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;

            statsPanel.Controls.Add(viewsLabel);
            statsPanel.Controls.Add(bidsLabel);

            // Action buttons
            Button btnSell = new Button();
            btnSell.Text = "SELL";
            btnSell.Size = new Size(90, 30);
            btnSell.Location = new Point(10, 295);
            UIHelper.StyleButton(btnSell, true);
            btnSell.Tag = nftId;
            btnSell.Click += BtnSellSpecific_Click;

            Button btnViewDetails = new Button();
            btnViewDetails.Text = "DETAILS";
            btnViewDetails.Size = new Size(90, 30);
            btnViewDetails.Location = new Point(110, 295);
            UIHelper.StyleButton(btnViewDetails, false);
            btnViewDetails.Tag = nftId;
            btnViewDetails.Click += BtnViewDetails_Click;

            // Add controls
            card.Controls.Add(titleLabel);
            card.Controls.Add(ownedBadge);
            card.Controls.Add(categoryLabel);
            card.Controls.Add(imagePanel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(bidLabel);
            card.Controls.Add(statsPanel);
            card.Controls.Add(dateLabel);
            card.Controls.Add(btnSell);
            card.Controls.Add(btnViewDetails);

            // Hover effect
            UIHelper.ApplyHoverEffect(card, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            return card;
        }

        private void DrawOwnedNFTImage(Graphics g, Rectangle bounds, DataRow row)
        {
            if (row["ImageData"] != DBNull.Value)
            {
                byte[] imageBytes = (byte[])row["ImageData"];
                using (var ms = new System.IO.MemoryStream(imageBytes))
                using (Image img = Image.FromStream(ms))
                {
                    g.DrawImage(img, bounds);
                }
            }
            else if (row["ImagePath"] != DBNull.Value)
            {
                string path = row["ImagePath"].ToString();
                if (System.IO.File.Exists(path))
                {
                    using (Image img = Image.FromFile(path))
                    {
                        g.DrawImage(img, bounds);
                    }
                }
                else
                {
                    DrawPlaceholder(g, bounds, row["Title"].ToString());
                }
            }
            else
            {
                DrawPlaceholder(g, bounds, row["Title"].ToString());
            }
        }



        private void DrawPlaceholder(Graphics g, Rectangle bounds, string title)
        {
            // Fill with gradient
            using (var brush = UIHelper.CreateGradientBrush(bounds,
                Color.FromArgb(60, 60, 75),
                Color.FromArgb(50, 50, 65)))
            {
                g.FillRectangle(brush, bounds);
            }

            // Draw random pattern
            Random rand = new Random(title.GetHashCode());
            using (Pen goldPen = new Pen(Color.FromArgb(100, UIHelper.GoldAccent), 2))
            using (Pen bluePen = new Pen(Color.FromArgb(100, UIHelper.BlueAccent), 2))
            {
                for (int i = 0; i < 8; i++)
                {
                    int x1 = rand.Next(bounds.Width);
                    int y1 = rand.Next(bounds.Height);
                    int x2 = rand.Next(bounds.Width);
                    int y2 = rand.Next(bounds.Height);

                    if (rand.Next(2) == 0)
                        g.DrawLine(goldPen, x1, y1, x2, y2);
                    else
                        g.DrawLine(bluePen, x1, y1, x2, y2);
                }
            }

            // Draw title
            using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
            using (Brush brush = new SolidBrush(UIHelper.TextPrimary))
            {
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                string displayText = title.Length > 15 ? title.Substring(0, 12) + "..." : title;
                g.DrawString(displayText, font, brush, bounds, format);
            }
        }

        #endregion

        #region Sold NFTs Tab

        private void InitializeSoldNFTsTab()
        {
            // Stats panel
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(880, 80);
            statsPanel.Location = new Point(10, 10);
            statsPanel.BackColor = Color.FromArgb(35, 35, 45);
            statsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, statsPanel.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Sold count
            Label lblSoldCountTitle = new Label();
            lblSoldCountTitle.Text = "SOLD NFTs";
            lblSoldCountTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblSoldCountTitle.ForeColor = UIHelper.TextSecondary;
            lblSoldCountTitle.Size = new Size(150, 30);
            lblSoldCountTitle.Location = new Point(30, 10);
            lblSoldCountTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblSoldCount = new Label();
            lblSoldCount.Text = "0";
            lblSoldCount.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblSoldCount.ForeColor = UIHelper.BlueAccent;
            lblSoldCount.Size = new Size(150, 40);
            lblSoldCount.Location = new Point(30, 35);
            lblSoldCount.TextAlign = ContentAlignment.MiddleLeft;

            // Total earnings
            Label lblEarningsTitle = new Label();
            lblEarningsTitle.Text = "TOTAL EARNINGS";
            lblEarningsTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEarningsTitle.ForeColor = UIHelper.TextSecondary;
            lblEarningsTitle.Size = new Size(150, 30);
            lblEarningsTitle.Location = new Point(230, 10);
            lblEarningsTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblTotalEarnings = new Label();
            lblTotalEarnings.Text = "$0.00";
            lblTotalEarnings.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTotalEarnings.ForeColor = UIHelper.GoldAccent;
            lblTotalEarnings.Size = new Size(200, 40);
            lblTotalEarnings.Location = new Point(230, 35);
            lblTotalEarnings.TextAlign = ContentAlignment.MiddleLeft;

            // Refresh button
            Button btnRefreshSold = new Button();
            btnRefreshSold.Text = "🔄 Refresh";
            btnRefreshSold.Size = new Size(120, 40);
            btnRefreshSold.Location = new Point(730, 20);
            UIHelper.StyleButton(btnRefreshSold, false);
            btnRefreshSold.Click += BtnRefreshSold_Click;

            // Sold NFTs panel
            soldNFTsPanel = new FlowLayoutPanel();
            soldNFTsPanel.Size = new Size(880, 500);
            soldNFTsPanel.Location = new Point(10, 100);
            soldNFTsPanel.BackColor = Color.Transparent;
            soldNFTsPanel.AutoScroll = true;
            soldNFTsPanel.WrapContents = true;
            soldNFTsPanel.Padding = new Padding(10);

            // Add controls
            statsPanel.Controls.Add(lblSoldCountTitle);
            statsPanel.Controls.Add(lblSoldCount);
            statsPanel.Controls.Add(lblEarningsTitle);
            statsPanel.Controls.Add(lblTotalEarnings);
            statsPanel.Controls.Add(btnRefreshSold);

            tabSoldNFTs.Controls.Add(statsPanel);
            tabSoldNFTs.Controls.Add(soldNFTsPanel);
        }

        private void LoadSoldNFTs()
        {
            soldNFTsPanel.Controls.Clear();

            string query = @"
                SELECT 
                    n.NFTID,
                    n.Title,
                    n.Description,
                    n.Price as SalePrice,
                    n.CreatedBy,
                    n.Category,
                    n.CreatedDate,
                    (SELECT Username FROM Users u WHERE u.UserID = n.OwnerID) as Buyer,
                    (SELECT MAX(TransactionDate) FROM Transactions t WHERE t.NFTID = n.NFTID AND t.TransactionType = 'SALE') as SaleDate
                FROM NFTs n
                WHERE n.CreatedBy = @Username AND n.IsSold = 1
                ORDER BY n.CreatedDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SessionManager.Instance.Username)
            };

            soldNFTs = DBHelper.ExecuteQuery(query, parameters);

            if (soldNFTs.Rows.Count == 0)
            {
                ShowNoNFTsMessage(soldNFTsPanel, "No NFTs sold yet.", "List your NFTs for sale to start earning!");
                lblSoldCount.Text = "0";
                lblTotalEarnings.Text = "$0.00";
                return;
            }

            decimal totalEarnings = 0;

            foreach (DataRow row in soldNFTs.Rows)
            {
                decimal salePrice = Convert.ToDecimal(row["SalePrice"]);
                totalEarnings += salePrice;

                Panel card = CreateSoldNFTCard(row);
                soldNFTsPanel.Controls.Add(card);
            }

            // Update stats
            lblSoldCount.Text = soldNFTs.Rows.Count.ToString();
            lblTotalEarnings.Text = totalEarnings.ToString("C2");
        }

        private Panel CreateSoldNFTCard(DataRow row)
        {
            string title = row["Title"].ToString();
            decimal salePrice = Convert.ToDecimal(row["SalePrice"]);
            string buyer = row["Buyer"].ToString();
            DateTime? saleDate = row["SaleDate"] != DBNull.Value ? Convert.ToDateTime(row["SaleDate"]) : (DateTime?)null;
            string category = row["Category"].ToString();

            Panel card = new Panel();
            card.Size = new Size(CARD_WIDTH, 200);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Padding = new Padding(10);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Rounded corners
            card.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, card.Width, card.Height, 12, 12));

            // Sold badge
            Label soldBadge = new Label();
            soldBadge.Text = "SOLD";
            soldBadge.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            soldBadge.ForeColor = UIHelper.GoldAccent;
            soldBadge.BackColor = Color.FromArgb(60, 50, 30);
            soldBadge.Size = new Size(60, 25);
            soldBadge.Location = new Point(10, 10);
            soldBadge.TextAlign = ContentAlignment.MiddleCenter;

            // Title
            string displayTitle = title.Length > 25 ? title.Substring(0, 22) + "..." : title;
            Label titleLabel = new Label();
            titleLabel.Text = displayTitle;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.ForeColor = UIHelper.TextPrimary;
            titleLabel.Size = new Size(180, 25);
            titleLabel.Location = new Point(10, 40);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Sale price
            Label priceLabel = new Label();
            priceLabel.Text = $"Sold for: {salePrice:C2}";
            priceLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            priceLabel.ForeColor = UIHelper.GoldAccent;
            priceLabel.Size = new Size(180, 25);
            priceLabel.Location = new Point(10, 70);
            priceLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Buyer
            Label buyerLabel = new Label();
            buyerLabel.Text = $"To: {buyer}";
            buyerLabel.Font = new Font("Segoe UI", 9);
            buyerLabel.ForeColor = UIHelper.BlueAccent;
            buyerLabel.Size = new Size(180, 20);
            buyerLabel.Location = new Point(10, 100);
            buyerLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Sale date
            Label dateLabel = new Label();
            if (saleDate.HasValue)
            {
                dateLabel.Text = $"On: {saleDate.Value:MMM dd, yyyy}";
            }
            else
            {
                dateLabel.Text = "Date: N/A";
            }
            dateLabel.Font = new Font("Segoe UI", 8);
            dateLabel.ForeColor = UIHelper.TextSecondary;
            dateLabel.Size = new Size(180, 20);
            dateLabel.Location = new Point(10, 125);
            dateLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Category
            Label categoryLabel = new Label();
            categoryLabel.Text = category;
            categoryLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            categoryLabel.ForeColor = UIHelper.TextSecondary;
            categoryLabel.Size = new Size(80, 20);
            categoryLabel.Location = new Point(120, 10);
            categoryLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls
            card.Controls.Add(soldBadge);
            card.Controls.Add(titleLabel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(buyerLabel);
            card.Controls.Add(dateLabel);
            card.Controls.Add(categoryLabel);

            // Hover effect
            UIHelper.ApplyHoverEffect(card, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            return card;
        }

        #endregion

        #region Bid History Tab

        private void InitializeBidHistoryTab()
        {
            // Stats panel
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(880, 80);
            statsPanel.Location = new Point(10, 10);
            statsPanel.BackColor = Color.FromArgb(35, 35, 45);
            statsPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, statsPanel.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Active bids
            Label lblActiveTitle = new Label();
            lblActiveTitle.Text = "ACTIVE BIDS";
            lblActiveTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblActiveTitle.ForeColor = UIHelper.TextSecondary;
            lblActiveTitle.Size = new Size(150, 30);
            lblActiveTitle.Location = new Point(30, 10);
            lblActiveTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblActiveBids = new Label();
            lblActiveBids.Text = "0";
            lblActiveBids.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblActiveBids.ForeColor = UIHelper.BlueAccent;
            lblActiveBids.Size = new Size(150, 40);
            lblActiveBids.Location = new Point(30, 35);
            lblActiveBids.TextAlign = ContentAlignment.MiddleLeft;

            // Winning bids
            Label lblWinningTitle = new Label();
            lblWinningTitle.Text = "WINNING BIDS";
            lblWinningTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblWinningTitle.ForeColor = UIHelper.TextSecondary;
            lblWinningTitle.Size = new Size(150, 30);
            lblWinningTitle.Location = new Point(230, 10);
            lblWinningTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblWinningBids = new Label();
            lblWinningBids.Text = "0";
            lblWinningBids.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblWinningBids.ForeColor = UIHelper.GoldAccent;
            lblWinningBids.Size = new Size(150, 40);
            lblWinningBids.Location = new Point(230, 35);
            lblWinningBids.TextAlign = ContentAlignment.MiddleLeft;

            // Refresh button
            Button btnRefreshBids = new Button();
            btnRefreshBids.Text = "🔄 Refresh";
            btnRefreshBids.Size = new Size(120, 40);
            btnRefreshBids.Location = new Point(730, 20);
            UIHelper.StyleButton(btnRefreshBids, false);
            btnRefreshBids.Click += BtnRefreshBids_Click;

            // DataGridView for bid history
            dgvBidHistory = new DataGridView();
            dgvBidHistory.Size = new Size(880, 500);
            dgvBidHistory.Location = new Point(10, 100);
            dgvBidHistory.BackgroundColor = Color.FromArgb(40, 40, 50);
            dgvBidHistory.BorderStyle = BorderStyle.None;
            dgvBidHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBidHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBidHistory.RowHeadersVisible = false;
            dgvBidHistory.AllowUserToAddRows = false;
            dgvBidHistory.AllowUserToDeleteRows = false;
            dgvBidHistory.ReadOnly = true;
            dgvBidHistory.DefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvBidHistory.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 60);
            dgvBidHistory.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvBidHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 70);
            dgvBidHistory.ColumnHeadersDefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvBidHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvBidHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 55);

            // Style scrollbars
            dgvBidHistory.EnableHeadersVisualStyles = false;

            // Add controls
            statsPanel.Controls.Add(lblActiveTitle);
            statsPanel.Controls.Add(lblActiveBids);
            statsPanel.Controls.Add(lblWinningTitle);
            statsPanel.Controls.Add(lblWinningBids);
            statsPanel.Controls.Add(btnRefreshBids);

            tabBidHistory.Controls.Add(statsPanel);
            tabBidHistory.Controls.Add(dgvBidHistory);
        }

        private void LoadBidHistory()
        {
            string query = @"
                SELECT 
                    b.BidID,
                    n.Title as NFTTitle,
                    b.BidAmount,
                    b.BidDate,
                    CASE 
                        WHEN b.BidAmount = n.CurrentBid THEN 'Winning'
                        WHEN n.IsSold = 1 AND n.CurrentBid = b.BidAmount THEN 'Won'
                        WHEN n.IsSold = 1 AND n.CurrentBid != b.BidAmount THEN 'Lost'
                        ELSE 'Active'
                    END as Status,
                    n.IsSold,
                    n.CurrentBid
                FROM Bids b
                INNER JOIN NFTs n ON b.NFTID = n.NFTID
                WHERE b.UserID = @UserID
                ORDER BY b.BidDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            bidHistory = DBHelper.ExecuteQuery(query, parameters);

            // Configure DataGridView
            dgvBidHistory.DataSource = bidHistory;

            // Format columns
            if (dgvBidHistory.Columns.Count > 0)
            {
                // Hide unnecessary columns
                dgvBidHistory.Columns["BidID"].Visible = false;
                dgvBidHistory.Columns["IsSold"].Visible = false;
                dgvBidHistory.Columns["CurrentBid"].Visible = false;

                // Format BidAmount column
                dgvBidHistory.Columns["BidAmount"].HeaderText = "Bid Amount";
                dgvBidHistory.Columns["BidAmount"].DefaultCellStyle.Format = "C2";
                dgvBidHistory.Columns["BidAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Format BidDate column
                dgvBidHistory.Columns["BidDate"].HeaderText = "Date";
                dgvBidHistory.Columns["BidDate"].DefaultCellStyle.Format = "MMM dd, yyyy HH:mm";

                // Format Status column with colors
                dgvBidHistory.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Add cell formatting for status colors
                dgvBidHistory.CellFormatting += DgvBidHistory_CellFormatting;
            }

            // Calculate stats
            int activeBids = 0;
            int winningBids = 0;

            if (bidHistory.Rows.Count > 0)
            {
                foreach (DataRow row in bidHistory.Rows)
                {
                    string status = row["Status"].ToString();
                    if (status == "Active" || status == "Winning")
                        activeBids++;
                    if (status == "Winning" || status == "Won")
                        winningBids++;
                }
            }

            lblActiveBids.Text = activeBids.ToString();
            lblWinningBids.Text = winningBids.ToString();
        }

        private void DgvBidHistory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvBidHistory.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();

                switch (status)
                {
                    case "Winning":
                        e.CellStyle.ForeColor = UIHelper.GoldAccent;
                        e.CellStyle.Font = new Font(dgvBidHistory.Font, FontStyle.Bold);
                        break;
                    case "Won":
                        e.CellStyle.ForeColor = UIHelper.SuccessColor;
                        e.CellStyle.Font = new Font(dgvBidHistory.Font, FontStyle.Bold);
                        break;
                    case "Lost":
                        e.CellStyle.ForeColor = UIHelper.ErrorColor;
                        break;
                    case "Active":
                        e.CellStyle.ForeColor = UIHelper.BlueAccent;
                        break;
                }
            }
            else if (dgvBidHistory.Columns[e.ColumnIndex].Name == "BidAmount" && e.Value != null)
            {
                e.CellStyle.ForeColor = UIHelper.GoldAccent;
                e.CellStyle.Font = new Font(dgvBidHistory.Font, FontStyle.Bold);
            }
        }

        #endregion

        #region Listed NFTs Tab

        private void InitializeListedNFTsTab()
        {
            // Header panel
            Panel headerPanel = new Panel();
            headerPanel.Size = new Size(880, 60);
            headerPanel.Location = new Point(10, 10);
            headerPanel.BackColor = Color.Transparent;

            // Title
            Label lblListedTitle = new Label();
            lblListedTitle.Text = "NFTs LISTED FOR SALE";
            lblListedTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblListedTitle.ForeColor = UIHelper.GoldAccent;
            lblListedTitle.Size = new Size(300, 40);
            lblListedTitle.Location = new Point(10, 10);
            lblListedTitle.TextAlign = ContentAlignment.MiddleLeft;

            // List new NFT button
            btnListNewNFT = new Button();
            btnListNewNFT.Text = "➕ LIST NEW NFT";
            btnListNewNFT.Size = new Size(150, 40);
            btnListNewNFT.Location = new Point(720, 10);
            UIHelper.StyleButton(btnListNewNFT, true);
            btnListNewNFT.Click += BtnListNewNFT_Click;

            // Listed NFTs panel
            listedNFTsPanel = new FlowLayoutPanel();
            listedNFTsPanel.Size = new Size(880, 540);
            listedNFTsPanel.Location = new Point(10, 80);
            listedNFTsPanel.BackColor = Color.Transparent;
            listedNFTsPanel.AutoScroll = true;
            listedNFTsPanel.WrapContents = true;
            listedNFTsPanel.Padding = new Padding(10);

            // Add controls
            headerPanel.Controls.Add(lblListedTitle);
            headerPanel.Controls.Add(btnListNewNFT);

            tabListedNFTs.Controls.Add(headerPanel);
            tabListedNFTs.Controls.Add(listedNFTsPanel);
        }

        private void LoadListedNFTs()
        {
            listedNFTsPanel.Controls.Clear();

            string query = @"
                SELECT 
                    n.NFTID,
                    n.Title,
                    n.Description,
                    n.Price,
                    n.CurrentBid,
                    n.ImagePath,
                    n.Category,
                    n.Views,
                    n.CreatedDate,
                    (SELECT COUNT(*) FROM Bids b WHERE b.NFTID = n.NFTID) as BidCount,
                    (SELECT Username FROM Users u WHERE u.UserID = n.OwnerID) as CurrentOwner
                FROM NFTs n
                WHERE n.CreatedBy = @Username AND n.IsSold = 0 AND n.OwnerID != @UserID
                ORDER BY n.CreatedDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", SessionManager.Instance.Username),
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            listedNFTs = DBHelper.ExecuteQuery(query, parameters);

            if (listedNFTs.Rows.Count == 0)
            {
                ShowNoNFTsMessage(listedNFTsPanel, "No NFTs listed for sale.", "List your NFTs to start selling!");
                return;
            }

            foreach (DataRow row in listedNFTs.Rows)
            {
                Panel card = CreateListedNFTCard(row);
                listedNFTsPanel.Controls.Add(card);
            }
        }

        private Panel CreateListedNFTCard(DataRow row)
        {
            int nftId = Convert.ToInt32(row["NFTID"]);
            string title = row["Title"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            decimal? currentBid = row["CurrentBid"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBid"]) : (decimal?)null;
            string category = row["Category"].ToString();
            int views = Convert.ToInt32(row["Views"]);
            int bidCount = Convert.ToInt32(row["BidCount"]);
            string currentOwner = row["CurrentOwner"].ToString();
            DateTime createdDate = Convert.ToDateTime(row["CreatedDate"]);

            Panel card = new Panel();
            card.Size = new Size(CARD_WIDTH, 250);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Padding = new Padding(10);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Rounded corners
            card.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, card.Width, card.Height, 12, 12));

            // Listed badge
            Label listedBadge = new Label();
            listedBadge.Text = "LISTED";
            listedBadge.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            listedBadge.ForeColor = UIHelper.BlueAccent;
            listedBadge.BackColor = Color.FromArgb(30, 40, 60);
            listedBadge.Size = new Size(60, 20);
            listedBadge.Location = new Point(10, 10);
            listedBadge.TextAlign = ContentAlignment.MiddleCenter;

            // Title
            string displayTitle = title.Length > 25 ? title.Substring(0, 22) + "..." : title;
            Label titleLabel = new Label();
            titleLabel.Text = displayTitle;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.ForeColor = UIHelper.TextPrimary;
            titleLabel.Size = new Size(180, 25);
            titleLabel.Location = new Point(10, 35);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Price
            Label priceLabel = new Label();
            priceLabel.Text = $"Price: {price:C2}";
            priceLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            priceLabel.ForeColor = UIHelper.GoldAccent;
            priceLabel.Size = new Size(180, 25);
            priceLabel.Location = new Point(10, 65);
            priceLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Current bid
            Label bidLabel = new Label();
            if (currentBid.HasValue)
            {
                bidLabel.Text = $"Highest Bid: {currentBid.Value:C2}";
                bidLabel.ForeColor = UIHelper.BlueAccent;
            }
            else
            {
                bidLabel.Text = "No bids yet";
                bidLabel.ForeColor = UIHelper.TextSecondary;
            }
            bidLabel.Font = new Font("Segoe UI", 9);
            bidLabel.Size = new Size(180, 20);
            bidLabel.Location = new Point(10, 90);
            bidLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Current owner
            Label ownerLabel = new Label();
            ownerLabel.Text = $"Owner: {currentOwner}";
            ownerLabel.Font = new Font("Segoe UI", 9);
            ownerLabel.ForeColor = UIHelper.TextSecondary;
            ownerLabel.Size = new Size(180, 20);
            ownerLabel.Location = new Point(10, 115);
            ownerLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Stats
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(180, 30);
            statsPanel.Location = new Point(10, 140);
            statsPanel.BackColor = Color.Transparent;

            // Views
            Label viewsLabel = new Label();
            viewsLabel.Text = $"👁️ {views}";
            viewsLabel.Font = new Font("Segoe UI", 9);
            viewsLabel.ForeColor = UIHelper.TextSecondary;
            viewsLabel.Size = new Size(60, 20);
            viewsLabel.Location = new Point(0, 5);
            viewsLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Bids
            Label bidsLabel = new Label();
            bidsLabel.Text = $"🔨 {bidCount}";
            bidsLabel.Font = new Font("Segoe UI", 9);
            bidsLabel.ForeColor = UIHelper.TextSecondary;
            bidsLabel.Size = new Size(60, 20);
            bidsLabel.Location = new Point(70, 5);
            bidsLabel.TextAlign = ContentAlignment.MiddleLeft;

            statsPanel.Controls.Add(viewsLabel);
            statsPanel.Controls.Add(bidsLabel);

            // Action buttons
            Button btnUpdatePrice = new Button();
            btnUpdatePrice.Text = "UPDATE PRICE";
            btnUpdatePrice.Size = new Size(90, 30);
            btnUpdatePrice.Location = new Point(10, 175);
            UIHelper.StyleButton(btnUpdatePrice, false);
            btnUpdatePrice.Tag = nftId;
            btnUpdatePrice.Click += BtnUpdatePrice_Click;

            Button btnDelist = new Button();
            btnDelist.Text = "DELIST";
            btnDelist.Size = new Size(90, 30);
            btnDelist.Location = new Point(110, 175);
            UIHelper.StyleButton(btnDelist, false);
            btnDelist.Tag = nftId;
            btnDelist.Click += BtnDelist_Click;

            // Add controls
            card.Controls.Add(listedBadge);
            card.Controls.Add(titleLabel);
            card.Controls.Add(priceLabel);
            card.Controls.Add(bidLabel);
            card.Controls.Add(ownerLabel);
            card.Controls.Add(statsPanel);
            card.Controls.Add(btnUpdatePrice);
            card.Controls.Add(btnDelist);

            // Hover effect
            UIHelper.ApplyHoverEffect(card, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            return card;
        }

        #endregion

        #region Helper Methods

        private void ShowNoNFTsMessage(Control container, string title, string message)
        {
            Panel messagePanel = new Panel();
            messagePanel.Size = new Size(400, 200);
            messagePanel.Location = new Point(250, 150);
            messagePanel.BackColor = Color.Transparent;

            Label iconLabel = new Label();
            iconLabel.Text = "🎴";
            iconLabel.Font = new Font("Segoe UI", 48);
            iconLabel.ForeColor = UIHelper.BlueAccent;
            iconLabel.Size = new Size(100, 100);
            iconLabel.Location = new Point(150, 20);
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;

            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            titleLabel.ForeColor = UIHelper.TextPrimary;
            titleLabel.Size = new Size(400, 40);
            titleLabel.Location = new Point(0, 130);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;

            Label messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.Font = new Font("Segoe UI", 12);
            messageLabel.ForeColor = UIHelper.TextSecondary;
            messageLabel.Size = new Size(400, 30);
            messageLabel.Location = new Point(0, 170);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            messagePanel.Controls.Add(iconLabel);
            messagePanel.Controls.Add(titleLabel);
            messagePanel.Controls.Add(messageLabel);
            container.Controls.Add(messagePanel);
        }

        private void LoadAllData()
        {
            // Load data for current tab
            switch (tabControl.SelectedIndex)
            {
                case 0: // Owned NFTs
                    LoadOwnedNFTs();
                    break;
                case 1: // Sold NFTs
                    LoadSoldNFTs();
                    break;
                case 2: // Bid History
                    LoadBidHistory();
                    break;
                case 3: // Listed NFTs
                    LoadListedNFTs();
                    break;
            }
        }

        #endregion

        #region Event Handlers

        private void BtnRefreshOwned_Click(object sender, EventArgs e)
        {
            LoadOwnedNFTs();
            UIHelper.ShowMessage("Owned NFTs refreshed!", "Refresh Complete");
        }

        private void BtnRefreshSold_Click(object sender, EventArgs e)
        {
            LoadSoldNFTs();
            UIHelper.ShowMessage("Sold NFTs refreshed!", "Refresh Complete");
        }

        private void BtnRefreshBids_Click(object sender, EventArgs e)
        {
            LoadBidHistory();
            UIHelper.ShowMessage("Bid history refreshed!", "Refresh Complete");
        }

        private void BtnSellNFT_Click(object sender, EventArgs e)
        {
            // Navigate to Sell NFT tab or open sell form
            tabControl.SelectedIndex = 3; // Listed NFTs tab
            LoadListedNFTs();
        }

        private void BtnSellSpecific_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int nftId = (int)button.Tag;

                // Find NFT details
                DataRow nftRow = null;
                foreach (DataRow row in ownedNFTs.Rows)
                {
                    if (Convert.ToInt32(row["NFTID"]) == nftId)
                    {
                        nftRow = row;
                        break;
                    }
                }

                if (nftRow != null)
                {
                    string title = nftRow["Title"].ToString();
                    decimal currentPrice = Convert.ToDecimal(nftRow["Price"]);

                    using (UpdatePriceForm updateForm = new UpdatePriceForm(nftId, title, currentPrice, true))
                    {
                        if (updateForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LoadOwnedNFTs();
                        }
                    }
                }
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int nftId = (int)button.Tag;

                // Find NFT details
                DataRow nftRow = null;
                foreach (DataRow row in ownedNFTs.Rows)
                {
                    if (Convert.ToInt32(row["NFTID"]) == nftId)
                    {
                        nftRow = row;
                        break;
                    }
                }

                if (nftRow != null)
                {
                    string title = nftRow["Title"].ToString();
                    string description = nftRow["Description"].ToString();
                    decimal price = Convert.ToDecimal(nftRow["Price"]);
                    decimal? currentBid = nftRow["CurrentBid"] != DBNull.Value ? Convert.ToDecimal(nftRow["CurrentBid"]) : (decimal?)null;
                    string creator = nftRow["CreatedBy"].ToString();
                    string category = nftRow["Category"].ToString();
                    int views = Convert.ToInt32(nftRow["Views"]);
                    int bidCount = Convert.ToInt32(nftRow["BidCount"]);

                    string details = $"NFT DETAILS\n\n" +
                                   $"Title: {title}\n" +
                                   $"Description: {description}\n" +
                                   $"Creator: {creator}\n" +
                                   $"Category: {category}\n" +
                                   $"List Price: {price:C2}\n" +
                                   $"Current Bid: {(currentBid.HasValue ? currentBid.Value.ToString("C2") : "None")}\n" +
                                   $"Views: {views}\n" +
                                   $"Bids: {bidCount}\n" +
                                   $"Status: Owned by you";

                    MessageBox.Show(details, "NFT Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnListNewNFT_Click(object sender, EventArgs e)
        {
            // This will be implemented in the SellNFTControl
            MessageBox.Show("NFT listing functionality will be implemented in the Sell NFT section.",
                "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdatePrice_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int nftId = (int)button.Tag;

                // Find NFT details
                DataRow nftRow = null;
                foreach (DataRow row in listedNFTs.Rows)
                {
                    if (Convert.ToInt32(row["NFTID"]) == nftId)
                    {
                        nftRow = row;
                        break;
                    }
                }

                if (nftRow != null)
                {
                    string title = nftRow["Title"].ToString();
                    decimal currentPrice = Convert.ToDecimal(nftRow["Price"]);

                    using (UpdatePriceForm updateForm = new UpdatePriceForm(nftId, title, currentPrice, false))
                    {
                        if (updateForm.ShowDialog(this) == DialogResult.OK)
                        {
                            LoadListedNFTs();
                        }
                    }
                }
            }
        }

        private void BtnDelist_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                int nftId = (int)button.Tag;

                // Find NFT title
                string nftTitle = "";
                foreach (DataRow row in listedNFTs.Rows)
                {
                    if (Convert.ToInt32(row["NFTID"]) == nftId)
                    {
                        nftTitle = row["Title"].ToString();
                        break;
                    }
                }

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delist '{nftTitle}'?\n\n" +
                    "This will remove it from the marketplace but you will keep ownership.",
                    "Confirm Delist",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Update NFT to be owned by the creator (delist)
                    string query = @"
                        UPDATE NFTs 
                        SET OwnerID = (SELECT UserID FROM Users WHERE Username = CreatedBy)
                        WHERE NFTID = @NFTID";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@NFTID", nftId)
                    };

                    int rowsAffected = DBHelper.ExecuteNonQuery(query, parameters);

                    if (rowsAffected > 0)
                    {
                        UIHelper.ShowSuccess($"'{nftTitle}' has been delisted and is now back in your collection.");
                        LoadListedNFTs();
                    }
                    else
                    {
                        UIHelper.ShowError("Failed to delist NFT. Please try again.");
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public void RefreshNFTs()
        {
            LoadAllData();
            UIHelper.ShowMessage("My NFTs refreshed successfully!", "Refresh Complete");
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw decorative border around tab control
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 70), 2))
            {
                Rectangle rect = new Rectangle(18, 78, 914, 654);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }
    }
}