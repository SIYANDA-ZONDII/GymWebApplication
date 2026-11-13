using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace GymWebApplication.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ErrorMessage.Visible = false;
            }

            if (User.Identity.IsAuthenticated)
            {
                RedirectAuthenticatedUser();
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string username = Email.Text.Trim();
            string password = Password.Text;

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    const string loginSql = @"
                        SELECT UserRole
                        FROM Login
                        WHERE Username = @Username
                          AND Password = @Password;";

                    string databaseRole;

                    using (SqlCommand command =
                           new SqlCommand(loginSql, connection))
                    {
                        command.Parameters.Add(
                            "@Username",
                            SqlDbType.VarChar,
                            100
                        ).Value = username;

                        command.Parameters.Add(
                            "@Password",
                            SqlDbType.VarChar,
                            100
                        ).Value = password;

                        object result = command.ExecuteScalar();

                        if (result == null ||
                            result == DBNull.Value)
                        {
                            ShowError(
                                "Incorrect email or password."
                            );

                            return;
                        }

                        databaseRole =
                            result.ToString().Trim();
                    }

                    if (databaseRole.Equals(
                            "Member",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        LoginMember(
                            connection,
                            username
                        );

                        return;
                    }

                    if (!IsSupportedStaffRole(databaseRole))
                    {
                        ShowError(
                            "Your account does not have a valid system role."
                        );

                        return;
                    }

                    if (!IsActiveStaff(
                            connection,
                            username,
                            databaseRole))
                    {
                        ShowError(
                            "This staff account is inactive. Please contact a manager."
                        );

                        return;
                    }

                    Session["Username"] = username;
                    Session["UserRole"] = databaseRole;

                    LoadStaffSessionDetails(
                        connection,
                        username,
                        databaseRole
                    );

                    FormsAuthentication.SetAuthCookie(
                        username,
                        RememberMe.Checked
                    );

                    RedirectAccordingToRole(databaseRole);
                }
                catch (SqlException)
                {
                    ShowError(
                        "The system could not connect to the database. Please try again."
                    );
                }
                catch (Exception)
                {
                    ShowError(
                        "The application could not complete your login. Please try again."
                    );
                }
            }
        }

        private static bool IsSupportedStaffRole(
            string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            return role.Trim().Equals(
                       "Trainer",
                       StringComparison.OrdinalIgnoreCase
                   )
                   ||
                   role.Trim().Equals(
                       "Manager",
                       StringComparison.OrdinalIgnoreCase
                   );
        }

        private bool IsActiveStaff(
            SqlConnection connection,
            string username,
            string role)
        {
            const string sql = @"
                SELECT Staff_Status
                FROM Staff
                WHERE Staff_Email = @Email
                  AND Staff_Role = @Role;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    100
                ).Value = username;

                command.Parameters.Add(
                    "@Role",
                    SqlDbType.VarChar,
                    20
                ).Value = role;

                object result = command.ExecuteScalar();

                return result != null
                    && result != DBNull.Value
                    && result.ToString().Trim().Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase
                    );
            }
        }

        private void LoadStaffSessionDetails(
            SqlConnection connection,
            string username,
            string role)
        {
            const string sql = @"
                SELECT
                    Staff_ID,
                    Staff_FName,
                    Staff_LName,
                    Staff_Status
                FROM Staff
                WHERE Staff_Email = @Email
                  AND Staff_Role = @Role;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    100
                ).Value = username;

                command.Parameters.Add(
                    "@Role",
                    SqlDbType.VarChar,
                    20
                ).Value = role;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    Session["StaffID"] =
                        Convert.ToString(
                            reader["Staff_ID"]
                        ).Trim();

                    Session[role + "FirstName"] =
                        Convert.ToString(
                            reader["Staff_FName"]
                        ).Trim();

                    Session[role + "LastName"] =
                        Convert.ToString(
                            reader["Staff_LName"]
                        ).Trim();

                    Session["StaffStatus"] =
                        Convert.ToString(
                            reader["Staff_Status"]
                        ).Trim();
                }
            }
        }

        private void LoginMember(
            SqlConnection connection,
            string username)
        {
            const string memberSql = @"
                SELECT
                    Member_ID,
                    Member_FName,
                    Member_LName,
                    Member_Email,
                    Member_Type,
                    Member_Status
                FROM Member
                WHERE Member_Email = @Email;";

            using (SqlCommand command =
                   new SqlCommand(memberSql, connection))
            {
                command.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    100
                ).Value = username;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ShowError(
                            "Your login account exists, but no member record was found."
                        );

                        return;
                    }

                    string memberId =
                        Convert.ToString(
                            reader["Member_ID"]
                        ).Trim();

                    string memberType =
                        reader["Member_Type"] == DBNull.Value
                            ? ""
                            : reader["Member_Type"]
                                .ToString()
                                .Trim();

                    string memberStatus =
                        reader["Member_Status"] == DBNull.Value
                            ? ""
                            : reader["Member_Status"]
                                .ToString()
                                .Trim();

                    string firstName =
                        reader["Member_FName"] == DBNull.Value
                            ? ""
                            : reader["Member_FName"]
                                .ToString()
                                .Trim();

                    string lastName =
                        reader["Member_LName"] == DBNull.Value
                            ? ""
                            : reader["Member_LName"]
                                .ToString()
                                .Trim();

                    Session["MemberID"] = memberId;
                    Session["Username"] = username;
                    Session["UserRole"] = "Member";
                    Session["MemberType"] = memberType;
                    Session["MemberStatus"] = memberStatus;
                    Session["MemberFirstName"] = firstName;
                    Session["MemberLastName"] = lastName;

                    FormsAuthentication.SetAuthCookie(
                        username,
                        RememberMe.Checked
                    );
                }
            }

            Response.Redirect(
                "~/Member/MemberDashboard.aspx"
            );
        }

        private void RedirectAuthenticatedUser()
        {
            string username =
                User.Identity.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                SignOutCurrentUser();
                return;
            }

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    const string sql = @"
                        SELECT UserRole
                        FROM Login
                        WHERE Username = @Username;";

                    string role;

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Username",
                            SqlDbType.VarChar,
                            100
                        ).Value = username;

                        object result =
                            command.ExecuteScalar();

                        if (result == null ||
                            result == DBNull.Value)
                        {
                            SignOutCurrentUser();
                            return;
                        }

                        role =
                            result.ToString().Trim();
                    }

                    if (role.Equals(
                            "Member",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        LoadAuthenticatedMember(
                            connection,
                            username
                        );

                        return;
                    }

                    if (!IsSupportedStaffRole(role))
                    {
                        SignOutCurrentUser();

                        ShowError(
                            "Your account does not have a valid system role."
                        );

                        return;
                    }

                    if (!IsActiveStaff(
                            connection,
                            username,
                            role))
                    {
                        SignOutCurrentUser();

                        ShowError(
                            "This staff account is inactive. Please contact a manager."
                        );

                        return;
                    }

                    Session["Username"] = username;
                    Session["UserRole"] = role;

                    LoadStaffSessionDetails(
                        connection,
                        username,
                        role
                    );

                    RedirectAccordingToRole(role);
                }
                catch (SqlException)
                {
                    SignOutCurrentUser();

                    ShowError(
                        "Unable to verify your account. Please try again."
                    );
                }
                catch (Exception)
                {
                    SignOutCurrentUser();

                    ShowError(
                        "Unable to verify your account. Please try again."
                    );
                }
            }
        }

        private void LoadAuthenticatedMember(
            SqlConnection connection,
            string username)
        {
            const string sql = @"
                SELECT
                    Member_ID,
                    Member_FName,
                    Member_LName,
                    Member_Type,
                    Member_Status
                FROM Member
                WHERE Member_Email = @Email;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    100
                ).Value = username;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        SignOutCurrentUser();

                        ShowError(
                            "No member record was found for this account."
                        );

                        return;
                    }

                    Session["MemberID"] =
                        Convert.ToString(
                            reader["Member_ID"]
                        ).Trim();

                    Session["Username"] = username;
                    Session["UserRole"] = "Member";

                    Session["MemberType"] =
                        reader["Member_Type"] == DBNull.Value
                            ? ""
                            : reader["Member_Type"]
                                .ToString()
                                .Trim();

                    Session["MemberStatus"] =
                        reader["Member_Status"] == DBNull.Value
                            ? ""
                            : reader["Member_Status"]
                                .ToString()
                                .Trim();

                    Session["MemberFirstName"] =
                        reader["Member_FName"] == DBNull.Value
                            ? ""
                            : reader["Member_FName"]
                                .ToString()
                                .Trim();

                    Session["MemberLastName"] =
                        reader["Member_LName"] == DBNull.Value
                            ? ""
                            : reader["Member_LName"]
                                .ToString()
                                .Trim();
                }
            }

            Response.Redirect(
                "~/Member/MemberDashboard.aspx"
            );
        }

        private void RedirectAccordingToRole(
            string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                SignOutCurrentUser();

                ShowError(
                    "Your account does not have a valid system role."
                );

                return;
            }

            switch (role.Trim().ToLowerInvariant())
            {
                case "member":

                    Response.Redirect(
                        "~/Member/MemberDashboard.aspx"
                    );

                    break;

                case "trainer":

                    Response.Redirect(
                        "~/Trainer/TrainerDashboard.aspx"
                    );

                    break;

                case "manager":

                    Response.Redirect(
                        "~/Manager/ManagerDashboard.aspx"
                    );

                    break;

                default:

                    SignOutCurrentUser();

                    ShowError(
                        "Your account does not have a valid system role."
                    );

                    break;
            }
        }

        private void SignOutCurrentUser()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();
        }

        private void ShowError(
            string message)
        {
            FailureText.Text = message;
            ErrorMessage.Visible = true;
        }
    }
}