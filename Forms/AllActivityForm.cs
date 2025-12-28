using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;
using System.Data.SqlClient;

namespace TriApex.Forms
{
    public partial class AllActivityForm : Form
    {
        private DataGridView dgvAllActivity;
        private Button btnClose;
        private Button btnExport;
        private DataTable allActivity;

        public AllActivityForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ApplyThemeAndStyling();
            LoadAllActivity();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // AllActivityForm
            this.ClientSize = new Size(900, 600);
            this.Text = "All Activity - TriApex";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Main panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Paint += MainPanel_Paint;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "ALL ACTIVITY HISTORY";
            lblTitle.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(250, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Activity grid
            dgvAllActivity = new DataGridView();
            dgvAllActivity.Size = new Size(840, 450);
            dgvAllActivity.Location = new Point(30, 80);
            dgvAllActivity.BackgroundColor = Color.FromArgb(40, 40, 50);
            dgvAllActivity.BorderStyle = BorderStyle.None;
            dgvAllActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAllActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAllActivity.RowHeadersVisible = false;
            dgvAllActivity.AllowUserToAddRows = false;
            dgvAllActivity.AllowUserToDeleteRows = false;
            dgvAllActivity.ReadOnly = true;
            dgvAllActivity.DefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvAllActivity.DefaultCellStyle.BackColor = Color.FromArgb(50, 50, 60);
            dgvAllActivity.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAllActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 60, 70);
            dgvAllActivity.ColumnHeadersDefaultCellStyle.ForeColor = UIHelper.TextPrimary;
            dgvAllActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAllActivity.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 55);
            dgvAllActivity.EnableHeadersVisualStyles = false;

            // Export button
            btnExport = new Button();
            btnExport.Text = "📥 EXPORT TO CSV";
            btnExport.Size = new Size(150, 40);
            btnExport.Location = new Point(30, 540);
            UIHelper.StyleButton(btnExport, false);
            btnExport.Click += BtnExport_Click;

            // Close button
            btnClose = new Button();
            btnClose.Text = "CLOSE";
            btnClose.Size = new Size(150, 40);
            btnClose.Location = new Point(720, 540);
            UIHelper.StyleButton(btnClose, true);
            btnClose.Click += BtnClose_Click;

            // Add controls
            mainPanel.Controls.Add(lblTitle);
            mainPanel.Controls.Add(dgvAllActivity);
            mainPanel.Controls.Add(btnExport);
            mainPanel.Controls.Add(btnClose);

            this.Controls.Add(mainPanel);
        }

        private void ApplyThemeAndStyling()
        {
            // Rounded corners
            this.Region = Region.FromHrgn(UIHelper.CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Key events
            this.KeyPreview = true;
            this.KeyDown += AllActivityForm_KeyDown;
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw gradient background
            Rectangle rect = new Rectangle(0, 0, 900, 600);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(25, 25, 35),
                Color.FromArgb(20, 20, 28)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw border
            using (Pen pen = new Pen(UIHelper.GoldAccent, 2))
            {
                e.Graphics.DrawRectangle(pen, 1, 1, 898, 598);
            }
        }

        private void LoadAllActivity()
        {
            string query = @"
                SELECT 
                    TransactionType as Type,
                    Amount,
                    Description,
                    TransactionDate as Date,
                    CASE 
                        WHEN NFTID IS NOT NULL THEN (SELECT Title FROM NFTs WHERE NFTID = t.NFTID)
                        ELSE ''
                    END as NFT
                FROM Transactions t
                WHERE UserID = @UserID
                ORDER BY TransactionDate DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID)
            };

            allActivity = DBHelper.ExecuteQuery(query, parameters);

            // Configure DataGridView
            dgvAllActivity.DataSource = allActivity;

            // Format columns
            if (dgvAllActivity.Columns.Count > 0)
            {
                // Format Amount column
                dgvAllActivity.Columns["Amount"].DefaultCellStyle.Format = "C2";
                dgvAllActivity.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Format Date column
                dgvAllActivity.Columns["Date"].DefaultCellStyle.Format = "MMM dd, yyyy HH:mm";

                // Add cell formatting
                dgvAllActivity.CellFormatting += DgvAllActivity_CellFormatting;
            }
        }

        private void DgvAllActivity_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvAllActivity.Columns["Type"].Index)
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
                    case "PROFILE_UPDATE":
                    case "PASSWORD_CHANGE":
                        e.CellStyle.ForeColor = Color.FromArgb(100, 100, 120);
                        break;
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == dgvAllActivity.Columns["Amount"].Index)
            {
                if (decimal.TryParse(e.Value?.ToString(), out decimal amount))
                {
                    string transactionType = dgvAllActivity.Rows[e.RowIndex].Cells["Type"].Value?.ToString();

                    if (transactionType == "PURCHASE" || transactionType == "BID")
                    {
                        e.CellStyle.ForeColor = UIHelper.ErrorColor;
                        e.CellStyle.Font = new Font(dgvAllActivity.Font, FontStyle.Bold);
                    }
                    else if (transactionType == "SALE" || transactionType == "ADD_FUNDS")
                    {
                        e.CellStyle.ForeColor = UIHelper.SuccessColor;
                        e.CellStyle.Font = new Font(dgvAllActivity.Font, FontStyle.Bold);
                    }
                }
            }
        }

        #region Event Handlers

        private void BtnExport_Click(object sender, EventArgs e)
        {
            ExportToCSV();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AllActivityForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        #endregion

        #region Business Logic

        private void ExportToCSV()
        {
            if (allActivity == null || allActivity.Rows.Count == 0)
            {
                MessageBox.Show("No activity data to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                saveFileDialog.Title = "Export Activity to CSV";
                saveFileDialog.FileName = $"TriApex_Activity_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveFileDialog.FileName))
                        {
                            // Write headers
                            writer.WriteLine("Type,Amount,Description,Date,NFT");

                            // Write data
                            foreach (DataRow row in allActivity.Rows)
                            {
                                string type = EscapeCsvField(row["Type"].ToString());
                                string amount = EscapeCsvField(row["Amount"].ToString());
                                string description = EscapeCsvField(row["Description"].ToString());
                                string date = EscapeCsvField(Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd HH:mm:ss"));
                                string nft = EscapeCsvField(row["NFT"].ToString());

                                writer.WriteLine($"{type},{amount},{description},{date},{nft}");
                            }
                        }

                        MessageBox.Show($"Activity exported successfully to:\n{saveFileDialog.FileName}",
                            "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export: {ex.Message}", "Export Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }

        #endregion
    }
}