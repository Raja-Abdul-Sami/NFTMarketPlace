using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TriApex.Helpers;
using TriApex.Forms;
using System.Data.SqlClient;
using System.IO;

namespace TriApex.UserControls
{
    public partial class BrowseNFTsControl : UserControl
    {
        // UI Components
        private Panel searchPanel;
        private Panel filtersPanel;
        private FlowLayoutPanel nftsFlowPanel;
        private Button btnRefresh;
        private Button btnClearFilters;
        private TextBox txtSearch;
        private ComboBox cmbSortBy;
        private ComboBox cmbCategory;
        private CheckBox chkShowBidsOnly;
        private NumericUpDown numMinPrice;
        private NumericUpDown numMaxPrice;
        private Label lblResultsCount;

        // Data
        private DataTable allNFTs;
        private List<NFTCard> nftCards = new List<NFTCard>();

        // Constants
        private const int CARD_WIDTH = 220;
        private const int CARD_HEIGHT = 320;

        public BrowseNFTsControl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadNFTs();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // BrowseNFTsControl
            this.BackColor = Color.Transparent;
            this.Size = new Size(950, 730);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "BROWSE NFTs";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(300, 50);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Refresh button
            btnRefresh = new Button();
            btnRefresh.Text = "🔄 Refresh";
            btnRefresh.Size = new Size(120, 35);
            btnRefresh.Location = new Point(810, 25);
            UIHelper.StyleButton(btnRefresh, false);
            btnRefresh.Click += BtnRefresh_Click;

            // Search panel
            searchPanel = new Panel();
            searchPanel.Size = new Size(910, 60);
            searchPanel.Location = new Point(20, 80);
            searchPanel.BackColor = Color.FromArgb(35, 35, 45);
            searchPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, searchPanel.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Search label
            Label lblSearch = new Label();
            lblSearch.Text = "🔍";
            lblSearch.Font = new Font("Segoe UI", 14);
            lblSearch.ForeColor = UIHelper.BlueAccent;
            lblSearch.Size = new Size(40, 40);
            lblSearch.Location = new Point(20, 10);
            lblSearch.TextAlign = ContentAlignment.MiddleCenter;

            // Search textbox
            txtSearch = new TextBox();
            txtSearch.Size = new Size(300, 35);
            txtSearch.Location = new Point(60, 12);
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.BackColor = Color.FromArgb(50, 50, 60);
            txtSearch.ForeColor = Color.White;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Text = "Search NFTs by title, creator, or category...";
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.KeyDown += TxtSearch_KeyDown;

            // Results count
            lblResultsCount = new Label();
            lblResultsCount.Text = "0 NFTs found";
            lblResultsCount.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblResultsCount.ForeColor = UIHelper.TextSecondary;
            lblResultsCount.Size = new Size(150, 30);
            lblResultsCount.Location = new Point(750, 15);
            lblResultsCount.TextAlign = ContentAlignment.MiddleRight;

            // Filters panel
            filtersPanel = new Panel();
            filtersPanel.Size = new Size(910, 100);
            filtersPanel.Location = new Point(20, 150);
            filtersPanel.BackColor = Color.FromArgb(35, 35, 45);
            filtersPanel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, filtersPanel.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Filters title
            Label lblFilters = new Label();
            lblFilters.Text = "FILTERS";
            lblFilters.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblFilters.ForeColor = UIHelper.GoldAccent;
            lblFilters.Size = new Size(100, 30);
            lblFilters.Location = new Point(20, 10);
            lblFilters.TextAlign = ContentAlignment.MiddleLeft;

            // Category filter
            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Font = new Font("Segoe UI", 10);
            lblCategory.ForeColor = UIHelper.TextSecondary;
            lblCategory.Size = new Size(80, 25);
            lblCategory.Location = new Point(20, 45);
            lblCategory.TextAlign = ContentAlignment.MiddleLeft;

