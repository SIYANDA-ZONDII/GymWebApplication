
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using MailKit.Security;
using MimeKit;

namespace GymWebApplication.Account
{
    public partial class ForgotPassword : Page
    {
        private const string NotFoundMessage =
            "We could not find an account matching those details. " +
            "Check your student number or registered email address and try again.";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ErrorMessage.Visible = false;
                DisplayEmail.Visible = false;
                ResetForm.Visible = true;
            }
        }

        protected void SendCode_Click(object sender, EventArgs e)
        {
            ErrorMessage.Visible = false;
            DisplayEmail.Visible = false;
            ResetForm.Visible = true;

            if (!Page.IsValid)
                return;

            string input = (StudentNumberOrEmail.Text ?? "").Trim();

            if (input.Length == 0)
            {
                ShowError("Please enter your student number, staff ID, or registered email address.");
                return;
            }

            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string userId = null;
            string email = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Member IDs and staff IDs are resolved against their own tables.
                    const string sql = @"
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

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@Input", SqlDbType.VarChar, 100).Value = input;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader["AccountID"] == DBNull.Value
                                    ? null : reader["AccountID"].ToString().Trim();

                                email = reader["AccountEmail"] == DBNull.Value
                                    ? null : reader["AccountEmail"].ToString().Trim();
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(userId) ||
                    string.IsNullOrWhiteSpace(email))
                {
                    ShowError(NotFoundMessage);
                    return;
                }

                try
                {
                    System.Net.Mail.MailAddress parsed =
                        new System.Net.Mail.MailAddress(email);

                    if (!string.Equals(parsed.Address, email,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ShowError("The email address registered to this account is invalid. Please contact the administrator.");
                        return;
                    }
                }
                catch (FormatException)
                {
                    ShowError("The email address registered to this account is invalid. Please contact the administrator.");
                    return;
                }

                string otp = GenerateResetCode();
                string otpHash = HashCode(otp);

                // Save before sending, then invalidate the code if SMTP fails.
                StoreResetCode(userId, otpHash);

                try
                {
                    SendResetEmail(email, otp);
                }
                catch (Exception ex)
                {
                    try { InvalidatePendingCodes(userId); }
                    catch (Exception cleanup)
                    {
                        System.Diagnostics.Trace.TraceError(
                            "Reset-code cleanup failed: {0}", cleanup);
                    }

                    System.Diagnostics.Trace.TraceError(
                        "Password-reset email failed: {0}", ex);

                    ShowError("We could not send the reset email. Please try again later.");
                    return;
                }

                MaskedEmail.Text = Server.HtmlEncode(MaskEmail(email));
                ResetForm.Visible = false;
                ErrorMessage.Visible = false;
                DisplayEmail.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Forgot-password request failed: {0}", ex);

                ShowError("We could not process your request right now. Please try again later.");
            }
        }

        private static string GenerateResetCode()
        {
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                uint value;

                do
                {
                    rng.GetBytes(bytes);
                    value = BitConverter.ToUInt32(bytes, 0);
                }
                while (value >= uint.MaxValue - (uint.MaxValue % 900000));

                return ((value % 900000) + 100000).ToString();
            }
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

        private static string MaskEmail(string email)
        {
            int at = email.LastIndexOf('@');

            if (at <= 0 || at == email.Length - 1)
                return "***";

            string local = email.Substring(0, at);
            string domain = email.Substring(at);

            // Show only the final character before @.
            return new string('*', Math.Max(3, local.Length - 1))
                + local.Substring(local.Length - 1)
                + domain;
        }

        private static void StoreResetCode(string userId, string hash)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        const string invalidate = @"
                            UPDATE dbo.PasswordReset
                            SET Is_Used = 1
                            WHERE User_ID = @UserID AND Is_Used = 0;";

                        using (SqlCommand command = new SqlCommand(invalidate, connection, transaction))
                        {
                            command.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = userId;
                            command.ExecuteNonQuery();
                        }

                        const string insert = @"
                            INSERT INTO dbo.PasswordReset
                                (User_ID, Reset_Code, Expiry_Date, Is_Used)
                            VALUES
                                (@UserID, @Hash, DATEADD(MINUTE, 10, GETDATE()), 0);";

                        using (SqlCommand command = new SqlCommand(insert, connection, transaction))
                        {
                            command.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = userId;
                            command.Parameters.Add("@Hash", SqlDbType.VarChar, 64).Value = hash;
                            command.ExecuteNonQuery();
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
        }

        private static void InvalidatePendingCodes(string userId)
        {
            string cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(cs))
            using (SqlCommand command = new SqlCommand(@"
                UPDATE dbo.PasswordReset
                SET Is_Used = 1
                WHERE User_ID = @UserID AND Is_Used = 0;", connection))
            {
                command.Parameters.Add("@UserID", SqlDbType.VarChar, 50).Value = userId;
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static string RequiredSetting(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);

            if (string.IsNullOrWhiteSpace(value))
                value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value) ||
                value.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase))
            {
                throw new ConfigurationErrorsException(
                    "Missing SMTP configuration setting: " + key);
            }

            return value.Trim();
        }

        private static void SendResetEmail(string email, string otp)
        {
            string host = RequiredSetting("SmtpHost");
            string username = RequiredSetting("SmtpUsername");
            string password = RequiredSetting("SmtpPassword");
            string fromEmail = RequiredSetting("SmtpFromEmail");

            int configuredPort;
            if (!int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out configuredPort) ||
                (configuredPort != 465 && configuredPort != 587))
                configuredPort = 587;

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("UKZN SPORT", fromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "UKZN SPORT - Password Reset Code";

            BodyBuilder body = new BodyBuilder();
            body.TextBody =
                "Your UKZN SPORT password reset code is " + otp +
                ". It expires in 10 minutes. If you did not request this, ignore this email.";

            body.HtmlBody =
                "<html><body style='font-family:Arial,sans-serif'>" +
                "<h2>UKZN SPORT Password Reset</h2>" +
                "<p>Your six-digit reset code is:</p>" +
                "<p style='font-size:30px;font-weight:bold;letter-spacing:6px'>" +
                otp + "</p><p>This code expires in <strong>10 minutes</strong>.</p>" +
                "<p>If you did not request this reset, ignore this email.</p>" +
                "</body></html>";

            message.Body = body.ToMessageBody();

            int[] ports = configuredPort == 465
                ? new[] { 465, 587 }
                : new[] { 587, 465 };

            Exception lastError = null;

            foreach (int port in ports)
            {
                SecureSocketOptions security = port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

                try
                {
                    using (MailKit.Net.Smtp.SmtpClient smtp =
                        new MailKit.Net.Smtp.SmtpClient())
                    {
                        smtp.Timeout = 20000;
                        smtp.Connect(host, port, security);
                        smtp.Authenticate(username, password);
                        smtp.Send(message);
                        smtp.Disconnect(true);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    System.Diagnostics.Trace.TraceWarning(
                        "SMTP attempt on port {0} failed: {1}", port, ex.Message);
                }
            }

            throw new InvalidOperationException(
                "Email delivery failed using both SMTP ports. Verify the host, network, and app password.",
                lastError);
        }

        private void ShowError(string message)
        {
            FailureText.Text = Server.HtmlEncode(message);
            ErrorMessage.Visible = true;
            DisplayEmail.Visible = false;
            ResetForm.Visible = true;
        }
    }
}
