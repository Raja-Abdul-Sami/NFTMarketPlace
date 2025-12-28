using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TriApex.Helpers;
using System.Data.SqlClient;

namespace TriApex.UserControls
{
    public partial class SellNFTControl : UserControl
    {
        // UI Components
        private Panel mainFormPanel;
        private Panel previewPanel;
        private TextBox txtTitle;
        private TextBox txtDescription;
        private TextBox txtPrice;
        private ComboBox cmbCategory;
        private Button btnSelectImage;
        private Button btnCreateNFT;
        private Button btnClearForm;
        private PictureBox picNFTPreview;
        private Label lblFileName;
        private Label lblPreviewTitle;
        private Label lblPreviewPrice;
        private Label lblPreviewCategory;
        private Label lblPreviewDescription;

        // Data
        private string selectedImagePath = "";
        private byte[] imageBytes = null;

        // Categories
        private string[] categories = {
            "Art", "Collectibles", "Photography", "Sports", "Trading Cards",
            "Utility", "Virtual Worlds", "Domain Names", "Music", "Gaming",
            "Memes", "Metaverse", "Abstract", "Animals", "Cityscape",
            "Fantasy", "Nature", "Portrait", "Space", "Other"
        };

        public SellNFTControl()
        {
            InitializeComponent();
            InitializeCustomComponents();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // SellNFTControl
            this.BackColor = Color.Transparent;
            this.Size = new Size(950, 730);
            this.AutoScroll = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "CREATE & SELL NFT";
            lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblTitle.ForeColor = UIHelper.GoldAccent;
            lblTitle.Size = new Size(400, 50);
            lblTitle.Location = new Point(20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Subtitle
            Label lblSubtitle = new Label();
            lblSubtitle.Text = "List your digital artwork on the TriApex marketplace";
            lblSubtitle.Font = new Font("Segoe UI", 12);
            lblSubtitle.ForeColor = UIHelper.TextSecondary;
            lblSubtitle.Size = new Size(500, 30);
            lblSubtitle.Location = new Point(20, 70);
            lblSubtitle.TextAlign = ContentAlignment.MiddleLeft;

            // Main form container
            mainFormPanel = new Panel();
            mainFormPanel.Size = new Size(450, 600);
            mainFormPanel.Location = new Point(20, 120);
            mainFormPanel.BackColor = Color.FromArgb(35, 35, 45);
            mainFormPanel.Paint += MainFormPanel_Paint;

            // Form title
            Label lblFormTitle = new Label();
            lblFormTitle.Text = "NFT DETAILS";
            lblFormTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblFormTitle.ForeColor = UIHelper.GoldAccent;
            lblFormTitle.Size = new Size(400, 40);
            lblFormTitle.Location = new Point(25, 20);
            lblFormTitle.TextAlign = ContentAlignment.MiddleLeft;

            // NFT Title
            Label lblNFTTitle = new Label();
            lblNFTTitle.Text = "NFT TITLE *";
            lblNFTTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblNFTTitle.ForeColor = UIHelper.TextPrimary;
            lblNFTTitle.Size = new Size(200, 25);
            lblNFTTitle.Location = new Point(25, 70);
            lblNFTTitle.TextAlign = ContentAlignment.MiddleLeft;

            txtTitle = new TextBox();
            txtTitle.Size = new Size(400, 40);
            txtTitle.Location = new Point(25, 95);
            txtTitle.Font = new Font("Segoe UI", 11);
            txtTitle.BackColor = Color.FromArgb(50, 50, 60);
            txtTitle.ForeColor = Color.White;
            txtTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTitle.Text = "Enter a catchy title for your NFT...";
            txtTitle.MaxLength = 100;

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = "DESCRIPTION";
            lblDescription.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblDescription.ForeColor = UIHelper.TextPrimary;
            lblDescription.Size = new Size(200, 25);
            lblDescription.Location = new Point(25, 145);
            lblDescription.TextAlign = ContentAlignment.MiddleLeft;

            txtDescription = new TextBox();
            txtDescription.Multiline = true;
            txtDescription.ScrollBars = ScrollBars.Vertical;
            txtDescription.Size = new Size(400, 100);
            txtDescription.Location = new Point(25, 170);
            txtDescription.Font = new Font("Segoe UI", 10);
            txtDescription.BackColor = Color.FromArgb(50, 50, 60);
            txtDescription.ForeColor = Color.White;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.Text = "Describe your NFT... (max 500 characters)";
            txtDescription.MaxLength = 500;

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = "PRICE (ETH) *";
            lblPrice.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblPrice.ForeColor = UIHelper.TextPrimary;
            lblPrice.Size = new Size(200, 25);
            lblPrice.Location = new Point(25, 280);
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;

            txtPrice = new TextBox();
            txtPrice.Size = new Size(400, 40);
            txtPrice.Location = new Point(25, 305);
            txtPrice.Font = new Font("Segoe UI", 11);
            txtPrice.BackColor = Color.FromArgb(50, 50, 60);
            txtPrice.ForeColor = Color.White;
            txtPrice.BorderStyle = BorderStyle.FixedSingle;
            txtPrice.Text = "0.00";
            txtPrice.KeyPress += TxtPrice_KeyPress;

            // Category
            Label lblCategory = new Label();
            lblCategory.Text = "CATEGORY *";
            lblCategory.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCategory.ForeColor = UIHelper.TextPrimary;
            lblCategory.Size = new Size(200, 25);
            lblCategory.Location = new Point(25, 355);
            lblCategory.TextAlign = ContentAlignment.MiddleLeft;

            cmbCategory = new ComboBox();
            cmbCategory.Size = new Size(400, 35);
            cmbCategory.Location = new Point(25, 380);
            cmbCategory.Font = new Font("Segoe UI", 11);
            cmbCategory.BackColor = Color.FromArgb(50, 50, 60);
            cmbCategory.ForeColor = Color.White;
            cmbCategory.FlatStyle = FlatStyle.Flat;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // Image upload
            Label lblImage = new Label();
            lblImage.Text = "NFT IMAGE *";
            lblImage.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblImage.ForeColor = UIHelper.TextPrimary;
            lblImage.Size = new Size(200, 25);
            lblImage.Location = new Point(25, 425);
            lblImage.TextAlign = ContentAlignment.MiddleLeft;

            btnSelectImage = new Button();
            btnSelectImage.Text = "📁 SELECT IMAGE";
            btnSelectImage.Size = new Size(180, 40);
            btnSelectImage.Location = new Point(25, 450);
            UIHelper.StyleButton(btnSelectImage, false);
            btnSelectImage.Click += BtnSelectImage_Click;

            lblFileName = new Label();
            lblFileName.Text = "No file selected";
            lblFileName.Font = new Font("Segoe UI", 9);
            lblFileName.ForeColor = UIHelper.TextSecondary;
            lblFileName.Size = new Size(200, 25);
            lblFileName.Location = new Point(215, 460);
            lblFileName.TextAlign = ContentAlignment.MiddleLeft;

            // Image requirements
            Label lblImageRequirements = new Label();
            lblImageRequirements.Text = "Supported: JPG, PNG, GIF | Max: 5MB";
            lblImageRequirements.Font = new Font("Segoe UI", 8);
            lblImageRequirements.ForeColor = UIHelper.TextSecondary;
            lblImageRequirements.Size = new Size(200, 20);
            lblImageRequirements.Location = new Point(25, 495);
            lblImageRequirements.TextAlign = ContentAlignment.MiddleLeft;

            // Buttons
            btnCreateNFT = new Button();
            btnCreateNFT.Text = "CREATE & LIST NFT";
            btnCreateNFT.Size = new Size(195, 45);
            btnCreateNFT.Location = new Point(25, 530);
            UIHelper.StyleButton(btnCreateNFT, true);
            btnCreateNFT.Click += BtnCreateNFT_Click;

            btnClearForm = new Button();
            btnClearForm.Text = "CLEAR FORM";
            btnClearForm.Size = new Size(195, 45);
            btnClearForm.Location = new Point(230, 530);
            UIHelper.StyleButton(btnClearForm, false);
            btnClearForm.Click += BtnClearForm_Click;

            // Add controls to main form panel
            mainFormPanel.Controls.Add(lblFormTitle);
            mainFormPanel.Controls.Add(lblNFTTitle);
            mainFormPanel.Controls.Add(txtTitle);
            mainFormPanel.Controls.Add(lblDescription);
            mainFormPanel.Controls.Add(txtDescription);
            mainFormPanel.Controls.Add(lblPrice);
            mainFormPanel.Controls.Add(txtPrice);
            mainFormPanel.Controls.Add(lblCategory);
            mainFormPanel.Controls.Add(cmbCategory);
            mainFormPanel.Controls.Add(lblImage);
            mainFormPanel.Controls.Add(btnSelectImage);
            mainFormPanel.Controls.Add(lblFileName);
            mainFormPanel.Controls.Add(lblImageRequirements);
            mainFormPanel.Controls.Add(btnCreateNFT);
            mainFormPanel.Controls.Add(btnClearForm);

            // Preview panel
            previewPanel = new Panel();
            previewPanel.Size = new Size(430, 600);
            previewPanel.Location = new Point(490, 120);
            previewPanel.BackColor = Color.FromArgb(35, 35, 45);
            previewPanel.Paint += PreviewPanel_Paint;

            // Preview title
            Label lblPreviewHeader = new Label();
            lblPreviewHeader.Text = "NFT PREVIEW";
            lblPreviewHeader.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblPreviewHeader.ForeColor = UIHelper.GoldAccent;
            lblPreviewHeader.Size = new Size(400, 40);
            lblPreviewHeader.Location = new Point(15, 20);
            lblPreviewHeader.TextAlign = ContentAlignment.MiddleCenter;

            // Image preview
            picNFTPreview = new PictureBox();
            picNFTPreview.Size = new Size(400, 300);
            picNFTPreview.Location = new Point(15, 70);
            picNFTPreview.BackColor = Color.FromArgb(50, 50, 65);
            picNFTPreview.SizeMode = PictureBoxSizeMode.Zoom;
            picNFTPreview.BorderStyle = BorderStyle.FixedSingle;
            picNFTPreview.Paint += PicNFTPreview_Paint;

            // Preview details
            Panel previewDetails = new Panel();
            previewDetails.Size = new Size(400, 250);
            previewDetails.Location = new Point(15, 380);
            previewDetails.BackColor = Color.FromArgb(40, 40, 50);
            previewDetails.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, previewDetails.ClientRectangle,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                    Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
            };

            // Preview title
            lblPreviewTitle = new Label();
            lblPreviewTitle.Text = "Your NFT Title";
            lblPreviewTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblPreviewTitle.ForeColor = UIHelper.TextPrimary;
            lblPreviewTitle.Size = new Size(380, 40);
            lblPreviewTitle.Location = new Point(10, 20);
            lblPreviewTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Preview price
            lblPreviewPrice = new Label();
            lblPreviewPrice.Text = "Price: -- ETH";
            lblPreviewPrice.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPreviewPrice.ForeColor = UIHelper.GoldAccent;
            lblPreviewPrice.Size = new Size(380, 30);
            lblPreviewPrice.Location = new Point(10, 70);
            lblPreviewPrice.TextAlign = ContentAlignment.MiddleLeft;

            // Preview category
            lblPreviewCategory = new Label();
            lblPreviewCategory.Text = "Category: --";
            lblPreviewCategory.Font = new Font("Segoe UI", 11);
            lblPreviewCategory.ForeColor = UIHelper.BlueAccent;
            lblPreviewCategory.Size = new Size(380, 25);
            lblPreviewCategory.Location = new Point(10, 105);
            lblPreviewCategory.TextAlign = ContentAlignment.MiddleLeft;

            // Preview description
            lblPreviewDescription = new Label();
            lblPreviewDescription.Text = "Description will appear here...";
            lblPreviewDescription.Font = new Font("Segoe UI", 10);
            lblPreviewDescription.ForeColor = UIHelper.TextSecondary;
            lblPreviewDescription.Size = new Size(380, 100);
            lblPreviewDescription.Location = new Point(10, 140);
            lblPreviewDescription.TextAlign = ContentAlignment.TopLeft;

            // Creator info
            Label lblCreator = new Label();
            lblCreator.Text = $"Creator: {SessionManager.Instance.Username}";
            lblCreator.Font = new Font("Segoe UI", 10);
            lblCreator.ForeColor = UIHelper.TextSecondary;
            lblCreator.Size = new Size(380, 25);
            lblCreator.Location = new Point(10, 215);
            lblCreator.TextAlign = ContentAlignment.MiddleLeft;

            previewDetails.Controls.Add(lblPreviewTitle);
            previewDetails.Controls.Add(lblPreviewPrice);
            previewDetails.Controls.Add(lblPreviewCategory);
            previewDetails.Controls.Add(lblPreviewDescription);
            previewDetails.Controls.Add(lblCreator);

            // Add controls to preview panel
            previewPanel.Controls.Add(lblPreviewHeader);
            previewPanel.Controls.Add(picNFTPreview);
            previewPanel.Controls.Add(previewDetails);

            // Add all to main control
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(mainFormPanel);
            this.Controls.Add(previewPanel);

            // Wire up events for live preview
            txtTitle.TextChanged += UpdatePreview;
            txtDescription.TextChanged += UpdatePreview;
            txtPrice.TextChanged += UpdatePreview;
            cmbCategory.SelectedIndexChanged += UpdatePreview;
        }

