using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TriApex.Helpers
{
    /// <summary>
    /// Handles navigation between forms and user controls
    /// Manages loading screen transitions
    /// </summary>
    public static class NavigationHelper
    {
        // Reference to main form for navigation
        private static Form mainForm;
        private static UserControl currentControl;

        /// <summary>
        /// Initialize navigation helper with main form
        /// </summary>
        public static void Initialize(Form mainFormInstance)
        {
            mainForm = mainFormInstance;
        }

        /// <summary>
        /// Switch to a different user control
        /// </summary>
        public static void SwitchUserControl(UserControl newControl, Panel containerPanel)
        {
            if (containerPanel == null)
                throw new ArgumentNullException(nameof(containerPanel));

            // Clear current control
            containerPanel.Controls.Clear();

            // Set new control
            newControl.Dock = DockStyle.Fill;
            containerPanel.Controls.Add(newControl);
            currentControl = newControl;
        }

        /// <summary>
        /// Show loading screen and then navigate
        /// </summary>
        public static async void ShowLoadingThenNavigate(Action navigationAction, int minDisplayTime = 1500)
        {
            // Create and show loading form
            //var loadingForm = new Forms.LoadingForm();

            //// Show loading form
            //loadingForm.Show();
            //loadingForm.BringToFront();

            // Store start time
            DateTime startTime = DateTime.Now;

            try
            {
                // Execute navigation action
                await Task.Run(() =>
                {
                    navigationAction?.Invoke();
                });
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Navigation failed: {ex.Message}");
            }
            finally
            {
                // Ensure minimum display time
                TimeSpan elapsed = DateTime.Now - startTime;
                int remainingTime = minDisplayTime - (int)elapsed.TotalMilliseconds;

                if (remainingTime > 0)
                {
                    await Task.Delay(remainingTime);
                }

                // Close loading form
                //loadingForm.Close();
            }
        }

        /// <summary>
        /// Show a form as dialog with loading screen
        /// </summary>
        public static DialogResult ShowDialogWithLoading(Form formToShow)
        {
            ShowLoadingThenNavigate(() =>
            {
                // Simulate loading time
                System.Threading.Thread.Sleep(1000);
            });

            return formToShow.ShowDialog();
        }

        /// <summary>
        /// Navigate back to previous control (if available)
        /// </summary>
        public static void NavigateBack(Panel containerPanel, UserControl previousControl)
        {
            if (previousControl != null && containerPanel != null)
            {
                SwitchUserControl(previousControl, containerPanel);
            }
        }

        /// <summary>
        /// Open a form centered on parent
        /// </summary>
        public static void OpenCenteredForm(Form childForm, Form parentForm)
        {
            childForm.StartPosition = FormStartPosition.CenterParent;
            childForm.ShowDialog(parentForm);
        }

        /// <summary>
        /// Get current active control
        /// </summary>
        public static UserControl GetCurrentControl()
        {
            return currentControl;
        }

        /// <summary>
        /// Check if main form is initialized
        /// </summary>
        public static bool IsInitialized()
        {
            return mainForm != null;
        }


    }
}