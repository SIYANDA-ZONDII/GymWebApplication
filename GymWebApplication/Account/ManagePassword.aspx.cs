using System;
using System.Configuration;
using System.Data.SqlClient;

namespace GymWebApplication.Account
{
    public partial class ManagePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Make sure the user is logged in
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string message = Request.QueryString["m"];

                if (!string.IsNullOrEmpty(message))
                {
                    if (message == "ChangePwdSuccess")
                    {
                        ShowMessage(
                            "Your password has been changed successfully."
                        );
                    }
                    else if (message == "SetPwdSuccess")
                    {
                        ShowMessage(
                            "Your password has been set successfully."
                        );
                    }
                }
            }
        }


        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        protected void ChangePassword_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string username = User.Identity.Name;
            string currentPassword = CurrentPassword.Text;
            string newPassword = NewPassword.Text;
            string confirmPassword = ConfirmNewPassword.Text;


            // -----------------------------------------------------
            // Check user
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError(
                    "Unable to identify your account."
                );

                return;
            }


            // -----------------------------------------------------
            // Check new passwords
            // -----------------------------------------------------

            if (newPassword != confirmPassword)
            {
                ShowError(
                    "The new passwords do not match."
                );

                return;
            }


            // -----------------------------------------------------
            // Make sure new password is different
            // -----------------------------------------------------

            if (currentPassword == newPassword)
            {
                ShowError(
                    "Your new password must be different from your current password."
                );

                return;
            }


            // -----------------------------------------------------
            // Get database connection
            // -----------------------------------------------------

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();


                // =================================================
                // CHECK CURRENT PASSWORD
                // =================================================

                string checkQuery = @"
                    SELECT COUNT(*)
                    FROM Login
                    WHERE Username = @Username
                    AND Password = @Password";


                using (SqlCommand command =
                       new SqlCommand(checkQuery, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Username",
                        username
                    );

                    command.Parameters.AddWithValue(
                        "@Password",
                        currentPassword
                    );


                    int count =
                        Convert.ToInt32(
                            command.ExecuteScalar()
                        );


                    if (count == 0)
                    {
                        ShowError(
                            "Your current password is incorrect."
                        );

                        return;
                    }
                }


                // =================================================
                // UPDATE PASSWORD
                // =================================================

                string updateQuery = @"
                    UPDATE Login
                    SET Password = @NewPassword
                    WHERE Username = @Username";


                using (SqlCommand command =
                       new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue(
                        "@NewPassword",
                        newPassword
                    );

                    command.Parameters.AddWithValue(
                        "@Username",
                        username
                    );


                    int rowsAffected =
                        command.ExecuteNonQuery();


                    if (rowsAffected == 1)
                    {
                        Response.Redirect(
                            "~/Account/ManagePassword.aspx?m=ChangePwdSuccess"
                        );

                        return;
                    }


                    ShowError(
                        "The password could not be changed."
                    );
                }
            }
        }


        // =========================================================
        // DISPLAY ERROR
        // =========================================================

        private void ShowError(string message)
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "PasswordError",
                "alert('" +
                message.Replace("'", "\\'") +
                "');",
                true
            );
        }


        // =========================================================
        // DISPLAY SUCCESS MESSAGE
        // =========================================================

        private void ShowMessage(string message)
        {
            ClientScript.RegisterStartupScript(
                GetType(),
                "PasswordMessage",
                "alert('" +
                message.Replace("'", "\\'") +
                "');",
                true
            );
        }
    }
}