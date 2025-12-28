using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TriApex.Helpers
{
    /// <summary>
    /// UI helper class for consistent styling and effects
    /// </summary>
    public static class UIHelper
    {
        // Brand colors
        public static Color DarkBackground = Color.FromArgb(18, 18, 24);
        public static Color PanelBackground = Color.FromArgb(28, 28, 36);
        public static Color GoldAccent = Color.FromArgb(212, 175, 55);
        public static Color BlueAccent = Color.FromArgb(0, 150, 255);
        public static Color TextPrimary = Color.White;
        public static Color TextSecondary = Color.FromArgb(180, 180, 200);
        public static Color SuccessColor = Color.FromArgb(46, 204, 113);
        public static Color ErrorColor = Color.FromArgb(231, 76, 60);

        /// <summary>
        /// Apply dark theme to a form
        /// </summary>
        public static void ApplyDarkTheme(Form form)
        {
            form.BackColor = DarkBackground;
            form.ForeColor = TextPrimary;
            form.Font = new Font("Segoe UI", 9, FontStyle.Regular);
        }

        /// <summary>
        /// Style a button with hover effects
        /// </summary>
        public static void StyleButton(Button button, bool isPrimary = false)
        {
            // Basic styling
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            button.Cursor = Cursors.Hand;

            if (isPrimary)
            {
                // Primary button (gold)
                button.BackColor = GoldAccent;
                button.ForeColor = Color.Black;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 195, 75);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(192, 155, 35);
            }
            else
            {
                // Secondary button (blue)
                button.BackColor = Color.Transparent;
                button.ForeColor = BlueAccent;
                button.FlatAppearance.BorderColor = BlueAccent;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 40, 50);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 50, 60);
            }

            // Rounded corners
            button.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width, button.Height, 8, 8));
        }

        /// <summary>
        /// Style a panel with rounded corners
        /// </summary>
        public static void StylePanel(Panel panel, bool withBorder = false)
        {
            panel.BackColor = PanelBackground;

            if (withBorder)
            {
                panel.BorderStyle = BorderStyle.None;
                panel.Paint += (sender, e) =>
                {
                    Panel p = sender as Panel;
                    using (Pen pen = new Pen(BlueAccent, 1))
                    {
                        e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
                    }
                };
            }

            // Rounded corners
            panel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, panel.Width, panel.Height, 10, 10));
        }

        /// <summary>
        /// Create a rounded rectangle region (for rounded corners)
        /// </summary>
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect,
            int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        /// <summary>
        /// Show a styled message box
        /// </summary>
        public static DialogResult ShowMessage(string message, string title = "TriApex",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return MessageBox.Show(message, title, buttons, icon);
        }

        /// <summary>
        /// Show success message
        /// </summary>
        public static void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show error message
        /// </summary>
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Create a gradient brush
        /// </summary>
        public static LinearGradientBrush CreateGradientBrush(Rectangle rect, Color startColor, Color endColor)
        {
            return new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical);
        }

        /// <summary>
        /// Apply hover effect to control
        /// </summary>
        public static void ApplyHoverEffect(Control control, Color normalColor, Color hoverColor)
        {
            Color originalColor = control.BackColor;

            control.MouseEnter += (sender, e) =>
            {
                control.BackColor = hoverColor;
            };

            control.MouseLeave += (sender, e) =>
            {
                control.BackColor = originalColor;
            };
        }

        /// <summary>
        /// Create a glowing border effect
        /// </summary>
        public static void ApplyGlowBorder(Control control, PaintEventHandler paintHandler)
        {
            control.Paint += paintHandler;
        }

        /// <summary>
        /// Center control horizontally in parent
        /// </summary>
        public static void CenterControlHorizontally(Control control, Control parent)
        {
            control.Left = (parent.Width - control.Width) / 2;
        }

        /// <summary>
        /// Center control vertically in parent
        /// </summary>
        public static void CenterControlVertically(Control control, Control parent)
        {
            control.Top = (parent.Height - control.Height) / 2;
        }

        /// <summary>
        /// Center control in parent
        /// </summary>
        public static void CenterControl(Control control, Control parent)
        {
            control.Left = (parent.Width - control.Width) / 2;
            control.Top = (parent.Height - control.Height) / 2;
        }

        /// <summary>
        /// Create a card-style panel for NFT display
        /// </summary>
        public static Panel CreateNFTCard(string title, decimal price, string imagePath,
            decimal? currentBid = null, EventHandler buyClick = null, EventHandler bidClick = null)
        {
            Panel card = new Panel();
            card.Size = new Size(220, 320);
            card.BackColor = PanelBackground;
            card.Padding = new Padding(10);
            card.Margin = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Rounded corners
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 12, 12));

            // Title label
            Label titleLabel = new Label();
            titleLabel.Text = title;
            titleLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleLabel.ForeColor = TextPrimary;
            titleLabel.Location = new Point(10, 10);
            titleLabel.Size = new Size(200, 25);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Image placeholder
            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(180, 180);
            pictureBox.Location = new Point(20, 40);
            pictureBox.BackColor = Color.FromArgb(40, 40, 50);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.None;

            // Try to load image if path exists
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    // For now, use placeholder - we'll implement actual image loading later
                    pictureBox.Image = Properties.Resources.TriApexLogo;
                }
                catch
                {
                    pictureBox.Image = null;
                }
            }

            // Price label
            Label priceLabel = new Label();
            priceLabel.Text = $"Price: {price:C2}";
            priceLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            priceLabel.ForeColor = GoldAccent;
            priceLabel.Location = new Point(10, 230);
            priceLabel.Size = new Size(200, 20);
            priceLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Current bid label (if exists)
            Label bidLabel = new Label();
            if (currentBid.HasValue)
            {
                bidLabel.Text = $"Current Bid: {currentBid.Value:C2}";
                bidLabel.ForeColor = BlueAccent;
            }
            else
            {
                bidLabel.Text = "No bids yet";
                bidLabel.ForeColor = TextSecondary;
            }
            bidLabel.Font = new Font("Segoe UI", 8);
            bidLabel.Location = new Point(10, 255);
            bidLabel.Size = new Size(200, 20);
            bidLabel.TextAlign = ContentAlignment.MiddleLeft;

            // Buy Now button
            Button buyButton = new Button();
            buyButton.Text = "BUY NOW";
            buyButton.Size = new Size(90, 30);
            buyButton.Location = new Point(10, 280);
            StyleButton(buyButton, true);
            if (buyClick != null)
                buyButton.Click += buyClick;

            // Place Bid button
            Button bidButton = new Button();
            bidButton.Text = "PLACE BID";
            bidButton.Size = new Size(90, 30);
            bidButton.Location = new Point(110, 280);
            StyleButton(bidButton, false);
            if (bidClick != null)
                bidButton.Click += bidClick;

            // Add controls to card
            card.Controls.Add(titleLabel);
            card.Controls.Add(pictureBox);
            card.Controls.Add(priceLabel);
            card.Controls.Add(bidLabel);
            card.Controls.Add(buyButton);
            card.Controls.Add(bidButton);

            // Hover effect
            ApplyHoverEffect(card, PanelBackground, Color.FromArgb(35, 35, 45));

            return card;
        }

        /// <summary>
        /// Create loading spinner control
        /// </summary>
        public static Control CreateLoadingSpinner(int size = 50)
        {
            PictureBox spinner = new PictureBox();
            spinner.Size = new Size(size, size);
            spinner.SizeMode = PictureBoxSizeMode.Zoom;
            spinner.BackColor = Color.Transparent;

            // We'll set the actual spinner image later
            // For now, create a simple animated label
            Label loadingLabel = new Label();
            loadingLabel.Text = "Loading...";
            loadingLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            loadingLabel.ForeColor = GoldAccent;
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            loadingLabel.Dock = DockStyle.Fill;
            loadingLabel.BackColor = Color.Transparent;

            Panel container = new Panel();
            container.Size = new Size(size, size);
            container.BackColor = Color.Transparent;
            container.Controls.Add(loadingLabel);

            return container;
        }

        /// <summary>
        /// Create a stat card for dashboard
        /// </summary>
        public static Panel CreateStatCard(string title, string value, Color color, string icon, EventHandler clickHandler = null)
        {
            Panel card = new Panel();
            card.Size = new Size(220, 150);
            card.BackColor = Color.FromArgb(35, 35, 45);
            card.Cursor = Cursors.Hand;

            // Draw border
            card.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(color, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
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
            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTitle.ForeColor = TextSecondary;
            lblTitle.Size = new Size(180, 25);
            lblTitle.Location = new Point(20, 75);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Value
            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblValue.ForeColor = color;
            lblValue.Size = new Size(180, 40);
            lblValue.Location = new Point(20, 100);
            lblValue.TextAlign = ContentAlignment.MiddleLeft;

            // Add controls
            card.Controls.Add(lblIcon);
            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            // Add click handler
            if (clickHandler != null)
            {
                card.Click += clickHandler;
                lblIcon.Click += clickHandler;
                lblTitle.Click += clickHandler;
                lblValue.Click += clickHandler;
            }

            // Hover effect
            ApplyHoverEffect(card, Color.FromArgb(35, 35, 45), Color.FromArgb(45, 45, 55));

            return card;
        }

        /// <summary>
        /// Create an activity log item
        /// </summary>
        public static Panel CreateActivityItem(string icon, Color iconColor, string description, string amount, string date, EventHandler clickHandler = null)
        {
            Panel item = new Panel();
            item.Size = new Size(410, 70);
            item.BackColor = Color.FromArgb(40, 40, 50);
            item.Margin = new Padding(0, 0, 0, 10);
            item.Padding = new Padding(10);
            item.Cursor = Cursors.Hand;

            // Icon
            Label lblIcon = new Label();
            lblIcon.Text = icon;
            lblIcon.Font = new Font("Segoe UI", 14);
            lblIcon.ForeColor = iconColor;
            lblIcon.Size = new Size(40, 40);
            lblIcon.Location = new Point(10, 15);
            lblIcon.TextAlign = ContentAlignment.MiddleCenter;

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = description;
            lblDescription.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblDescription.ForeColor = TextPrimary;
            lblDescription.Size = new Size(250, 25);
            lblDescription.Location = new Point(60, 10);
            lblDescription.TextAlign = ContentAlignment.MiddleLeft;

            // Amount
            Label lblAmount = new Label();
            lblAmount.Text = amount;
            lblAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAmount.ForeColor = iconColor;
            lblAmount.Size = new Size(100, 25);
            lblAmount.Location = new Point(320, 10);
            lblAmount.TextAlign = ContentAlignment.MiddleRight;

            // Date
            Label lblDate = new Label();
            lblDate.Text = date;
            lblDate.Font = new Font("Segoe UI", 9);
            lblDate.ForeColor = TextSecondary;
            lblDate.Size = new Size(250, 20);
            lblDate.Location = new Point(60, 35);
            lblDate.TextAlign = ContentAlignment.MiddleLeft;

            // Add controls
            item.Controls.Add(lblIcon);
            item.Controls.Add(lblDescription);
            item.Controls.Add(lblAmount);
            item.Controls.Add(lblDate);

            // Add click handler
            if (clickHandler != null)
            {
                item.Click += clickHandler;
                foreach (Control control in item.Controls)
                {
                    control.Click += clickHandler;
                }
            }

            // Hover effect
            ApplyHoverEffect(item, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            return item;
        }

        /// <summary>
        /// Create a trending NFT card
        /// </summary>
        public static Panel CreateTrendingNFTCard(string title, decimal price, string creator, string category,
            int bidCount, int views, decimal? currentBid, EventHandler clickHandler)
        {
            int cardWidth = 200;
            int cardHeight = 250;

            Panel card = new Panel();
            card.Size = new Size(cardWidth, cardHeight);
            card.BackColor = Color.FromArgb(40, 40, 50);
            card.Margin = new Padding(0, 0, 10, 10);
            card.Padding = new Padding(10);
            card.Cursor = Cursors.Hand;

            // Title (truncate if too long)
            string displayTitle = title.Length > 20 ? title.Substring(0, 17) + "..." : title;

            Label lblTitle = new Label();
            lblTitle.Text = displayTitle;
            lblTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTitle.ForeColor = TextPrimary;
            lblTitle.Size = new Size(180, 25);
            lblTitle.Location = new Point(10, 10);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Category badge
            Label lblCategory = new Label();
            lblCategory.Text = category;
            lblCategory.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblCategory.ForeColor = BlueAccent;
            lblCategory.BackColor = Color.FromArgb(30, 30, 45);
            lblCategory.Size = new Size(60, 20);
            lblCategory.Location = new Point(10, 40);
            lblCategory.TextAlign = ContentAlignment.MiddleCenter;
            lblCategory.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, lblCategory.ClientRectangle,
                    BlueAccent, 1, ButtonBorderStyle.Solid,
                    BlueAccent, 1, ButtonBorderStyle.Solid,
                    BlueAccent, 1, ButtonBorderStyle.Solid,
                    BlueAccent, 1, ButtonBorderStyle.Solid);
            };

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = $"Price: {price:C2}";
            lblPrice.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblPrice.ForeColor = GoldAccent;
            lblPrice.Size = new Size(180, 25);
            lblPrice.Location = new Point(10, 150);
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;

            // Bid info
            Label lblBidInfo = new Label();
            if (currentBid.HasValue)
            {
                lblBidInfo.Text = $"Current: {currentBid.Value:C2}";
                lblBidInfo.ForeColor = BlueAccent;
            }
            else
            {
                lblBidInfo.Text = "No bids yet";
                lblBidInfo.ForeColor = TextSecondary;
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
            lblBids.ForeColor = TextSecondary;
            lblBids.Size = new Size(60, 20);
            lblBids.Location = new Point(0, 10);
            lblBids.TextAlign = ContentAlignment.MiddleLeft;

            // Views
            Label lblViews = new Label();
            lblViews.Text = $"👁️ {views}";
            lblViews.Font = new Font("Segoe UI", 9);
            lblViews.ForeColor = TextSecondary;
            lblViews.Size = new Size(60, 20);
            lblViews.Location = new Point(70, 10);
            lblViews.TextAlign = ContentAlignment.MiddleLeft;

            statsPanel.Controls.Add(lblBids);
            statsPanel.Controls.Add(lblViews);

            // Creator
            Label lblCreator = new Label();
            lblCreator.Text = $"By: {creator}";
            lblCreator.Font = new Font("Segoe UI", 8);
            lblCreator.ForeColor = TextSecondary;
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

            // Image placeholder
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
                using (Pen pen = new Pen(Color.FromArgb(100, GoldAccent), 1))
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
                using (Brush brush = new SolidBrush(BlueAccent))
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
            ApplyHoverEffect(card, Color.FromArgb(40, 40, 50), Color.FromArgb(50, 50, 60));

            // Scale effect on hover
            card.MouseEnter += (s, e) =>
            {
                card.Size = new Size(cardWidth + 2, cardHeight + 2);
                card.Location = new Point(card.Location.X - 1, card.Location.Y - 1);
            };

            card.MouseLeave += (s, e) =>
            {
                card.Size = new Size(cardWidth, cardHeight);
                card.Location = new Point(card.Location.X + 1, card.Location.Y + 1);
            };

            // Click event
            if (clickHandler != null)
            {
                card.Click += clickHandler;
                foreach (Control control in card.Controls)
                {
                    control.Click += clickHandler;
                }
            }

            return card;
        }

        /// <summary>
        /// Create a rounded panel
        /// </summary>
        public static Panel CreateRoundedPanel(int width, int height, Color backgroundColor, int borderRadius = 10)
        {
            Panel panel = new Panel();
            panel.Size = new Size(width, height);
            panel.BackColor = backgroundColor;
            panel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, width, height, borderRadius, borderRadius));
            return panel;
        }

        /// <summary>
        /// Create a search textbox with icon
        /// </summary>
        public static Panel CreateSearchBox(string placeholder, EventHandler textChangedHandler)
        {
            Panel searchPanel = new Panel();
            searchPanel.Size = new Size(400, 40);
            searchPanel.BackColor = Color.FromArgb(50, 50, 60);

            // Search icon
            Label iconLabel = new Label();
            iconLabel.Text = "🔍";
            iconLabel.Font = new Font("Segoe UI", 14);
            iconLabel.ForeColor = BlueAccent;
            iconLabel.Size = new Size(40, 40);
            iconLabel.Location = new Point(5, 0);
            iconLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Textbox
            TextBox textBox = new TextBox();
            textBox.Size = new Size(350, 35);
            textBox.Location = new Point(45, 2);
            textBox.Font = new Font("Segoe UI", 11);
            textBox.BackColor = Color.FromArgb(50, 50, 60);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Text = placeholder;

            if (textChangedHandler != null)
                textBox.TextChanged += textChangedHandler;

            // Bottom border
            Panel borderPanel = new Panel();
            borderPanel.Size = new Size(350, 1);
            borderPanel.Location = new Point(45, 38);
            borderPanel.BackColor = BlueAccent;

            searchPanel.Controls.Add(iconLabel);
            searchPanel.Controls.Add(textBox);
            searchPanel.Controls.Add(borderPanel);

            return searchPanel;
        }

        /// <summary>
        /// Create a form field with label
        /// </summary>
        public static Panel CreateFormField(string labelText, Control inputControl, bool isRequired = false)
        {
            Panel fieldPanel = new Panel();
            fieldPanel.Size = new Size(400, 80);
            fieldPanel.BackColor = Color.Transparent;

            // Label
            Label label = new Label();
            label.Text = labelText + (isRequired ? " *" : "");
            label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            label.ForeColor = isRequired ? GoldAccent : TextPrimary;
            label.Size = new Size(380, 25);
            label.Location = new Point(10, 0);
            label.TextAlign = ContentAlignment.MiddleLeft;

            // Input control
            inputControl.Location = new Point(10, 30);
            inputControl.Size = new Size(380, 35);

            fieldPanel.Controls.Add(label);
            fieldPanel.Controls.Add(inputControl);

            return fieldPanel;
        }

        /// <summary>
        /// Create a text area field
        /// </summary>
        public static Panel CreateTextAreaField(string labelText, TextBox textBox, bool isRequired = false)
        {
            Panel fieldPanel = new Panel();
            fieldPanel.Size = new Size(400, 150);
            fieldPanel.BackColor = Color.Transparent;

            // Label
            Label label = new Label();
            label.Text = labelText + (isRequired ? " *" : "");
            label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            label.ForeColor = isRequired ? GoldAccent : TextPrimary;
            label.Size = new Size(380, 25);
            label.Location = new Point(10, 0);
            label.TextAlign = ContentAlignment.MiddleLeft;

            // TextBox
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Location = new Point(10, 30);
            textBox.Size = new Size(380, 110);
            textBox.Font = new Font("Segoe UI", 10);
            textBox.BackColor = Color.FromArgb(50, 50, 60);
            textBox.ForeColor = Color.White;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            fieldPanel.Controls.Add(label);
            fieldPanel.Controls.Add(textBox);

            return fieldPanel;
        }

        /// <summary>
        /// Create a file upload control
        /// </summary>
        public static Panel CreateFileUploadField(string labelText, out Button uploadButton, out Label fileNameLabel,
            EventHandler uploadClickHandler, bool isRequired = false)
        {
            Panel fieldPanel = new Panel();
            fieldPanel.Size = new Size(400, 80);
            fieldPanel.BackColor = Color.Transparent;

            // Label
            Label label = new Label();
            label.Text = labelText + (isRequired ? " *" : "");
            label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            label.ForeColor = isRequired ? GoldAccent : TextPrimary;
            label.Size = new Size(380, 25);
            label.Location = new Point(10, 0);
            label.TextAlign = ContentAlignment.MiddleLeft;

            // Upload button
            uploadButton = new Button();
            uploadButton.Text = "📁 SELECT FILE";
            uploadButton.Size = new Size(150, 35);
            uploadButton.Location = new Point(10, 30);
            StyleButton(uploadButton, false);
            uploadButton.Click += uploadClickHandler;

            // File name label
            fileNameLabel = new Label();
            fileNameLabel.Text = "No file selected";
            fileNameLabel.Font = new Font("Segoe UI", 9);
            fileNameLabel.ForeColor = TextSecondary;
            fileNameLabel.Size = new Size(220, 25);
            fileNameLabel.Location = new Point(170, 35);
            fileNameLabel.TextAlign = ContentAlignment.MiddleLeft;

            fieldPanel.Controls.Add(label);
            fieldPanel.Controls.Add(uploadButton);
            fieldPanel.Controls.Add(fileNameLabel);

            return fieldPanel;
        }
    }
}