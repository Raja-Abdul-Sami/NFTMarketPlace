using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class PlaceBidForm : Form
    {
        private int nftId;
        private string nftTitle;
        private decimal currentPrice;
        private decimal? currentBid;

        private Panel mainPanel;
        private Label lblTitle;
        private Label lblNFTInfo;
        private Label lblCurrentPrice;
        private Label lblCurrentBid;
        private Label lblMinBid;
        private TextBox txtBidAmount;
        private Button btnPlaceBid;
        private Button btnCancel;
        private Label lblError;

        public PlaceBidForm(int nftId, string nftTitle, decimal currentPrice, decimal? currentBid)
        {
            this.nftId = nftId;
            this.nftTitle = nftTitle;
            this.currentPrice = currentPrice;
            this.currentBid = currentBid;

            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // PlaceBidForm
            this.ClientSize = new Size(500, 400);
            this.Text = "Place Bid - TriApex";
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
            lblTitle = new Label();
            lblTitle.Text = "PLACE A BID";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(50, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // NFT info
            string displayTitle = nftTitle.Length > 40 ? nftTitle.Substring(0, 37) + "..." : nftTitle;
            lblNFTInfo = new Label();
            lblNFTInfo.Text = $"On: {displayTitle}";
            lblNFTInfo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblNFTInfo.ForeColor = UIHelper.TextPrimary;
            lblNFTInfo.Size = new Size(400, 40);
            lblNFTInfo.Location = new Point(50, 80);
            lblNFTInfo.TextAlign = ContentAlignment.MiddleCenter;

            // Current price
            lblCurrentPrice = new Label();
            lblCurrentPrice.Text = $"List Price: {currentPrice:C2}";
            lblCurrentPrice.Font = new Font("Segoe UI", 12);
            lblCurrentPrice.ForeColor = UIHelper.TextSecondary;
            lblCurrentPrice.Size = new Size(400, 30);
            lblCurrentPrice.Location = new Point(50, 130);
            lblCurrentPrice.TextAlign = ContentAlignment.MiddleCenter;

            // Current bid (if exists)
            lblCurrentBid = new Label();
            if (currentBid.HasValue)
            {
                lblCurrentBid.Text = $"Current Highest Bid: {currentBid.Value:C2}";
                lblCurrentBid.ForeColor = UIHelper.BlueAccent;
            }
            else
            {
                lblCurrentBid.Text = "No bids placed yet";
                lblCurrentBid.ForeColor = UIHelper.TextSecondary;
            }
            lblCurrentBid.Font = new Font("Segoe UI", 12);
            lblCurrentBid.Size = new Size(400, 30);
            lblCurrentBid.Location = new Point(50, 160);
            lblCurrentBid.TextAlign = ContentAlignment.MiddleCenter;

            // Minimum bid required
            decimal minBid = currentBid.HasValue ? currentBid.Value + 0.1m : currentPrice + 0.1m;
            lblMinBid = new Label();
            lblMinBid.Text = $"Minimum Bid Required: {minBid:C2}";
            lblMinBid.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblMinBid.ForeColor = UIHelper.GoldAccent;
            lblMinBid.Size = new Size(400, 30);
            lblMinBid.Location = new Point(50, 190);
            lblMinBid.TextAlign = ContentAlignment.MiddleCenter;

            // Balance info
            Label lblBalance = new Label();
            lblBalance.Text = $"Your Balance: {SessionManager.Instance.GetFormattedBalance()}";
            lblBalance.Font = new Font("Segoe UI", 11);
            lblBalance.ForeColor = SessionManager.Instance.Balance >= minBid ?
                UIHelper.SuccessColor : UIHelper.ErrorColor;
            lblBalance.Size = new Size(400, 30);
            lblBalance.Location = new Point(50, 220);
            lblBalance.TextAlign = ContentAlignment.MiddleCenter;

            // Bid amount label
            Label lblBidAmount = new Label();
            lblBidAmount.Text = "YOUR BID AMOUNT ($)";
            lblBidAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblBidAmount.ForeColor = UIHelper.TextPrimary;
            lblBidAmount.Size = new Size(200, 25);
            lblBidAmount.Location = new Point(50, 260);
            lblBidAmount.TextAlign = ContentAlignment.MiddleLeft;

            // Bid amount textbox
            txtBidAmount = new TextBox();
            txtBidAmount.Size = new Size(400, 40);
            txtBidAmount.Location = new Point(50, 290);
            txtBidAmount.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtBidAmount.BackColor = Color.FromArgb(40, 40, 50);
            txtBidAmount.ForeColor = UIHelper.GoldAccent;
            txtBidAmount.BorderStyle = BorderStyle.FixedSingle;
            txtBidAmount.TextAlign = HorizontalAlignment.Center;
            txtBidAmount.Text = minBid.ToString("F2");
            txtBidAmount.KeyPress += TxtBidAmount_KeyPress;
            txtBidAmount.TextChanged += TxtBidAmount_TextChanged;

            // Quick bid buttons
            Panel quickBidPanel = new Panel();
            quickBidPanel.Size = new Size(400, 40);
            quickBidPanel.Location = new Point(50, 335);
            quickBidPanel.BackColor = Color.Transparent;

            string[] quickAmounts = { "+0.5", "+1.0", "+5.0", "+10.0", "MAX" };
            for (int i = 0; i < quickAmounts.Length; i++)
            {
                string amount = quickAmounts[i];               // capture
                Button quickBtn = new Button();
                quickBtn.Text = amount;
                quickBtn.Size = new Size(70, 30);
                quickBtn.Location = new Point(i * 80, 5);
                UIHelper.StyleButton(quickBtn, false);
                quickBtn.Font = new Font("Segoe UI", 9);
                quickBtn.Click += (s, e) => ApplyQuickBid(amount);
                quickBidPanel.Controls.Add(quickBtn);
            }

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(400, 30);
            lblError.Location = new Point(50, 380);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Place Bid button
            btnPlaceBid = new Button();
            btnPlaceBid.Text = "PLACE BID";
            btnPlaceBid.Size = new Size(180, 45);
            btnPlaceBid.Location = new Point(80, 420);
            UIHelper.StyleButton(btnPlaceBid, true);
            btnPlaceBid.Click += BtnPlaceBid_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(180, 45);
            btnCancel.Location = new Point(280, 420);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblNFTInfo);
            mainPanel.Controls.Add(lblCurrentPrice);
            mainPanel.Controls.Add(lblCurrentBid);
            mainPanel.Controls.Add(lblMinBid);
            mainPanel.Controls.Add(lblBalance);
            mainPanel.Controls.Add(lblBidAmount);
            mainPanel.Controls.Add(txtBidAmount);
            mainPanel.Controls.Add(quickBidPanel);
            mainPanel.Controls.Add(lblError);
            mainPanel.Controls.Add(btnPlaceBid);
            mainPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += PlaceBidForm_KeyDown;
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

        private void TxtBidAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numbers, decimal point, and control characters
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Allow only one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void TxtBidAmount_TextChanged(object sender, EventArgs e)
        {
            ClearError();
        }

        private void BtnPlaceBid_Click(object sender, EventArgs e)
        {
            PlaceBid();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void PlaceBidForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PlaceBid();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Business Logic

        private void ApplyQuickBid(string action)
        {
            if (!decimal.TryParse(txtBidAmount.Text, out decimal currentAmount))
                currentAmount = 0;

            decimal newAmount = currentAmount;

            switch (action)
            {
                case "+0.5":
                    newAmount = currentAmount + 0.5m;
                    break;
                case "+1.0":
                    newAmount = currentAmount + 1.0m;
                    break;
                case "+5.0":
                    newAmount = currentAmount + 5.0m;
                    break;
                case "+10.0":
                    newAmount = currentAmount + 10.0m;
                    break;
                case "MAX":
                    newAmount = SessionManager.Instance.Balance;
                    break;
            }

            txtBidAmount.Text = newAmount.ToString("F2");
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

        private bool ValidateBid()
        {
            // Validate bid amount
            if (!decimal.TryParse(txtBidAmount.Text, out decimal bidAmount) || bidAmount <= 0)
            {
                ShowError("Please enter a valid bid amount greater than 0.");
                txtBidAmount.Focus();
                txtBidAmount.SelectAll();
                return false;
            }

            // Calculate minimum required bid
            decimal minBid = currentBid.HasValue ? currentBid.Value + 0.1m : currentPrice + 0.1m;

            if (bidAmount < minBid)
            {
                ShowError($"Bid must be at least {minBid:C2}.");
                txtBidAmount.Focus();
                txtBidAmount.SelectAll();
                return false;
            }

            // Check balance
            if (bidAmount > SessionManager.Instance.Balance)
            {
                ShowError($"Insufficient balance. You have {SessionManager.Instance.GetFormattedBalance()}.");
                txtBidAmount.Focus();
                txtBidAmount.SelectAll();
                return false;
            }

            // Check if bid is too high (optional, for user protection)
            if (bidAmount > currentPrice * 10)
            {
                DialogResult result = MessageBox.Show(
                    $"Your bid of {bidAmount:C2} is significantly higher than the asking price of {currentPrice:C2}.\n\n" +
                    "Are you sure you want to continue?",
                    "High Bid Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return false;
            }

            return true;
        }

        private void PlaceBid()
        {
            if (!ValidateBid())
                return;

            decimal bidAmount = decimal.Parse(txtBidAmount.Text);

            // Confirm bid
            DialogResult confirm = MessageBox.Show(
                $"Place a bid of {bidAmount:C2} on '{nftTitle}'?\n\n" +
                $"Your current balance: {SessionManager.Instance.GetFormattedBalance()}\n" +
                $"Balance after bid: {(SessionManager.Instance.Balance - bidAmount):C2}\n\n" +
                "Note: Funds will be reserved but not deducted until you win the auction.",
                "Confirm Bid",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // Place bid using stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                new SqlParameter("@NFTID", nftId),
                new SqlParameter("@BidAmount", bidAmount),
                new SqlParameter("@Result", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
            };

            int result = DBHelper.ExecuteStoredProcedureWithOutput("sp_PlaceBid", parameters, "@Result");

            switch (result)
            {
                case 1: // Success
                    // Record transaction
                    string transactionQuery = @"
                        INSERT INTO Transactions (UserID, NFTID, Amount, TransactionType, Description)
                        VALUES (@UserID, @NFTID, @Amount, 'BID', 'Placed bid on NFT')";

                    SqlParameter[] transParams = new SqlParameter[]
                    {
                        new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                        new SqlParameter("@NFTID", nftId),
                        new SqlParameter("@Amount", bidAmount)
                    };

                    DBHelper.ExecuteNonQuery(transactionQuery, transParams);

                    UIHelper.ShowSuccess($"Successfully placed bid of {bidAmount:C2} on '{nftTitle}'!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;

                case -1: // NFT not found or sold
                    ShowError("This NFT is no longer available for bidding.");
                    break;

                case -2: // Bid too low
                    decimal minBid = currentBid.HasValue ? currentBid.Value + 0.1m : currentPrice + 0.1m;
                    ShowError($"Bid too low. Minimum bid is {minBid:C2}.");
                    txtBidAmount.Focus();
                    txtBidAmount.SelectAll();
                    break;

                default: // Error
                    ShowError("Failed to place bid. Please try again.");
                    break;
            }
        }

        #endregion
    }
}