            cmbCategory = new ComboBox();
            cmbCategory.Size = new Size(150, 30);
            cmbCategory.Location = new Point(100, 45);
            cmbCategory.Font = new Font("Segoe UI", 10);
            cmbCategory.BackColor = Color.FromArgb(50, 50, 60);
            cmbCategory.ForeColor = Color.White;
            cmbCategory.FlatStyle = FlatStyle.Flat;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.SelectedIndexChanged += Filter_Changed;

            // Price range
            Label lblPriceRange = new Label();
            lblPriceRange.Text = "Price Range:";
            lblPriceRange.Font = new Font("Segoe UI", 10);
            lblPriceRange.ForeColor = UIHelper.TextSecondary;
            lblPriceRange.Size = new Size(90, 25);
            lblPriceRange.Location = new Point(270, 45);
            lblPriceRange.TextAlign = ContentAlignment.MiddleLeft;

            numMinPrice = new NumericUpDown();
            numMinPrice.Size = new Size(80, 30);
            numMinPrice.Location = new Point(365, 45);
            numMinPrice.Font = new Font("Segoe UI", 10);
            numMinPrice.BackColor = Color.FromArgb(50, 50, 60);
            numMinPrice.ForeColor = Color.White;
            numMinPrice.BorderStyle = BorderStyle.FixedSingle;
            numMinPrice.Minimum = 0;
            numMinPrice.Maximum = 1000000;
            numMinPrice.DecimalPlaces = 2;
            numMinPrice.Value = 0;
            numMinPrice.ValueChanged += Filter_Changed;

            Label lblTo = new Label();
            lblTo.Text = "to";
            lblTo.Font = new Font("Segoe UI", 10);
            lblTo.ForeColor = UIHelper.TextSecondary;
            lblTo.Size = new Size(20, 25);
            lblTo.Location = new Point(450, 45);
            lblTo.TextAlign = ContentAlignment.MiddleCenter;

            numMaxPrice = new NumericUpDown();
            numMaxPrice.Size = new Size(80, 30);
            numMaxPrice.Location = new Point(475, 45);
            numMaxPrice.Font = new Font("Segoe UI", 10);
            numMaxPrice.BackColor = Color.FromArgb(50, 50, 60);
            numMaxPrice.ForeColor = Color.White;
            numMaxPrice.BorderStyle = BorderStyle.FixedSingle;
            numMaxPrice.Minimum = 0;
            numMaxPrice.Maximum = 1000000;
            numMaxPrice.DecimalPlaces = 2;
            numMaxPrice.Value = 10000;
            numMaxPrice.ValueChanged += Filter_Changed;

            // Show bids only
            chkShowBidsOnly = new CheckBox();
            chkShowBidsOnly.Text = "Show only NFTs with bids";
            chkShowBidsOnly.Font = new Font("Segoe UI", 10);
            chkShowBidsOnly.ForeColor = UIHelper.TextSecondary;
            chkShowBidsOnly.Size = new Size(200, 25);
            chkShowBidsOnly.Location = new Point(580, 45);
            chkShowBidsOnly.BackColor = Color.Transparent;
            chkShowBidsOnly.CheckedChanged += Filter_Changed;

            // Sort by
            Label lblSortBy = new Label();
            lblSortBy.Text = "Sort by:";
            lblSortBy.Font = new Font("Segoe UI", 10);
            lblSortBy.ForeColor = UIHelper.TextSecondary;
            lblSortBy.Size = new Size(60, 25);
            lblSortBy.Location = new Point(20, 75);
            lblSortBy.TextAlign = ContentAlignment.MiddleLeft;

            cmbSortBy = new ComboBox();
            cmbSortBy.Size = new Size(200, 30);
            cmbSortBy.Location = new Point(85, 75);
            cmbSortBy.Font = new Font("Segoe UI", 10);
            cmbSortBy.BackColor = Color.FromArgb(50, 50, 60);
            cmbSortBy.ForeColor = Color.White;
            cmbSortBy.FlatStyle = FlatStyle.Flat;
            cmbSortBy.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSortBy.Items.AddRange(new string[] {
                "Newest First",
                "Oldest First",
                "Price: Low to High",
                "Price: High to Low",
                "Most Bids",
                "Most Views"
            });
            cmbSortBy.SelectedIndex = 0;
            cmbSortBy.SelectedIndexChanged += Filter_Changed;