        private void LoadCategories()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.AddRange(categories);
            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        #region Event Handlers

        private void MainFormPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, mainFormPanel.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
        }

        private void PreviewPanel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, previewPanel.ClientRectangle,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid,
                Color.FromArgb(60, 60, 70), 1, ButtonBorderStyle.Solid);
        }

        private void PicNFTPreview_Paint(object sender, PaintEventArgs e)
        {
            if (picNFTPreview.Image == null)
            {
                // Draw placeholder
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 50, 65)),
                    picNFTPreview.ClientRectangle);

                // Draw upload icon
                using (Font font = new Font("Segoe UI", 48))
                using (Brush brush = new SolidBrush(Color.FromArgb(100, 100, 120)))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString("📁", font, brush,
                        new Rectangle(0, 0, picNFTPreview.Width, picNFTPreview.Height), format);
                }

                // Draw text
                using (Font font = new Font("Segoe UI", 12))
                using (Brush brush = new SolidBrush(Color.FromArgb(150, 150, 170)))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString("Upload an image to preview", font, brush,
                        new Rectangle(0, picNFTPreview.Height - 50, picNFTPreview.Width, 30), format);
                }
            }
        }

        private void TxtPrice_KeyPress(object sender, KeyPressEventArgs e)
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

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog.Title = "Select NFT Image";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    string path = openFileDialog.FileName;

                    // Validate path
                    if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                    {
                        MessageBox.Show("Invalid file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check file size (max 5MB)
                    FileInfo fileInfo = new FileInfo(path);
                    if (fileInfo.Length > 5 * 1024 * 1024)
                    {
                        MessageBox.Show("File size must be less than 5MB.", "File Too Large",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    selectedImagePath = path;
                    lblFileName.Text = Path.GetFileName(selectedImagePath);

                    // Load image safely
                    byte[] imgBytes = File.ReadAllBytes(selectedImagePath);
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        using (Image originalImage = Image.FromStream(ms))
                        {
                            Image imageForPreview = originalImage;

                            // Resize if too large for preview
                            if (originalImage.Width > 800 || originalImage.Height > 600)
                            {
                                double ratio = Math.Min(800.0 / originalImage.Width, 600.0 / originalImage.Height);
                                int newWidth = (int)(originalImage.Width * ratio);
                                int newHeight = (int)(originalImage.Height * ratio);
                                imageForPreview = new Bitmap(originalImage, new Size(newWidth, newHeight));
                            }

                            // Set preview
                            picNFTPreview.Image = imageForPreview;

                            // Convert original image to byte array for database
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                originalImage.Save(ms2, originalImage.RawFormat);
                                imageBytes = ms2.ToArray();
                            }
                        }
                    }

                    UpdatePreview(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdatePreview(object sender, EventArgs e)
        {
            // Update title
            if (!string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                string displayTitle = txtTitle.Text.Length > 30 ?
                    txtTitle.Text.Substring(0, 27) + "..." : txtTitle.Text;
                lblPreviewTitle.Text = displayTitle;
            }
            else
            {
                lblPreviewTitle.Text = "Your NFT Title";
            }

            // Update price
            if (decimal.TryParse(txtPrice.Text, out decimal price) && price > 0)
            {
                lblPreviewPrice.Text = $"Price: {price:C2}";
            }
            else
            {
                lblPreviewPrice.Text = "Price: -- ETH";
            }

            // Update category
            if (cmbCategory.SelectedItem != null)
            {
                lblPreviewCategory.Text = $"Category: {cmbCategory.SelectedItem}";
            }
            else
            {
                lblPreviewCategory.Text = "Category: --";
            }

            // Update description
            if (!string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                string displayDesc = txtDescription.Text.Length > 100 ?
                    txtDescription.Text.Substring(0, 97) + "..." : txtDescription.Text;
                lblPreviewDescription.Text = displayDesc;
            }
            else
            {
                lblPreviewDescription.Text = "Description will appear here...";
            }
        }

        private void BtnCreateNFT_Click(object sender, EventArgs e)
        {
            CreateNFT();
        }

        private void BtnClearForm_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        #endregion

        #region Business Logic

        private bool ValidateForm()
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title for your NFT.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return false;
            }

            if (txtTitle.Text.Length < 3)
            {
                MessageBox.Show("Title must be at least 3 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return false;
            }

            // Validate price
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            if (price > 1000000)
            {
                MessageBox.Show("Price cannot exceed $1,000,000.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            // Validate category
            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            // Validate image
            if (string.IsNullOrEmpty(selectedImagePath) || imageBytes == null)
            {
                MessageBox.Show("Please select an image for your NFT.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSelectImage.Focus();
                return false;
            }

            return true;
        }

        private void CreateNFT()
        {
            if (!ValidateForm())
                return;

            decimal price = decimal.Parse(txtPrice.Text);
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            string category = cmbCategory.SelectedItem.ToString();

            // Confirm creation
            DialogResult confirm = MessageBox.Show(
                $"Create NFT: {title}\n\n" +
                $"Price: {price:C2}\n" +
                $"Category: {category}\n\n" +
                "This will list your NFT for sale on the marketplace.",
                "Confirm NFT Creation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                // Generate a unique image filename
                string imageFileName = $"{Guid.NewGuid()}_{Path.GetFileName(selectedImagePath)}";
                string imagePath = Path.Combine("NFT_Images", imageFileName);

                // In a real application, you would save the image to a server or IPFS
                // For this demo, we'll store the path only

                // Insert NFT into database
                string query = @"
                    INSERT INTO NFTs (
                        Title, 
                        Description, 
                        Price, 
                        ImagePath, 
                        OwnerID, 
                        IsSold, 
                        CreatedBy, 
                        Category,
                        CreatedDate
                    ) 
                    VALUES (
                        @Title, 
                        @Description, 
                        @Price, 
                        @ImagePath, 
                        @OwnerID, 
                        0, 
                        @CreatedBy, 
                        @Category,
                        GETDATE()
                    )";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Description", description ?? (object)DBNull.Value),
                    new SqlParameter("@Price", price),
                    new SqlParameter("@ImagePath", imagePath ?? (object)DBNull.Value),
                    new SqlParameter("@OwnerID", SessionManager.Instance.CurrentUserID),
                    new SqlParameter("@CreatedBy", SessionManager.Instance.Username),
                    new SqlParameter("@Category", category)
                };

                // Insert NFT and get its ID in one step
                string insertQuery = @"
    INSERT INTO NFTs (
        Title, Description, Price, ImagePath, OwnerID, IsSold, CreatedBy, Category, CreatedDate
    )
    OUTPUT INSERTED.NFTID
    VALUES (
        @Title, @Description, @Price, @ImagePath, @OwnerID, 0, @CreatedBy, @Category, GETDATE()
    )";

                object nftIdObj = DBHelper.ExecuteScalar(insertQuery, parameters);
                int nftId = nftIdObj != null ? Convert.ToInt32(nftIdObj) : 0;

                if (nftId == 0)
                {
                    MessageBox.Show("Failed to retrieve NFT ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Record transaction
                string transactionQuery = @"
    INSERT INTO Transactions (UserID, NFTID, Amount, TransactionType, Description)
    VALUES (@UserID, @NFTID, 0, 'CREATE', 'Created new NFT listing')";

                SqlParameter[] transParams = new SqlParameter[]
                {
    new SqlParameter("@UserID", SessionManager.Instance.CurrentUserID),
    new SqlParameter("@NFTID", nftId)
                };

                DBHelper.ExecuteNonQuery(transactionQuery, transParams);

                // Show success message
                MessageBox.Show(
                    $"🎉 Successfully created NFT: {title}\n\n" +
                    $"Your NFT is now listed on the marketplace for {price:C2}.\n" +
                    "You can view it in 'My NFTs' > 'Listed NFTs'.",
                    "NFT Created Successfully",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Clear form
                ClearForm();

                // Navigate to My NFTs tab
                NavigateToMyNFTs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating NFT: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtPrice.Text = "";
            cmbCategory.SelectedIndex = 0;
            selectedImagePath = "";
            imageBytes = null;
            picNFTPreview.Image = null;
            lblFileName.Text = "No file selected";

            // Reset preview
            UpdatePreview(null, null);

            // Refresh preview panel
            picNFTPreview.Invalidate();

            txtTitle.Focus();
        }

        private void NavigateToMyNFTs()
        {
            // Find parent MainDashboardForm
            Control parent = this.Parent;
            while (parent != null && !(parent is Forms.MainDashboardForm))
            {
                parent = parent.Parent;
            }

            if (parent is Forms.MainDashboardForm dashboard)
            {
                // Switch to My NFTs tab
                // This requires exposing a method in MainDashboardForm to change tabs
                // For now, show a message
                MessageBox.Show("Your NFT has been created! Go to 'My NFTs' to view it.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw decorative elements
            using (Pen pen = new Pen(Color.FromArgb(60, 60, 70), 2))
            {
                // Vertical separator between form and preview
                e.Graphics.DrawLine(pen, 480, 120, 480, 720);
            }
        }

        #region Helper Methods for Image Processing

        private byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        #endregion
    }
}