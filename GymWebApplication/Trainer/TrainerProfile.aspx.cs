using System;
using System.Configuration;
using System.Data.SqlClient;

namespace GymWebApplication.Trainer
{
    public partial class TrainerProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsTrainerLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
            }
        }


        // =========================================================
        // CHECK TRAINER LOGIN
        // =========================================================

        private bool IsTrainerLoggedIn()
        {
            return
                Request.IsAuthenticated
                &&
                Session["UserRole"] != null
                &&
                Session["UserRole"]
                    .ToString()
                    .Equals(
                        "Trainer",
                        StringComparison.OrdinalIgnoreCase
                    );
        }


        // =========================================================
        // LOAD PROFILE
        // =========================================================

        private void LoadProfile()
        {
            string email =
                User.Identity.Name;

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();


                string sql = @"
                    SELECT
                        Staff_ID,
                        Staff_Role,
                        Staff_FName,
                        Staff_LName,
                        Staff_Telephone,
                        Staff_Phone_Number,
                        Staff_Email,
                        Staff_DOB,
                        Staff_Gender,
                        Staff_Status,
                        Staff_Join_Date,
                        Specialisation

                    FROM Staff

                    WHERE Staff_Email = @Email

                    AND Staff_Role = 'Trainer'
                ";


                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Email",
                        email
                    );


                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            lblMessage.Text =
                                "Trainer profile could not be found.";

                            return;
                        }


                        // ---------------------------------------------
                        // READ-ONLY INFORMATION
                        // ---------------------------------------------

                        lblStaffID.Text =
                            GetValue(
                                reader,
                                "Staff_ID"
                            );


                        lblStatus.Text =
                            GetValue(
                                reader,
                                "Staff_Status"
                            );


                        lblSpecialisation.Text =
                            GetValue(
                                reader,
                                "Specialisation"
                            );


                        lblJoinDate.Text =
                            GetDateValue(
                                reader,
                                "Staff_Join_Date"
                            );


                        // ---------------------------------------------
                        // EDITABLE INFORMATION
                        // ---------------------------------------------

                        txtFirstName.Text =
                            GetValue(
                                reader,
                                "Staff_FName",
                                ""
                            );


                        txtLastName.Text =
                            GetValue(
                                reader,
                                "Staff_LName",
                                ""
                            );


                        txtEmail.Text =
                            GetValue(
                                reader,
                                "Staff_Email",
                                ""
                            );


                        txtPhone.Text =
                            GetValue(
                                reader,
                                "Staff_Phone_Number",
                                ""
                            );


                        txtTelephone.Text =
                            GetValue(
                                reader,
                                "Staff_Telephone",
                                ""
                            );


                        // ---------------------------------------------
                        // DATE OF BIRTH
                        // ---------------------------------------------

                        if (
                            reader["Staff_DOB"]
                            != DBNull.Value
                        )
                        {
                            DateTime dob =
                                Convert.ToDateTime(
                                    reader["Staff_DOB"]
                                );

                            txtDOB.Text =
                                dob.ToString(
                                    "yyyy-MM-dd"
                                );
                        }


                        // ---------------------------------------------
                        // GENDER
                        // ---------------------------------------------

                        string gender =
                            GetValue(
                                reader,
                                "Staff_Gender",
                                ""
                            );


                        if (
                            ddlGender.Items.FindByValue(
                                gender
                            ) != null
                        )
                        {
                            ddlGender.SelectedValue =
                                gender;
                        }
                    }
                }
            }
        }


        // =========================================================
        // SAVE PROFILE
        // =========================================================

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }


            string originalEmail =
                User.Identity.Name;


            string newEmail =
                txtEmail.Text.Trim();


            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;


            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();


                string sql = @"
                    UPDATE Staff

                    SET
                        Staff_FName =
                            @FirstName,

                        Staff_LName =
                            @LastName,

                        Staff_Email =
                            @Email,

                        Staff_Phone_Number =
                            @PhoneNumber,

                        Staff_Telephone =
                            @Telephone,

                        Staff_DOB =
                            @DOB,

                        Staff_Gender =
                            @Gender

                    WHERE Staff_Email =
                          @OriginalEmail

                    AND Staff_Role =
                        'Trainer'
                ";


                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@FirstName",
                        txtFirstName.Text.Trim()
                    );


                    command.Parameters.AddWithValue(
                        "@LastName",
                        txtLastName.Text.Trim()
                    );


                    command.Parameters.AddWithValue(
                        "@Email",
                        newEmail
                    );


                    command.Parameters.AddWithValue(
                        "@PhoneNumber",
                        txtPhone.Text.Trim()
                    );


                    command.Parameters.AddWithValue(
                        "@Telephone",
                        txtTelephone.Text.Trim()
                    );


                    if (
                        string.IsNullOrWhiteSpace(
                            txtDOB.Text
                        )
                    )
                    {
                        command.Parameters.AddWithValue(
                            "@DOB",
                            DBNull.Value
                        );
                    }
                    else
                    {
                        DateTime dob;

                        if (
                            DateTime.TryParse(
                                txtDOB.Text,
                                out dob
                            )
                        )
                        {
                            command.Parameters.AddWithValue(
                                "@DOB",
                                dob
                            );
                        }
                        else
                        {
                            command.Parameters.AddWithValue(
                                "@DOB",
                                DBNull.Value
                            );
                        }
                    }


                    if (
                        string.IsNullOrWhiteSpace(
                            ddlGender.SelectedValue
                        )
                    )
                    {
                        command.Parameters.AddWithValue(
                            "@Gender",
                            DBNull.Value
                        );
                    }
                    else
                    {
                        command.Parameters.AddWithValue(
                            "@Gender",
                            ddlGender.SelectedValue
                        );
                    }


                    command.Parameters.AddWithValue(
                        "@OriginalEmail",
                        originalEmail
                    );


                    int affected =
                        command.ExecuteNonQuery();


                    if (affected > 0)
                    {
                        lblMessage.Text =
                            "Your profile has been updated successfully.";

                        lblMessage.CssClass =
                            "trainer-message trainer-message-success";
                    }
                    else
                    {
                        lblMessage.Text =
                            "No profile changes were saved.";

                        lblMessage.CssClass =
                            "trainer-message trainer-message-error";
                    }
                }
            }
        }


        // =========================================================
        // CANCEL
        // =========================================================

        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "TrainerProfile.aspx"
            );
        }


        // =========================================================
        // GET STRING VALUE
        // =========================================================

        private static string GetValue(
            SqlDataReader reader,
            string column,
            string emptyValue = "—")
        {
            if (
                reader[column]
                == DBNull.Value
            )
            {
                return emptyValue;
            }


            string value =
                reader[column]
                    .ToString()
                    .Trim();


            if (
                string.IsNullOrWhiteSpace(value)
            )
            {
                return emptyValue;
            }


            return value;
        }


        // =========================================================
        // GET DATE
        // =========================================================

        private static string GetDateValue(
            SqlDataReader reader,
            string column)
        {
            if (
                reader[column]
                == DBNull.Value
            )
            {
                return "—";
            }


            return Convert
                .ToDateTime(
                    reader[column]
                )
                .ToString(
                    "dd/MM/yyyy"
                );
        }
    }
}