            // Clear filters button
            btnClearFilters = new Button();
            btnClearFilters.Text = "Clear Filters";
            btnClearFilters.Size = new Size(120, 30);
            btnClearFilters.Location = new Point(770, 65);
            UIHelper.StyleButton(btnClearFilters, false);
            btnClearFilters.Click += BtnClearFilters_Click;

            // NFTs flow panel
            nftsFlowPanel = new FlowLayoutPanel();
            nftsFlowPanel.Size = new Size(910, 450);
            nftsFlowPanel.Location = new Point(20, 260);
            nftsFlowPanel.BackColor = Color.Transparent;
            nftsFlowPanel.AutoScroll = true;
            nftsFlowPanel.WrapContents = true;
            nftsFlowPanel.Padding = new Padding(10);

            // Add controls to panels
            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(lblResultsCount);

            filtersPanel.Controls.Add(lblFilters);
            filtersPanel.Controls.Add(lblCategory);
            filtersPanel.Controls.Add(cmbCategory);
            filtersPanel.Controls.Add(lblPriceRange);
            filtersPanel.Controls.Add(numMinPrice);
            filtersPanel.Controls.Add(lblTo);
            filtersPanel.Controls.Add(numMaxPrice);
            filtersPanel.Controls.Add(chkShowBidsOnly);
            filtersPanel.Controls.Add(lblSortBy);
            filtersPanel.Controls.Add(cmbSortBy);
            filtersPanel.Controls.Add(btnClearFilters);

            // Add controls to main control
            this.Controls.Add(lblTitle);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(searchPanel);
            this.Controls.Add(filtersPanel);
            this.Controls.Add(nftsFlowPanel);
        }

        private void LoadNFTs()
        {
            try
            {
                // Show loading
                nftsFlowPanel.Controls.Clear();
                nftCards.Clear();

                Label loadingLabel = new Label();
                loadingLabel.Text = "Loading NFTs...";
                loadingLabel.Font = new Font("Segoe UI", 14);
                loadingLabel.ForeColor = UIHelper.GoldAccent;
                loadingLabel.Size = new Size(200, 40);
                loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
                nftsFlowPanel.Controls.Add(loadingLabel);

                // Load NFTs from database using stored procedure
                allNFTs = DBHelper.ExecuteStoredProcedure("sp_GetAvailableNFTs", null);

                // Load categories
                LoadCategories();

                // Display NFTs
                DisplayNFTs();

                // Update results count
                UpdateResultsCount();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Failed to load NFTs: {ex.Message}");
            }
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");

            if (allNFTs != null && allNFTs.Rows.Count > 0)
            {
                var categories = allNFTs.AsEnumerable()
                    .Select(row => row["Category"].ToString())
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();

                cmbCategory.Items.AddRange(categories.ToArray());
                cmbCategory.SelectedIndex = 0;
            }
        }

        private void DisplayNFTs()
        {
            nftsFlowPanel.Controls.Clear();
            nftCards.Clear();

            if (allNFTs == null || allNFTs.Rows.Count == 0)
            {
                ShowNoResultsMessage();
                return;
            }

            // Apply filters and sorting
            var filteredNFTs = ApplyFilters(allNFTs);
            var sortedNFTs = ApplySorting(filteredNFTs);

            if (sortedNFTs.Count == 0)
            {
                ShowNoResultsMessage();
                return;
            }

            // Create NFT cards
            foreach (DataRow row in sortedNFTs)
            {
                NFTCard card = CreateNFTCard(row);
                nftCards.Add(card);
                nftsFlowPanel.Controls.Add(card.GetCard());
            }
        }

