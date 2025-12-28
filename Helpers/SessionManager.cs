using System;

namespace TriApex.Helpers
{
    /// <summary>
    /// Manages user session throughout the application
    /// Singleton pattern for global access
    /// </summary>
    public sealed class SessionManager
    {
        // Singleton instance
        private static readonly Lazy<SessionManager> instance =
            new Lazy<SessionManager>(() => new SessionManager());

        public static SessionManager Instance => instance.Value;

        // Private constructor
        private SessionManager() { }

        // Session properties
        public int CurrentUserID { get; private set; }
        public string Username { get; private set; }
        public decimal Balance { get; private set; }
        public string Email { get; private set; }
        public bool IsLoggedIn { get; private set; }

        /// <summary>
        /// Initialize user session after successful login
        /// </summary>
        public void InitializeSession(int userId, string username, decimal balance, string email)
        {
            CurrentUserID = userId;
            Username = username;
            Balance = balance;
            Email = email;
            IsLoggedIn = true;
        }

        /// <summary>
        /// Update user balance in session
        /// </summary>
        public void UpdateBalance(decimal newBalance)
        {
            Balance = newBalance;
        }

        /// <summary>
        /// Clear session on logout
        /// </summary>
        public void ClearSession()
        {
            CurrentUserID = 0;
            Username = string.Empty;
            Balance = 0;
            Email = string.Empty;
            IsLoggedIn = false;
        }

        /// <summary>
        /// Refresh balance from database
        /// </summary>
        public void RefreshBalance()
        {
            if (IsLoggedIn)
            {
                Balance = DBHelper.GetUserBalance(CurrentUserID);
            }
        }

        /// <summary>
        /// Check if user has sufficient balance for transaction
        /// </summary>
        public bool HasSufficientBalance(decimal amount)
        {
            return IsLoggedIn && Balance >= amount;
        }

        /// <summary>
        /// Format balance for display
        /// </summary>
        public string GetFormattedBalance()
        {
            return Balance.ToString("C2");
        }
    }
}