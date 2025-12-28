using System;
using System.Drawing;
using System.Windows.Forms;
using TriApex.Helpers;

namespace TriApex.Forms
{
    public partial class SplashForm : Form
    {
        private Timer fadeTimer;
        private Timer closeTimer;
        private Timer fadeOutTimer;

        private double fadeOpacity = 0.0;
        private double fadeOutOpacity = 1.0;
        private const double FADE_INCREMENT = 0.08;

        public SplashForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // SplashForm
            this.ClientSize = new Size(600, 400);
            this.Text = "TriApex";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UIHelper.DarkBackground;
            this.DoubleBuffered = true;

            this.ResumeLayout(false);
        }

        private void InitializeCustomComponents()
        {
            // Set form to be topmost and borderless
            this.TopMost = true;
            this.ShowInTaskbar = false;

            // Initialize fade-in animation
            this.Opacity = 0;
            fadeOpacity = 0.0;
            fadeTimer = new Timer();
            fadeTimer.Interval = 30;
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            // Close after 3 seconds (use field so we can stop/dispose reliably)
            closeTimer = new Timer();
            closeTimer.Interval = 3000;
            closeTimer.Tick += CloseTimer_Tick;
            closeTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw gradient background
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            using (var brush = UIHelper.CreateGradientBrush(rect,
                Color.FromArgb(10, 10, 15),
                Color.FromArgb(25, 25, 35)))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Draw logo
            DrawLogo(e.Graphics);

            // Draw loading text
            DrawLoadingText(e.Graphics);

            // Draw decorative elements
            DrawDecorativeElements(e.Graphics);
        }

        private void DrawLogo(Graphics g)
        {
            string text = "TriApex";
            Font font = new Font("Segoe UI", 48, FontStyle.Bold);
            SizeF textSize = g.MeasureString(text, font);

            // Position in center
            float x = (this.Width - textSize.Width) / 2;
            float y = (this.Height - textSize.Height) / 2 - 40;

            // Draw text with gradient
            RectangleF rect = new RectangleF(x, y, textSize.Width, textSize.Height);
            using (var brush = UIHelper.CreateGradientBrush(
                Rectangle.Round(rect),
                UIHelper.GoldAccent,
                UIHelper.BlueAccent))
            {
                g.DrawString(text, font, brush, x, y);
            }

            // Draw subtitle
            string subtitle = "NFT Marketplace";
            Font subtitleFont = new Font("Segoe UI", 16, FontStyle.Regular);
            SizeF subtitleSize = g.MeasureString(subtitle, subtitleFont);

            float subtitleX = (this.Width - subtitleSize.Width) / 2;
            float subtitleY = y + textSize.Height + 10;

            g.DrawString(subtitle, subtitleFont, Brushes.White, subtitleX, subtitleY);
        }
        private void DrawLoadingText(Graphics g)
        {
            string loadingText = "Loading...";
            Font font = new Font("Segoe UI", 12, FontStyle.Regular);
            SizeF textSize = g.MeasureString(loadingText, font);

            float x = (this.Width - textSize.Width) / 2;
            float y = this.Height - 80;

            g.DrawString(loadingText, font, Brushes.Gray, x, y);

            // Draw animated dots
            int dotCount = (int)(DateTime.Now.Millisecond / 250) % 4;
            string dots = new string('.', dotCount);
            using (Brush goldBrush = new SolidBrush(UIHelper.GoldAccent))
            {
                g.DrawString(dots, font, goldBrush, x + textSize.Width, y);
            }
        }

        private void DrawDecorativeElements(Graphics g)
        {
            // Draw some abstract shapes
            using (Pen goldPen = new Pen(Color.FromArgb(100, UIHelper.GoldAccent.R,
                UIHelper.GoldAccent.G, UIHelper.GoldAccent.B), 3))
            using (Pen bluePen = new Pen(Color.FromArgb(100, UIHelper.BlueAccent.R,
                UIHelper.BlueAccent.G, UIHelper.BlueAccent.B), 3))
            {
                // Draw circles
                for (int i = 0; i < 8; i++)
                {
                    float angle = (float)(i * Math.PI / 4);
                    int radius = 150;
                    int centerX = this.Width / 2;
                    int centerY = this.Height / 2;

                    int x = (int)(centerX + Math.Cos(angle) * radius);
                    int y = (int)(centerY + Math.Sin(angle) * radius);
                    int size = 20 + i * 5;

                    if (i % 2 == 0)
                        g.DrawEllipse(goldPen, x - size / 2, y - size / 2, size, size);
                    else
                        g.DrawEllipse(bluePen, x - size / 2, y - size / 2, size, size);
                }
            }
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            // Protect against handling after disposal
            if (this.IsDisposed || this.Disposing)
            {
                if (fadeTimer != null)
                {
                    fadeTimer.Stop();
                }
                return;
            }

            if (fadeOpacity < 1.0)
            {
                fadeOpacity += FADE_INCREMENT;
                // Clamp
                if (fadeOpacity > 1.0) fadeOpacity = 1.0;
                this.Opacity = fadeOpacity;
            }
            else
            {
                fadeTimer.Stop();
                this.Opacity = 1.0;
            }
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            if (closeTimer != null)
            {
                closeTimer.Stop();
            }
            FadeOutAndClose();
        }

        private void FadeOutAndClose()
        {
            // Ensure any previous fadeOutTimer is stopped
            if (fadeOutTimer != null)
            {
                fadeOutTimer.Stop();
                fadeOutTimer.Tick -= FadeOutTimer_Tick;
                fadeOutTimer.Dispose();
                fadeOutTimer = null;
            }

            fadeOutOpacity = this.Opacity;
            if (double.IsNaN(fadeOutOpacity) || fadeOutOpacity <= 0) fadeOutOpacity = 1.0;

            fadeOutTimer = new Timer();
            fadeOutTimer.Interval = 30;
            fadeOutTimer.Tick += FadeOutTimer_Tick;
            fadeOutTimer.Start();
        }

        private void FadeOutTimer_Tick(object sender, EventArgs e)
        {
            // Protect against handling after disposal
            if (this.IsDisposed || this.Disposing)
            {
                if (fadeOutTimer != null)
                {
                    fadeOutTimer.Stop();
                }
                return;
            }

            if (fadeOutOpacity > 0)
            {
                fadeOutOpacity -= FADE_INCREMENT;
                if (fadeOutOpacity < 0) fadeOutOpacity = 0;
                this.Opacity = fadeOutOpacity;
            }
            else
            {
                if (fadeOutTimer != null)
                {
                    fadeOutTimer.Stop();
                }
                // Close will trigger OnFormClosing which disposes timers
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop and dispose timers so no Tick runs after dispose
            if (fadeTimer != null)
            {
                fadeTimer.Stop();
                fadeTimer.Tick -= FadeTimer_Tick;
                fadeTimer.Dispose();
                fadeTimer = null;
            }

            if (closeTimer != null)
            {
                closeTimer.Stop();
                closeTimer.Tick -= CloseTimer_Tick;
                closeTimer.Dispose();
                closeTimer = null;
            }

            if (fadeOutTimer != null)
            {
                fadeOutTimer.Stop();
                fadeOutTimer.Tick -= FadeOutTimer_Tick;
                fadeOutTimer.Dispose();
                fadeOutTimer = null;
            }

            base.OnFormClosing(e);
        }
    }
}