        private List<DataRow> ApplyFilters(DataTable nfts)
        {
            var filtered = nfts.AsEnumerable().ToList();

            // Search filter
            string searchTerm = txtSearch.Text.Trim();
            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm != "Search NFTs by title, creator, or category...")
            {
                searchTerm = searchTerm.ToLower();
                filtered = filtered.Where(row =>
                    row["Title"].ToString().ToLower().Contains(searchTerm) ||
                    row["CreatedBy"].ToString().ToLower().Contains(searchTerm) ||
                    row["Category"].ToString().ToLower().Contains(searchTerm) ||
                    row["Description"].ToString().ToLower().Contains(searchTerm)
                ).ToList();
            }


            // Category filter
            if (cmbCategory.SelectedIndex > 0)
            {
                string selectedCategory = cmbCategory.SelectedItem.ToString();
                filtered = filtered.Where(row => row["Category"].ToString() == selectedCategory).ToList();
            }

            // Price range filter
            decimal minPrice = numMinPrice.Value;
            decimal maxPrice = numMaxPrice.Value;
            if (minPrice > 0 || maxPrice < 10000)
            {
                filtered = filtered.Where(row =>
                {
                    decimal price = Convert.ToDecimal(row["Price"]);
                    return price >= minPrice && price <= maxPrice;
                }).ToList();
            }

            // Bids only filter
            if (chkShowBidsOnly.Checked)
            {
                filtered = filtered.Where(row => row["CurrentBid"] != DBNull.Value).ToList();
            }

