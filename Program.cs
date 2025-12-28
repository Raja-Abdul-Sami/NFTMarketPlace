using System;
using System.Windows.Forms;
using TriApex.Forms;

namespace TriApex
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

                // Show splash/loading then login
                ShowSplashThenLogin();   
        }

        /// <summary>
        /// Show splash screen then login form
        /// </summary>
        static void ShowSplashThenLogin()
        {
            // Create and show splash form
            SplashForm splash = new SplashForm();
            splash.Show();

            // Force splash to display
            Application.DoEvents();

            // Keep splash visible for minimum time
            System.Threading.Thread.Sleep(2000);

            // Close splash and show login
            splash.Close();

            // Run application with login form
            Application.Run(new LoginForm());
        }
    }
}