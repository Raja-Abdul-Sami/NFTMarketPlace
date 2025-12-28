using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class AddFundsForm : Form
    {
        private Panel mainPanel;
        private Label lblTitle;
        private Label lblAmount;
        private TextBox txtAmount;
        private Button btnAdd;
        private Button btnCancel;
        private ComboBox cmbAmounts;
        private Label lblCurrentBalance;

        public AddFundsForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // AddFundsForm
            this.ClientSize = new Size(500, 350);
            this.Text = "Add Funds - TriApex";
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
            lblTitle.Text = "ADD FUNDS TO WALLET";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(50, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Current balance
            lblCurrentBalance = new Label();
            lblCurrentBalance.Text = $"Current Balance: {SessionManager.Instance.GetFormattedBalance()}";
            lblCurrentBalance.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            lblCurrentBalance.ForeColor = UIHelper.BlueAccent;
            lblCurrentBalance.Size = new Size(400, 30);
            lblCurrentBalance.Location = new Point(50, 90);
            lblCurrentBalance.TextAlign = ContentAlignment.MiddleCenter;

            // Amount label
            lblAmount = new Label();
            lblAmount.Text = "AMOUNT TO ADD ($)";
            lblAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAmount.ForeColor = UIHelper.TextPrimary;
            lblAmount.Size = new Size(200, 25);
            lblAmount.Location = new Point(50, 140);
            lblAmount.TextAlign = ContentAlignment.MiddleLeft;

            // Quick amount selector
            cmbAmounts = new ComboBox();
            cmbAmounts.Size = new Size(400, 35);
            cmbAmounts.Location = new Point(50, 170);
            cmbAmounts.Font = new Font("Segoe UI", 11);
            cmbAmounts.BackColor = Color.FromArgb(40, 40, 50);
            cmbAmounts.ForeColor = Color.White;
            cmbAmounts.FlatStyle = FlatStyle.Flat;
            cmbAmounts.DropDownStyle = ComboBoxStyle.DropDownList;

            // Add preset amounts
            cmbAmounts.Items.Add("Select preset amount...");
            cmbAmounts.Items.Add("$10");
            cmbAmounts.Items.Add("$25");
            cmbAmounts.Items.Add("$50");
            cmbAmounts.Items.Add("$100");
            cmbAmounts.Items.Add("$250");
            cmbAmounts.Items.Add("$500");
            cmbAmounts.Items.Add("$1000");
            cmbAmounts.SelectedIndex = 0;
            cmbAmounts.SelectedIndexChanged += CmbAmounts_SelectedIndexChanged;

            // Custom amount textbox
            txtAmount = new TextBox();
            txtAmount.Size = new Size(400, 40);
            txtAmount.Location = new Point(50, 220);
            txtAmount.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            txtAmount.BackColor = Color.FromArgb(40, 40, 50);
            txtAmount.ForeColor = UIHelper.GoldAccent;
            txtAmount.BorderStyle = BorderStyle.FixedSingle;
            txtAmount.TextAlign = HorizontalAlignment.Center;
            txtAmount.Text = "0.00";
            txtAmount.KeyPress += TxtAmount_KeyPress;

            // Add button
            btnAdd = new Button();
            btnAdd.Text = "ADD FUNDS";
            btnAdd.Size = new Size(180, 45);
            btnAdd.Location = new Point(80, 280);
            UIHelper.StyleButton(btnAdd, true);
            btnAdd.Click += BtnAdd_Click;

            // Cancel button
            btnCancel = new Button();
            btnCancel.Text = "CANCEL";
            btnCancel.Size = new Size(180, 45);
            btnCancel.Location = new Point(280, 280);
            UIHelper.StyleButton(btnCancel, false);
            btnCancel.Click += BtnCancel_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(lblCurrentBalance);
            mainPanel.Controls.Add(lblAmount);
            mainPanel.Controls.Add(cmbAmounts);
            mainPanel.Controls.Add(txtAmount);
            mainPanel.Controls.Add(btnAdd);
            mainPanel.Controls.Add(btnCancel);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += AddFundsForm_KeyDown;
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

        private void CmbAmounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAmounts.SelectedIndex > 0)
            {
                string amountText = cmbAmounts.SelectedItem.ToString().Replace("$", "");
                txtAmount.Text = amountText;
            }
        }

        private void TxtAmount_KeyPress(object sender, KeyPressEventArgs e)
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddFunds();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AddFundsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddFunds();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Business Logic

        private void AddFunds()
        {
            // Validate amount
            if (!decimal.TryParse(txtAmount.Text, out decimal amount) || amount <= 0)
            {
                UIHelper.ShowError("Please enter a valid amount greater than 0.");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            if (amount > 10000)
            {
                UIHelper.ShowError("Maximum amount per transaction is $10,000.");
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            // Confirm with user
            DialogResult confirm = MessageBox.Show($"Add ${amount:F2} to your wallet?\n\n" +
                                                  $"New balance: ${SessionManager.Instance.Balance + amount:F2}",
                                                  "Confirm Add Funds",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // Add funds using stored procedure
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
                new SqlParameter("@Amount", amount),
                new SqlParameter("@Result", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
            };

            int result = DBHelper.ExecuteStoredProcedureWithOutput("sp_AddFunds", parameters, "@Result");

            if (result == 1)
            {
                // Success
                SessionManager.Instance.RefreshBalance();

                UIHelper.ShowSuccess($"Successfully added ${amount:F2} to your wallet!");

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                UIHelper.ShowError("Failed to add funds. Please try again.");
            }
        }

        #endregion
    }
}