            return filtered;
        }

        private List<DataRow> ApplySorting(List<DataRow> nfts)
        {
            switch (cmbSortBy.SelectedIndex)
            {
                case 0: // Newest First
                    return nfts.OrderByDescending(row => Convert.ToDateTime(row["CreatedDate"])).ToList();
                case 1: // Oldest First
                    return nfts.OrderBy(row => Convert.ToDateTime(row["CreatedDate"])).ToList();
                case 2: // Price: Low to High
                    return nfts.OrderBy(row => Convert.ToDecimal(row["Price"])).ToList();
                case 3: // Price: High to Low
                    return nfts.OrderByDescending(row => Convert.ToDecimal(row["Price"])).ToList();
                case 4: // Most Bids
                    return nfts.OrderByDescending(row => Convert.ToInt32(row["BidCount"])).ToList();
                case 5: // Most Views
                    return nfts.OrderByDescending(row => Convert.ToInt32(row["Views"])).ToList();
                default:
                    return nfts;
            }
        }

        private NFTCard CreateNFTCard(DataRow row)
        {
            int nftId = Convert.ToInt32(row["NFTID"]);
            string title = row["Title"].ToString();
            string description = row["Description"].ToString();
            decimal price = Convert.ToDecimal(row["Price"]);
            decimal? currentBid = row["CurrentBid"] != DBNull.Value ? Convert.ToDecimal(row["CurrentBid"]) : (decimal?)null;
            string imagePath = row["ImagePath"].ToString();
            string creator = row["CreatedBy"].ToString();
            string category = row["Category"].ToString();
            int views = Convert.ToInt32(row["Views"]);
            int bidCount = Convert.ToInt32(row["BidCount"]);
            int ownerId = Convert.ToInt32(row["OwnerID"]);

            return new NFTCard(nftId, title, description, price, currentBid, imagePath,
                creator, category, views, bidCount, ownerId, OnBuyNowClicked, OnPlaceBidClicked);
        }

        private void ShowNoResultsMessage()
        {
            Panel messagePanel = new Panel();
            messagePanel.Size = new Size(400, 200);
            messagePanel.Location = new Point(275, 350);
            messagePanel.BackColor = Color.Transparent;

            Label iconLabel = new Label();
            iconLabel.Text = "🔍";
            iconLabel.Font = new Font("Segoe UI", 48);
            iconLabel.ForeColor = UIHelper.BlueAccent;
            iconLabel.Size = new Size(100, 100);
            iconLabel.Location = new Point(150, 20);
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;

            Label messageLabel = new Label();
            messageLabel.Text = "No NFTs found matching your criteria";
            messageLabel.Font = new Font("Segoe UI", 14);
            messageLabel.ForeColor = UIHelper.TextSecondary;
            messageLabel.Size = new Size(400, 40);
            messageLabel.Location = new Point(0, 130);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            messagePanel.Controls.Add(iconLabel);
            messagePanel.Controls.Add(messageLabel);
            nftsFlowPanel.Controls.Add(messagePanel);
        }

        private void UpdateResultsCount()
        {
            int totalCount = allNFTs?.Rows.Count ?? 0;
            int filteredCount = nftCards.Count;

            if (totalCount == filteredCount || filteredCount == 0)
            {
                lblResultsCount.Text = $"{filteredCount} NFTs found";
            }
            else
            {
                lblResultsCount.Text = $"{filteredCount} of {totalCount} NFTs";
            }
        }

        #region Event Handlers

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshNFTs();
        }

        private void BtnClearFilters_Click(object sender, EventArgs e)
        {
            ClearFilters();
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Delay the filter to avoid too many updates
            Timer searchTimer = new Timer();
            searchTimer.Interval = 500;
            searchTimer.Tick += (s, e2) =>
            {
                searchTimer.Stop();
                DisplayNFTs();
                UpdateResultsCount();
            };
            searchTimer.Start();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DisplayNFTs();
                UpdateResultsCount();
            }
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            DisplayNFTs();
            UpdateResultsCount();
        }

        #endregion

        #region Business Logic Methods

        public void RefreshNFTs()
        {
            btnRefresh.Enabled = false;
            btnRefresh.Text = "Loading...";

            LoadNFTs();

            btnRefresh.Enabled = true;
            btnRefresh.Text = "🔄 Refresh";

            UIHelper.ShowMessage("NFTs refreshed successfully!", "Refresh Complete");
        }

        private void ClearFilters()
        {
            txtSearch.Text = "";
            cmbCategory.SelectedIndex = 0;
            numMinPrice.Value = 0;
            numMaxPrice.Value = 10000;
            chkShowBidsOnly.Checked = false;
            cmbSortBy.SelectedIndex = 0;

            DisplayNFTs();
            UpdateResultsCount();
        }

        private void OnBuyNowClicked(int nftId, decimal price, string title)
        {
            // Confirm purchase
            DialogResult result = MessageBox.Show($"Are you sure you want to buy '{title}' for {price:C2}?\n\n" +
                                                 $"Your current balance: {SessionManager.Instance.GetFormattedBalance()}",
                                                 "Confirm Purchase", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Check balance
            if (!SessionManager.Instance.HasSufficientBalance(price))
            {
                UIHelper.ShowError($"Insufficient balance. You need {price:C2}, but you have {SessionManager.Instance.GetFormattedBalance()}.");
                return;
            }

            // Process purchase using stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                new SqlParameter("@NFTID", nftId),
                new SqlParameter("@Result", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
            };

            int purchaseResult = DBHelper.ExecuteStoredProcedureWithOutput("sp_BuyNFT", parameters, "@Result");

            switch (purchaseResult)
            {
                case 1: // Success
                    SessionManager.Instance.RefreshBalance();
                    UIHelper.ShowSuccess($"Successfully purchased '{title}' for {price:C2}!\nIt has been added to your collection.");
                    RefreshNFTs();
                    break;
                case -1: // NFT not found or sold
                    UIHelper.ShowError("This NFT is no longer available for purchase.");
                    RefreshNFTs();
                    break;
                case -2: // Insufficient balance
                    UIHelper.ShowError("Insufficient balance. Please add funds to your wallet.");
                    break;
                default: // Error
                    UIHelper.ShowError("Failed to complete purchase. Please try again.");
                    break;
            }
        }

        private void OnPlaceBidClicked(int nftId, decimal currentPrice, decimal? currentBid, string title)
        {
            using (PlaceBidForm bidForm = new PlaceBidForm(nftId, title, currentPrice, currentBid))
            {
                if (bidForm.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshNFTs();
                }
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw decorative elements
            using (Pen pen = new Pen(Color.FromArgb(40, 40, 50), 1))
            {
                // Horizontal separator
                e.Graphics.DrawLine(pen, 20, 250, 930, 250);
            }
        }
    }

    #region NFTCard Class

    public class NFTCard
    {
        private int nftId;
        private string title;
        private string description;
        private decimal price;
        private decimal? currentBid;
        private string imagePath;
        private string creator;
        private string category;
        private int views;
        private int bidCount;
        private int ownerId;

        private Panel cardPanel;
        private Action<int, decimal, string> buyNowCallback;
        private Action<int, decimal, decimal?, string> placeBidCallback;

        public NFTCard(int nftId, string title, string description, decimal price, decimal? currentBid,
                      string imagePath, string creator, string category, int views, int bidCount,
                      int ownerId, Action<int, decimal, string> buyNowCallback,
                      Action<int, decimal, decimal?, string> placeBidCallback)
        {
            this.nftId = nftId;
            this.title = title;
            this.description = description;
            this.price = price;
            this.currentBid = currentBid;
            this.imagePath = imagePath;
            this.creator = creator;
            this.category = category;
            this.views = views;
            this.bidCount = bidCount;
            this.ownerId = ownerId;
            this.buyNowCallback = buyNowCallback;
            this.placeBidCallback = placeBidCallback;

            CreateCard();
        }

        private void CreateCard()
        {
            cardPanel = new Panel();
            cardPanel.Size = new Size(220, 320);
            cardPanel.BackColor = Color.FromArgb(40, 40, 50);
            cardPanel.Padding = new Padding(10);
            cardPanel.Margin = new Padding(10);
            cardPanel.Cursor = Cursors.Hand;

            // Rounded corners
            cardPanel.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0,
                cardPanel.Width, cardPanel.Height, 12, 12));

            // Title (truncate if too long)
            string displayTitle = title.Length > 20 ? title.Substring(0, 17) + "..." : title;

            Label titleLabel = new Label();
            titleLabel.Text = displayTitle;
            titleLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            titleLabel.ForeColor = UIHelper.TextPrimary;
            titleLabel.Size = new Size(200, 25);
            titleLabel.Location = new Point(10, 10);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            titleLabel.Cursor = Cursors.Hand;

            // Category badge
            Label categoryLabel = new Label();
            categoryLabel.Text = category;
            categoryLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            categoryLabel.ForeColor = UIHelper.BlueAccent;
            categoryLabel.BackColor = Color.FromArgb(30, 30, 45);
            categoryLabel.Size = new Size(80, 20);
            categoryLabel.Location = new Point(10, 40);
            categoryLabel.TextAlign = ContentAlignment.MiddleCenter;
            categoryLabel.Cursor = Cursors.Hand;
            categoryLabel.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, categoryLabel.ClientRectangle,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid,
                    UIHelper.BlueAccent, 1, ButtonBorderStyle.Solid);
            };

            // Creator
            Label creatorLabel = new Label();
            creatorLabel.Text = $"By: {creator}";
            creatorLabel.Font = new Font("Segoe UI", 8);
            creatorLabel.ForeColor = UIHelper.TextSecondary;
            creatorLabel.Size = new Size(200, 20);
            creatorLabel.Location = new Point(10, 65);
            creatorLabel.TextAlign = ContentAlignment.MiddleLeft;
            creatorLabel.Cursor = Cursors.Hand;

            // Image placeholder
            Panel imagePanel = new Panel();
            imagePanel.Size = new Size(180, 120);
            imagePanel.Location = new Point(10, 90);
            imagePanel.BackColor = Color.FromArgb(50, 50, 65);
            imagePanel.Cursor = Cursors.Hand;

            // Draw NFT image or placeholder
            imagePanel.Paint += (s, e) => DrawNFTCardImage(e.Graphics, imagePanel.ClientRectangle);

            // Price
            Label priceLabel = new Label();
            priceLabel.Text = $"Price: {price:C2}";
            priceLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            priceLabel.ForeColor = UIHelper.GoldAccent;
            priceLabel.Size = new Size(200, 25);
            priceLabel.Location = new Point(10, 220);
            priceLabel.TextAlign = ContentAlignment.MiddleLeft;
            priceLabel.Cursor = Cursors.Hand;

            // Current bid
            Label bidLabel = new Label();
            if (currentBid.HasValue)
            {
                bidLabel.Text = $"Current Bid: {currentBid.Value:C2}";
                bidLabel.ForeColor = UIHelper.BlueAccent;
            }
            else
            {
                bidLabel.Text = "No bids yet";
                bidLabel.ForeColor = UIHelper.TextSecondary;
            }
            bidLabel.Font = new Font("Segoe UI", 9);
            bidLabel.Size = new Size(200, 20);
            bidLabel.Location = new Point(10, 245);
            bidLabel.TextAlign = ContentAlignment.MiddleLeft;
            bidLabel.Cursor = Cursors.Hand;

            // Stats
            Panel statsPanel = new Panel();
            statsPanel.Size = new Size(200, 25);
            statsPanel.Location = new Point(10, 265);
            statsPanel.BackColor = Color.Transparent;
            statsPanel.Cursor = Cursors.Hand;

            // Views
            Label viewsLabel = new Label();
            viewsLabel.Text = $"👁️ {views}";
            viewsLabel.Font = new Font("Segoe UI", 9);
            viewsLabel.ForeColor = UIHelper.TextSecondary;
            viewsLabel.Size = new Size(60, 20);
            viewsLabel.Location = new Point(0, 2);
            viewsLabel.TextAlign = ContentAlignment.MiddleLeft;
            viewsLabel.Cursor = Cursors.Hand;

            // Bids
            Label bidsLabel = new Label();
            bidsLabel.Text = $"🔨 {bidCount}";
            bidsLabel.Font = new Font("Segoe UI", 9);
            bidsLabel.ForeColor = UIHelper.TextSecondary;
            bidsLabel.Size = new Size(60, 20);
            bidsLabel.Location = new Point(70, 2);
            bidsLabel.TextAlign = ContentAlignment.MiddleLeft;
            bidsLabel.Cursor = Cursors.Hand;

            statsPanel.Controls.Add(viewsLabel);
            statsPanel.Controls.Add(bidsLabel);

            // Check if user is the owner
            bool isOwner = ownerId == SessionManager.Instance.CurrentUserID;

            // Buy Now button (disabled if owner)
            Button buyButton = new Button();
            buyButton.Text = "BUY NOW";
            buyButton.Size = new Size(90, 30);
            buyButton.Location = new Point(10, 280);
            buyButton.Enabled = !isOwner;
            UIHelper.StyleButton(buyButton, true);
            buyButton.Click += (s, e) => buyNowCallback?.Invoke(nftId, price, title);
            buyButton.Cursor = Cursors.Hand;

            // Place Bid button (disabled if owner)
            Button bidButton = new Button();
            bidButton.Text = "PLACE BID";
            bidButton.Size = new Size(90, 30);
            bidButton.Location = new Point(110, 280);
            bidButton.Enabled = !isOwner;
            UIHelper.StyleButton(bidButton, false);
            bidButton.Click += (s, e) => placeBidCallback?.Invoke(nftId, price, currentBid, title);
            bidButton.Cursor = Cursors.Hand;

            // If owner, show "Your NFT" label
            if (isOwner)
            {
                Label ownerLabel = new Label();
                ownerLabel.Text = "YOUR NFT";
                ownerLabel.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                ownerLabel.ForeColor = UIHelper.SuccessColor;
                ownerLabel.Size = new Size(200, 20);
                ownerLabel.Location = new Point(10, 285);
                ownerLabel.TextAlign = ContentAlignment.MiddleCenter;
                ownerLabel.Cursor = Cursors.Hand;

                cardPanel.Controls.Add(ownerLabel);
                buyButton.Visible = false;
                bidButton.Visible = false;
            }

            // Add controls to card
            cardPanel.Controls.Add(titleLabel);
            cardPanel.Controls.Add(categoryLabel);
            cardPanel.Controls.Add(creatorLabel);
            cardPanel.Controls.Add(imagePanel);
            cardPanel.Controls.Add(priceLabel);
            cardPanel.Controls.Add(bidLabel);
            cardPanel.Controls.Add(statsPanel);
            cardPanel.Controls.Add(buyButton);
            cardPanel.Controls.Add(bidButton);

            // Hover effect
            UIHelper.ApplyHoverEffect(cardPanel, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            // Scale effect on hover
            cardPanel.MouseEnter += (s, e) =>
            {
                cardPanel.Size = new Size(222, 322);
                cardPanel.Location = new Point(cardPanel.Location.X - 1, cardPanel.Location.Y - 1);
            };

            cardPanel.MouseLeave += (s, e) =>
            {
                cardPanel.Size = new Size(220, 320);
                cardPanel.Location = new Point(cardPanel.Location.X + 1, cardPanel.Location.Y + 1);
            };

            // Click event for card (view details)
            cardPanel.Click += (s, e) => ShowNFTDetails();

            // Propagate click to all child controls
            foreach (Control control in cardPanel.Controls)
            {
                control.Click += (s, e) => ShowNFTDetails();
            }
        }

        private void DrawNFTCardImage(Graphics g, Rectangle bounds)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(60, 60, 75)), bounds);

            if (File.Exists(imagePath))
            {
                using (Image img = Image.FromFile(imagePath))
                {
                    g.DrawImage(img, bounds);
                }
            }
            else
            {
                // Draw placeholder pattern
                using (Pen goldPen = new Pen(Color.FromArgb(80, UIHelper.GoldAccent), 1))
                using (Pen bluePen = new Pen(Color.FromArgb(80, UIHelper.BlueAccent), 1))
                {
                    for (int i = -bounds.Height; i < bounds.Width; i += 20)
                        g.DrawLine(goldPen, i, 0, i + bounds.Height, bounds.Height);

                    Random rand = new Random(nftId);
                    for (int i = 0; i < 5; i++)
                    {
                        int x = rand.Next(bounds.Width - 30);
                        int y = rand.Next(bounds.Height - 30);
                        int size = rand.Next(20, 40);
                        if (rand.Next(2) == 0)
                            g.DrawEllipse(goldPen, x, y, size, size);
                        else
                            g.DrawEllipse(bluePen, x, y, size, size);
                    }
                }

                using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
                using (Brush brush = new SolidBrush(UIHelper.TextPrimary))
                {
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    string displayText = title.Length > 15 ? title.Substring(0, 12) + "..." : title;
                    g.DrawString(displayText, font, brush, bounds, format);
                }
            }
        }


        private void ShowNFTDetails()
        {
            string details = $"NFT Details:\n\n" +
                           $"Title: {title}\n" +
                           $"Description: {description}\n" +
                           $"Creator: {creator}\n" +
                           $"Category: {category}\n" +
                           $"Price: {price:C2}\n" +
                           $"Current Bid: {(currentBid.HasValue ? currentBid.Value.ToString("C2") : "None")}\n" +
                           $"Views: {views}\n" +
                           $"Bids: {bidCount}\n" +
                           $"Owner: {(ownerId == SessionManager.Instance.CurrentUserID ? "You" : creator)}";

            MessageBox.Show(details, $"NFT: {title}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public Panel GetCard()
        {
            return cardPanel;
        }
    }

    #endregion
}