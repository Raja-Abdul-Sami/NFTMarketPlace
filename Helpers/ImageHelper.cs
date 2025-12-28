using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace TriApex.Helpers
{
    /// <summary>
    /// Helper class for image processing and management
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Resize image to fit within specified dimensions while maintaining aspect ratio
        /// </summary>
        public static Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            if (image == null) return null;

            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        /// <summary>
        /// Convert image to byte array
        /// </summary>
        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convert byte array to image
        /// </summary>
        public static Image ByteArrayToImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Create a placeholder NFT image with title
        /// </summary>
        public static Image CreatePlaceholderNFTImage(string title, int width, int height)
        {
            Bitmap image = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(image))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Create gradient background
                Rectangle rect = new Rectangle(0, 0, width, height);
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(60, 60, 75),
                    Color.FromArgb(50, 50, 65),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, rect);
                }

                // Draw abstract pattern
                Random rand = new Random(title.GetHashCode());
                using (Pen goldPen = new Pen(Color.FromArgb(100, UIHelper.GoldAccent), 2))
                using (Pen bluePen = new Pen(Color.FromArgb(100, UIHelper.BlueAccent), 2))
                {
                    // Draw random shapes
                    for (int i = 0; i < 15; i++)
                    {
                        int x1 = rand.Next(width);
                        int y1 = rand.Next(height);
                        int x2 = rand.Next(width);
                        int y2 = rand.Next(height);
                        int size = rand.Next(20, 60);

                        if (rand.Next(2) == 0)
                        {
                            // Draw circle
                            g.DrawEllipse(goldPen, x1, y1, size, size);
                        }
                        else
                        {
                            // Draw line
                            g.DrawLine(bluePen, x1, y1, x2, y2);
                        }
                    }
                }

                // Draw title in center
                using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    string displayText = title.Length > 20 ? title.Substring(0, 17) + "..." : title;
                    g.DrawString(displayText, font, textBrush, rect, format);
                }

                // Draw TriApex watermark
                using (Font watermarkFont = new Font("Segoe UI", 8))
                using (Brush watermarkBrush = new SolidBrush(Color.FromArgb(100, 100, 120)))
                {
                    g.DrawString("TriApex NFT", watermarkFont, watermarkBrush, 10, height - 20);
                }
            }

            return image;
        }

        /// <summary>
        /// Validate image file
        /// </summary>
        public static bool ValidateImageFile(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    errorMessage = "File does not exist.";
                    return false;
                }

                // Check file size (max 5MB)
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 5 * 1024 * 1024) // 5MB
                {
                    errorMessage = "File size must be less than 5MB.";
                    return false;
                }

                // Check file extension
                string extension = Path.GetExtension(filePath).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (Array.IndexOf(allowedExtensions, extension) == -1)
                {
                    errorMessage = "Only JPG, PNG, and GIF files are allowed.";
                    return false;
                }

                // Try to load the image to verify it's valid
                using (Image testImage = Image.FromFile(filePath))
                {
                    // Image loaded successfully
                    testImage.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Invalid image file: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Open file dialog to select an image
        /// </summary>
        public static string SelectImageFile(Form parentForm)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";
                openFileDialog.Title = "Select NFT Image";
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog(parentForm) == DialogResult.OK)
                {
                    if (ValidateImageFile(openFileDialog.FileName, out string errorMessage))
                    {
                        return openFileDialog.FileName;
                    }
                    else
                    {
                        MessageBox.Show(errorMessage, "Invalid Image",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Save image to application data folder
        /// </summary>
        public static string SaveImageToAppData(Image image, string fileName)
        {
            try
            {
                // Create application data folder if it doesn't exist
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TriApex",
                    "NFT_Images");

                Directory.CreateDirectory(appDataPath);

                // Generate unique filename
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
                string fullPath = Path.Combine(appDataPath, uniqueFileName);

                // Save image
                image.Save(fullPath, ImageFormat.Png);

                return fullPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Load image from application data folder
        /// </summary>
        public static Image LoadImageFromAppData(string relativePath)
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TriApex",
                    "NFT_Images");

                string fullPath = Path.Combine(appDataPath, Path.GetFileName(relativePath));

                if (File.Exists(fullPath))
                {
                    return Image.FromFile(fullPath);
                }
                else
                {
                    // Return placeholder if image not found
                    return CreatePlaceholderNFTImage("NFT Image", 400, 300);
                }
            }
            catch
            {
                // Return placeholder on error
                return CreatePlaceholderNFTImage("NFT Image", 400, 300);
            }
        }
    }
}