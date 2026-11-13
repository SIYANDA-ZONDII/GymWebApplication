
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace GymWebApplication.Account
{
    public partial class ResetPassword : Page
    {
        private const string InvalidCodeMessage =
            "The reset code is invalid or has expired. Request a new code and try again.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ErrorMessage.Visible = false;
        }

        protected void Reset_Click(object sender, EventArgs e)
        {
            ErrorMessage.Visible = false;

            if (!Page.IsValid)
                return;

            string input = (StudentNumberOrEmail.Text ?? "").Trim();
            string code = (ResetCode.Text ?? "").Trim();
            string newPassword = Password.Text ?? "";
            string confirmPassword = ConfirmPassword.Text ?? "";

            if (input.Length == 0)
            {
                ShowError("Enter your student number, staff ID, or registered email address.");
                return;
            }

            if (code.Length != 6 || !IsSixDigitCode(code))
            {
                ShowError("The reset code must contain exactly six digits.");
                return;
            }

            if (newPassword.Length < 8)
            {
                ShowError("Your new password must contain at least 8 characters.");
                return;
            }

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ShowError("The passwords do not match.");
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string codeHash = HashCode(code);

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    using (SqlTransaction transaction =
                        connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            string userId;
                            string email;

                            const string accountSql = @"
                                SELECT TOP (1) AccountID, AccountEmail
                                FROM
                                (
                                    SELECT Member_ID AS AccountID,
                                           Member_Email AS AccountEmail,
                                           1 AS AccountPriority
                                    FROM dbo.Member
                                    WHERE Member_ID = @Input
                                       OR LOWER(Member_Email) = LOWER(@Input)

                                    UNION ALL

                                    SELECT Staff_ID AS AccountID,
                                           Staff_Email AS AccountEmail,
                                           2 AS AccountPriority
                                    FROM dbo.Staff
                                    WHERE Staff_ID = @Input
                                       OR LOWER(Staff_Email) = LOWER(@Input)
                                ) AS Accounts
                                ORDER BY AccountPriority;";

                            using (SqlCommand command =
                                new SqlCommand(accountSql, connection, transaction))
                            {
                                command.Parameters.Add("@Input", SqlDbType.VarChar, 100).Value = input;

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (!reader.Read())
                                    {
                                        reader.Close();
                                        transaction.Rollback();
                                        ShowError(InvalidCodeMessage);
                                        return;
                                    }

                                    userId = reader["AccountID"].ToString().Trim();
                                    email = reader["AccountEmail"].ToString().Trim();
                                }
                            }

                            int resetId;

                            const string resetSql = @"
                                SELECT TOP (1) Reset_ID
                                FROM dbo.PasswordReset WITH (UPDLOCK, ROWLOCK)
                                WHERE User_ID = @UserID
                                  AND Reset_Code = @ResetCode
                                  AND Is_Used = 0
                                  AND Expiry_Date > GETDATE()
                                ORDER BY Created_Date DESC;";

                            using (SqlCommand command =
                                new SqlCommand(resetSql, connection, transaction))
                            {
                                command.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = userId;
                                command.Parameters.Add("@ResetCode", SqlDbType.VarChar, 64).Value = codeHash;

                                object result = command.ExecuteScalar();

                                if (result == null || result == DBNull.Value)
                                {
                                    transaction.Rollback();
                                    ShowError(InvalidCodeMessage);
                                    return;
                                }

                                resetId = Convert.ToInt32(result);
                            }

                            // Login.Username is the registered email for both account types.
                            // Keep this password assignment consistent with your existing login system.
                            const string updatePasswordSql = @"
                                UPDATE dbo.Login
                                SET Password = @Password
                                WHERE LOWER(Username) = LOWER(@Email);";

                            int rowsUpdated;

                            using (SqlCommand command =
                                new SqlCommand(updatePasswordSql, connection, transaction))
                            {
                                command.Parameters.Add("@Password", SqlDbType.VarChar, 100).Value = newPassword;
                                command.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = email;
                                rowsUpdated = command.ExecuteNonQuery();
                            }

                            if (rowsUpdated != 1)
                            {
                                transaction.Rollback();
                                ShowError(
                                    "We could not update the password for this account. " +
                                    "Please contact the gym administrator.");
                                return;
                            }

                            const string usedSql = @"
                                UPDATE dbo.PasswordReset
                                SET Is_Used = 1
                                WHERE Reset_ID = @ResetID AND Is_Used = 0;";

                            using (SqlCommand command =
                                new SqlCommand(usedSql, connection, transaction))
                            {
                                command.Parameters.Add("@ResetID", SqlDbType.Int).Value = resetId;

                                if (command.ExecuteNonQuery() != 1)
                                {
                                    transaction.Rollback();
                                    ShowError(
                                        "The reset code has already been used. Request a new code.");
                                    return;
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            try { transaction.Rollback(); } catch { }
                            throw;
                        }
                    }
                }

                Response.Redirect("~/Account/Login.aspx?reset=success", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Password reset completion failed: {0}", ex);

                ShowError("We could not complete the password reset. Please try again later.");
            }
        }

        private static bool IsSixDigitCode(string value)
        {
            foreach (char c in value)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static string HashCode(string code)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(code));
                StringBuilder result = new StringBuilder(64);

                foreach (byte b in hash)
                    result.Append(b.ToString("x2"));

                return result.ToString();
            }
        }

        private void ShowError(string message)
        {
            FailureText.Text = Server.HtmlEncode(message);
            ErrorMessage.Visible = true;
        }
    }
}
