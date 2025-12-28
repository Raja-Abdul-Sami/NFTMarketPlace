using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class UpdatePriceForm : Form
    {
        private int nftId;
        private string nftTitle;
        private decimal currentPrice;
        private bool isOwned;

        private Panel mainPanel;
        private Label lblTitle;
        private Label lblNFTInfo;
        private Label lblCurrentPrice;
        private TextBox txtNewPrice;
        private Button btnUpdate;
        private Button btnCancel;
        private Label lblError;
        private Label lblRecommendation;

        public UpdatePriceForm(int nftId, string nftTitle, decimal currentPrice, bool isOwned)
        {
            this.nftId = nftId;
            this.nftTitle = nftTitle;
            this.currentPrice = currentPrice;
            this.isOwned = isOwned;

            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UpdatePriceForm
            this.ClientSize = new Size(500, 350);
            this.Text = "Update Price - TriApex";
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
            lblTitle.Text = isOwned ? "LIST NFT FOR SALE" : "UPDATE NFT PRICE";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(50, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // NFT info
            string displayTitle = nftTitle.Length > 40 ? nftTitle.Substring(0, 37) + "..." : nftTitle;
            lblNFTInfo = new Label();
            lblNFTInfo.Text = $"NFT: {displayTitle}";
            lblNFTInfo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblNFTInfo.ForeColor = UIHelper.TextPrimary;
            lblNFTInfo.Size = new Size(400, 40);
            lblNFTInfo.Location = new Point(50, 80);
            lblNFTInfo.TextAlign = ContentAlignment.MiddleCenter;

            // Current price
            lblCurrentPrice = new Label();
            if (isOwned)
            {
                lblCurrentPrice.Text = $"Current Value: {currentPrice:C2}";
            }
            else
            {
                lblCurrentPrice.Text = $"Current Price: {currentPrice:C2}";
            }
            lblCurrentPrice.Font = new Font("Segoe UI", 12);
            lblCurrentPrice.ForeColor = UIHelper.TextSecondary;
            lblCurrentPrice.Size = new Size(400, 30);
            lblCurrentPrice.Location = new Point(50, 130);
            lblCurrentPrice.TextAlign = ContentAlignment.MiddleCenter;

            // New price label
            Label lblNewPrice = new Label();
            lblNewPrice.Text = isOwned ? "SET SALE PRICE ($)" : "NEW PRICE ($)";
            lblNewPrice.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblNewPrice.ForeColor = UIHelper.TextPrimary;
            lblNewPrice.Size = new Size(200, 25);
            lblNewPrice.Location = new Point(50, 170);
            lblNewPrice.TextAlign = ContentAlignment.MiddleLeft;

            // New price textbox
            txtNewPrice = new TextBox();
            txtNewPrice.Size = new Size(400, 40);
            txtNewPrice.Location = new Point(50, 200);
            txtNewPrice.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtNewPrice.BackColor = Color.FromArgb(40, 40, 50);
            txtNewPrice.ForeColor = UIHelper.GoldAccent;
            txtNewPrice.BorderStyle = BorderStyle.FixedSingle;
            txtNewPrice.TextAlign = HorizontalAlignment.Center;
            txtNewPrice.Text = currentPrice.ToString("F2");
            txtNewPrice.KeyPress += TxtNewPrice_KeyPress;
            txtNewPrice.TextChanged += TxtNewPrice_TextChanged;

            // Recommendation
            lblRecommendation = new Label();
            if (isOwned)
            {
                lblRecommendation.Text = "💡 Tip: Price it fairly to attract buyers!";
            }
            else
            {
                lblRecommendation.Text = "💡 Tip: Consider market trends and bid activity.";
            }
            lblRecommendation.Font = new Font("Segoe UI", 10);
            lblRecommendation.ForeColor = UIHelper.BlueAccent;
            lblRecommendation.Size = new Size(400, 30);
            lblRecommendation.Location = new Point(50, 245);
            lblRecommendation.TextAlign = ContentAlignment.MiddleCenter;

            // Quick price buttons
            Panel quickPricePanel = new Panel();
            quickPricePanel.Size = new Size(400, 40);
            quickPricePanel.Location = new Point(50, 270);
            quickPricePanel.BackColor = Color.Transparent;

            string[] quickPercentages = { "-10%", "-5%", "+5%", "+10%", "+25%" };
            for (int i = 0; i < quickPercentages.Length; i++)
            {
                Button quickBtn = new Button();
                quickBtn.Text = quickPercentages[i];
                quickBtn.Size = new Size(70, 30);
                quickBtn.Location = new Point(i * 80, 5);
                UIHelper.StyleButton(quickBtn, false);
                quickBtn.Font = new Font("Segoe UI", 9);
                quickBtn.Click += (s, e) => ApplyQuickPercentage(quickPercentages[i]);
                quickPricePanel.Controls.Add(quickBtn);
            }

            // Error label
            lblError = new Label();
            lblError.Text = "";
            lblError.Font = new Font("Segoe UI", 9);
            lblError.ForeColor = UIHelper.ErrorColor;
            lblError.Size = new Size(400, 30);
            lblError.Location = new Point(50, 315);
            lblError.TextAlign = ContentAlignment.MiddleCenter;
            lblError.Visible = false;

            // Update button
            btnUpdate = new Button();
            btnUpdate.Text = isOwned ? "LIST FOR SALE" : "UPDATE PRICE";
            btnUpdate.Size = new Size(180, 45);
            btnUpdate.Location = new Point(80, 350);
            UIHelper.StyleButton(btnUpdate, true);
            btnUpdate.Click += BtnUpdate_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(180, 45);
            btnCancel.Location = new Point(280, 350);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblNFTInfo);
            mainPanel.Controls.Add(lblCurrentPrice);
            mainPanel.Controls.Add(lblNewPrice);
            mainPanel.Controls.Add(txtNewPrice);
            mainPanel.Controls.Add(lblRecommendation);
            mainPanel.Controls.Add(quickPricePanel);
            mainPanel.Controls.Add(lblError);
            mainPanel.Controls.Add(btnUpdate);
            mainPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += UpdatePriceForm_KeyDown;
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

        private void TxtNewPrice_KeyPress(object sender, KeyPressEventArgs e)
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

        private void TxtNewPrice_TextChanged(object sender, EventArgs e)
        {
            ClearError();

            // Update recommendation based on price change
            if (decimal.TryParse(txtNewPrice.Text, out decimal newPrice))
            {
                decimal percentageChange = ((newPrice - currentPrice) / currentPrice) * 100;

                if (percentageChange > 50)
                {
                    lblRecommendation.Text = "⚠️ Warning: Price increase over 50% may deter buyers.";
                    lblRecommendation.ForeColor = UIHelper.ErrorColor;
                }
                else if (percentageChange < -30)
                {
                    lblRecommendation.Text = "⚠️ Warning: Price decrease over 30% may seem suspicious.";
                    lblRecommendation.ForeColor = UIHelper.ErrorColor;
                }
                else if (percentageChange > 0)
                {
                    lblRecommendation.Text = "📈 Price increase: Good if demand is high.";
                    lblRecommendation.ForeColor = UIHelper.SuccessColor;
                }
                else if (percentageChange < 0)
                {
                    lblRecommendation.Text = "📉 Price decrease: May attract more buyers.";
                    lblRecommendation.ForeColor = UIHelper.BlueAccent;
                }
                else
                {
                    lblRecommendation.Text = "💡 Tip: Consider market trends and bid activity.";
                    lblRecommendation.ForeColor = UIHelper.BlueAccent;
                }
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            UpdatePrice();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void UpdatePriceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdatePrice();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Business Logic

        private void ApplyQuickPercentage(string percentage)
        {
            if (!decimal.TryParse(txtNewPrice.Text, out decimal currentValue))
                currentValue = currentPrice;

            decimal multiplier = 1.0m;

            switch (percentage)
            {
                case "-10%":
                    multiplier = 0.9m;
                    break;
                case "-5%":
                    multiplier = 0.95m;
                    break;
                case "+5%":
                    multiplier = 1.05m;
                    break;
                case "+10%":
                    multiplier = 1.10m;
                    break;
                case "+25%":
                    multiplier = 1.25m;
                    break;
            }

            decimal newValue = currentPrice * multiplier;
            txtNewPrice.Text = newValue.ToString("F2");
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

        private bool ValidatePrice()
        {
            // Validate price
            if (!decimal.TryParse(txtNewPrice.Text, out decimal newPrice) || newPrice <= 0)
            {
                ShowError("Please enter a valid price greater than 0.");
                txtNewPrice.Focus();
                txtNewPrice.SelectAll();
                return false;
            }

            if (newPrice > 1000000)
            {
                ShowError("Price cannot exceed $1,000,000.");
                txtNewPrice.Focus();
                txtNewPrice.SelectAll();
                return false;
            }

            // Check if price change is too extreme
            decimal percentageChange = Math.Abs((newPrice - currentPrice) / currentPrice) * 100;
            if (percentageChange > 1000) // 1000% change
            {
                DialogResult result = MessageBox.Show(
                    $"You're changing the price by {percentageChange:F0}%.\n\n" +
                    "Are you sure you want to continue?",
                    "Extreme Price Change",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                    return false;
            }

            return true;
        }

        private void UpdatePrice()
        {
            if (!ValidatePrice())
                return;

            decimal newPrice = decimal.Parse(txtNewPrice.Text);

            string action = isOwned ? "list for sale" : "update price";
            string confirmationMessage = isOwned ?
                $"List '{nftTitle}' for sale at {newPrice:C2}?\n\n" +
                "This will make it available in the marketplace." :
                $"Update '{nftTitle}' price from {currentPrice:C2} to {newPrice:C2}?";

            DialogResult confirm = MessageBox.Show(
                confirmationMessage,
                $"Confirm {action}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // Update price in database
            string query = "";
            if (isOwned)
            {
                // List for sale - update price and ensure it's owned by the NFT (not the creator)
                query = @"
                    UPDATE NFTs 
                    SET Price = @NewPrice,
                        OwnerID = @UserID
                    WHERE NFTID = @NFTID";
            }
            else
            {
                // Just update price
                query = @"
                    UPDATE NFTs 
                    SET Price = @NewPrice
                    WHERE NFTID = @NFTID";
            }

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NewPrice", newPrice),
                new SqlParameter("@NFTID", nftId),
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            int rowsAffected = DBHelper.ExecuteNonQuery(query, parameters);

            if (rowsAffected > 0)
            {
                // Record transaction
                string transactionQuery = @"
                    INSERT INTO Transactions (UserID, NFTID, Amount, TransactionType, Description)
                    VALUES (@UserID, @NFTID, 0, 'PRICE_UPDATE', @Description)";

                string description = isOwned ?
                    $"Listed NFT for sale at {newPrice:C2}" :
                    $"Updated price from {currentPrice:C2} to {newPrice:C2}";

                SqlParameter[] transParams = new SqlParameter[]
                {
                    new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                    new SqlParameter("@NFTID", nftId),
                    new SqlParameter("@Description", description)
                };

                DBHelper.ExecuteNonQuery(transactionQuery, transParams);

                UIHelper.ShowSuccess(isOwned ?
                    $"Successfully listed '{nftTitle}' for sale at {newPrice:C2}!" :
                    $"Successfully updated price to {newPrice:C2}!");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                ShowError("Failed to update price. Please try again.");
            }
        }

        #endregion
    }
}