using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TriApex.Helpers
{
    public static class DBHelper
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["TriApexConnection"].ConnectionString;

        // Helper to add parameter clones to a command
        private static void AddParametersCloned(SqlCommand cmd, SqlParameter[] parameters)
        {
            if (parameters == null) return;
            foreach (var p in parameters)
            {
                // Create a new parameter with the same name and value. You can extend to copy DbType, Size, Direction if needed.
                var clone = new SqlParameter(p.ParameterName, p.Value ?? DBNull.Value)
                {
                    Direction = p.Direction,
                    Size = p.Size,
                    DbType = p.DbType,
                    Precision = p.Precision,
                    Scale = p.Scale,
                    IsNullable = p.IsNullable
                };
                cmd.Parameters.Add(clone);
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        AddParametersCloned(cmd, parameters);
                    }

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Query execution failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            object result = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        AddParametersCloned(cmd, parameters);
                    }

                    conn.Open();
                    result = cmd.ExecuteScalar();
                    // Normalize DB NULL to null to avoid downstream Convert.*(DBNull.Value) exceptions
                    if (result == DBNull.Value) result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scalar execution failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                    {
                        AddParametersCloned(cmd, parameters);
                    }

                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Non-query execution failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            return rowsAffected;
        }

        public static DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        AddParametersCloned(cmd, parameters);
                    }

                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stored procedure failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        public static int ExecuteStoredProcedureWithOutput(string procedureName, SqlParameter[] parameters, string outputParamName)
        {
            int result = -1;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        AddParametersCloned(cmd, parameters);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    object val = null;
                    if (cmd.Parameters.Contains(outputParamName))
                        val = cmd.Parameters[outputParamName].Value;

                    if (val == null || val == DBNull.Value)
                    {
                        result = -1; // stored procedure returned NULL / did not set output
                    }
                    else
                    {
                        // handle numeric types safely
                        if (val is int) result = (int)val;
                        else result = Convert.ToInt32(val);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Stored procedure with output failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        /// <summary>
        /// Check if a user exists with given username
        /// </summary>
        public static bool UserExists(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username)
            };

            object scalar = ExecuteScalar(query, parameters);
            int count = 0;
            if (scalar != null)
            {
                try
                {
                    count = Convert.ToInt32(scalar);
                }
                catch
                {
                    count = 0;
                }
            }

            return count > 0;
        }

        /// <summary>
        /// Get user balance by ID
        /// </summary>
        public static decimal GetUserBalance(int userId)
        {
            string query = "SELECT Balance FROM Users WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserID", userId)
            };

            object result = ExecuteScalar(query, parameters);
            return result != null ? Convert.ToDecimal(result) : 0;
        }

        /// <summary>
        /// Update user balance
        /// </summary>
        public static bool UpdateUserBalance(int userId, decimal newBalance)
        {
            string query = "UPDATE Users SET Balance = @Balance WHERE UserID = @UserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Balance", newBalance),
                new SqlParameter("@UserID", userId)
            };

            return ExecuteNonQuery(query, parameters) > 0;
        }
    }
}