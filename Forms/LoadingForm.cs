using System;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class LoadingForm : Form
    {
        // Animation components
        private Timer animationTimer;
        private Timer fadeTimer;
        private PictureBox logoPictureBox;
        private Label loadingLabel;
        private Panel spinnerPanel;
        private ProgressBar progressBar;
        private Label statusLabel;

        // Animation variables
        private int animationStep = 0;
        private double fadeOpacity = 0.0;
        private const int TOTAL_STEPS = 100;
        private string[] loadingMessages = {
            "Initializing marketplace...",
            "Loading NFT collections...",
            "Connecting to blockchain...",
            "Preparing your dashboard...",
            "Almost ready..."
        };

        public LoadingForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            StartAnimations();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // LoadingForm
            this.ClientSize = new Size(800, 500);
            this.Text = "TriApex - Loading";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;
            this.DoubleBuffered = true;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Main container panel
            Panel mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Paint += MainPanel_Paint;

            // Logo
            logoPictureBox = new PictureBox();
            logoPictureBox.Size = new Size(200, 200);
            logoPictureBox.Location = new Point(300, 80);
            logoPictureBox.BackColor = Color.Transparent;
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            SetLogoImage();

            // Loading label
            loadingLabel = new Label();
            loadingLabel.Text = "TriApex NFT Marketplace";
            loadingLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            loadingLabel.ForeColor = Color.White;
            loadingLabel.Size = new Size(400, 40);
            loadingLabel.Location = new Point(200, 280);
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            loadingLabel.BackColor = Color.Transparent;

            // Status label
            statusLabel = new Label();
            statusLabel.Text = loadingMessages[0];
            statusLabel.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            statusLabel.ForeColor = UIHelper.BlueAccent;
            statusLabel.Size = new Size(400, 30);
            statusLabel.Location = new Point(200, 320);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.BackColor = Color.Transparent;

            // Progress bar
            progressBar = new ProgressBar();
            progressBar.Size = new Size(400, 20);
            progressBar.Location = new Point(200, 360);
            progressBar.Minimum = 0;
            progressBar.Maximum = TOTAL_STEPS;
            progressBar.Value = 0;
            progressBar.ForeColor = UIHelper.GoldAccent;
            progressBar.BackColor = Color.FromArgb(40, 40, 50);

            // Customize progress bar
            progressBar.Style = ProgressBarStyle.Continuous;

            // Spinner panel
            spinnerPanel = new Panel();
            spinnerPanel.Size = new Size(50, 50);
            spinnerPanel.Location = new Point(375, 390);
            spinnerPanel.BackColor = Color.Transparent;
            spinnerPanel.Paint += SpinnerPanel_Paint;

            // Add controls to main panel
            mainPanel.Controls.Add(logoPictureBox);
            mainPanel.Controls.Add(loadingLabel);
            mainPanel.Controls.Add(statusLabel);
            mainPanel.Controls.Add(progressBar);
            mainPanel.Controls.Add(spinnerPanel);

            // Add main panel to form
            this.Controls.Add(mainPanel);

            // Set initial opacity for fade-in
            this.Opacity = 0;
        }

        private void SetLogoImage()
        {
            try
            {
                if (Properties.Resources.TriApexLogo != null)
                {
                    logoPictureBox.Image = Properties.Resources.TriApexLogo;
                }
                else
                {
                    CreateAnimatedLogo();
                }
            }
            catch
            {
                CreateAnimatedLogo();
            }
        }

        private void CreateAnimatedLogo()
        {
            Bitmap logo = new Bitmap(200, 200);
            using (Graphics g = Graphics.FromImage(logo))
            {
                g.Clear(Color.Transparent);

                // Draw animated circle
                int circleSize = 150;
                int circleX = (200 - circleSize) / 2;
                int circleY = (200 - circleSize) / 2;

                using (Pen pen = new Pen(UIHelper.GoldAccent, 4))
                {
                    g.DrawEllipse(pen, circleX, circleY, circleSize, circleSize);
                }

                // Draw TriApex text
                Rectangle rect = new Rectangle(0, 0, 200, 200);
                using (var brush = UIHelper.CreateGradientBrush(rect, UIHelper.GoldAccent, UIHelper.BlueAccent))
                {
                    Font font = new Font("Segoe UI", 20, FontStyle.Bold);
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString("TriApex", font, brush, rect, format);
                }
            }

            logoPictureBox.Image = logo;
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw gradient background with pattern
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(15, 15, 20),
                Color.FromArgb(25, 25, 35)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw geometric pattern
            DrawGeometricPattern(e.Graphics);
        }

        private void DrawGeometricPattern(Graphics g)
        {
            using (Pen pen = new Pen(Color.FromArgb(30, UIHelper.BlueAccent), 1))
            {
                int step = 40;
                for (int x = 0; x < this.Width; x += step)
                {
                    g.DrawLine(pen, x, 0, x, this.Height);
                }
                for (int y = 0; y < this.Height; y += step)
                {
                    g.DrawLine(pen, 0, y, this.Width, y);
                }
            }

            // Draw some circles
            using (Pen goldPen = new Pen(Color.FromArgb(20, UIHelper.GoldAccent), 2))
            using (Pen bluePen = new Pen(Color.FromArgb(20, UIHelper.BlueAccent), 2))
            {
                Random rand = new Random();
                for (int i = 0; i < 20; i++)
                {
                    int x = rand.Next(this.Width);
                    int y = rand.Next(this.Height);
                    int size = rand.Next(20, 80);

                    if (rand.Next(2) == 0)
                        g.DrawEllipse(goldPen, x, y, size, size);
                    else
                        g.DrawEllipse(bluePen, x, y, size, size);
                }
            }
        }

        private void SpinnerPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int centerX = panel.Width / 2;
            int centerY = panel.Height / 2;
            int radius = Math.Min(panel.Width, panel.Height) / 2 - 5;

            // Draw spinner arc
            float startAngle = animationStep * 3.6f;
            float sweepAngle = 120;

            using (Pen pen = new Pen(UIHelper.GoldAccent, 4))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                e.Graphics.DrawArc(pen,
                    centerX - radius, centerY - radius,
                    radius * 2, radius * 2,
                    startAngle, sweepAngle);
            }

            // Draw inner spinner
            int innerRadius = radius - 10;
            float innerStartAngle = -animationStep * 3.6f;

            using (Pen pen = new Pen(UIHelper.BlueAccent, 3))
            {
                pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

                e.Graphics.DrawArc(pen,
                    centerX - innerRadius, centerY - innerRadius,
                    innerRadius * 2, innerRadius * 2,
                    innerStartAngle, 90);
            }
        }

        private void StartAnimations()
        {
            // Fade-in animation
            fadeTimer = new Timer();
            fadeTimer.Interval = 20;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            // Main animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 50;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadeOpacity < 1.0)
            {
                fadeOpacity += 0.05;
                this.Opacity = fadeOpacity;
            }
            else
            {
                fadeTimer.Stop();
                this.Opacity = 1.0;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Update spinner animation
            animationStep = (animationStep + 5) % 100;
            spinnerPanel.Invalidate();

            // Update progress bar
            if (progressBar.Value < TOTAL_STEPS)
            {
                progressBar.Value += 1;

                // Update status message based on progress
                int messageIndex = (progressBar.Value * loadingMessages.Length) / TOTAL_STEPS;
                if (messageIndex < loadingMessages.Length)
                {
                    statusLabel.Text = loadingMessages[messageIndex];
                }
            }

            // Add pulsing effect to logo
            if (logoPictureBox.Image != null)
            {
                float scale = 1.0f + 0.05f * (float)Math.Sin(animationStep * 0.1);
                logoPictureBox.Size = new Size((int)(200 * scale), (int)(200 * scale));
                logoPictureBox.Left = (this.Width - logoPictureBox.Width) / 2;
                logoPictureBox.Top = 80 - (int)((scale - 1.0f) * 100);
            }
        }

        /// <summary>
        /// Public method to update loading status
        /// </summary>
        public void UpdateStatus(string status)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action<string>(UpdateStatus), status);
            }
            else
            {
                statusLabel.Text = status;
            }
        }

        /// <summary>
        /// Public method to update progress
        /// </summary>
        public void UpdateProgress(int progress)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action<int>(UpdateProgress), progress);
            }
            else
            {
                progressBar.Value = Math.Min(progress, TOTAL_STEPS);
            }
        }

        /// <summary>
        /// Complete loading with fade out
        /// </summary>
        public void CompleteLoading()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(CompleteLoading));
                return;
            }

            // Set to 100%
            progressBar.Value = TOTAL_STEPS;
            statusLabel.Text = "Ready!";

            // Wait a moment then fade out
            Timer completeTimer = new Timer();
            completeTimer.Interval = 500;
            completeTimer.Tick += (s, e) =>
            {
                completeTimer.Stop();
                FadeOutAndClose();
            };
            completeTimer.Start();
        }

        private void FadeOutAndClose()
        {
            Timer fadeOutTimer = new Timer();
            fadeOutTimer.Interval = 20;
            double opacity = 1.0;

            fadeOutTimer.Tick += (s, e) =>
            {
                if (opacity > 0)
                {
                    opacity -= 0.05;
                    this.Opacity = opacity;
                }
                else
                {
                    fadeOutTimer.Stop();
                    animationTimer.Stop();
                    this.Close();
                }
            };

            fadeOutTimer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Clean up timers
            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            }

            if (fadeTimer != null)
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();
            }

            base.OnFormClosing(e);
        }
    }
}