
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace GymWebApplication
{
    public partial class site_master : MasterPage
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupNavigation();

            if (!IsPostBack)
            {
                LoadGlobalSessionReminder();
            }
        }

        private void SetupNavigation()
        {
            bool loggedIn = Request.IsAuthenticated;

            string role = Session["UserRole"] == null
                ? ""
                : Session["UserRole"].ToString().Trim();

            bool isMember = loggedIn &&
                role.Equals("Member", StringComparison.OrdinalIgnoreCase);

            bool isTrainer = loggedIn &&
                role.Equals("Trainer", StringComparison.OrdinalIgnoreCase);

            bool isManager = loggedIn &&
                role.Equals("Manager", StringComparison.OrdinalIgnoreCase);

            PublicAboutLink.Visible = !loggedIn;
            PublicContactLink.Visible = !loggedIn;

            AnonymousLinks.Visible = !loggedIn;
            LoginLink.Visible = !loggedIn;

            UserWelcome.Visible = loggedIn;
            LogoutLink.Visible = loggedIn;

            MemberProfileLink.Visible = isMember;
            MemberPaymentLink.Visible = isMember;

            // Members use one page for booking and managing sessions.
            MemberTrainingLink.Visible = isMember;
            MemberBookingLink.Visible = false;

            TrainerProfileLink.Visible = isTrainer;
            TrainerSessionsLink.Visible = isTrainer;
            TrainerAvailabilityLink.Visible = isTrainer;
            TrainerMembersLink.Visible = isTrainer;

            ManagerDashboardLink.Visible = isManager;
            ManagerMembersLink.Visible = isManager;
            ManagerTrainersLink.Visible = isManager;
            ManagerSessionsLink.Visible = isManager;
            ManagerReportsLink.Visible = isManager;

            string homeUrl = "~/Default.aspx";

            if (isMember)
            {
                homeUrl = "~/Member/MemberDashboard.aspx";
            }
            else if (isTrainer)
            {
                homeUrl = "~/Trainer/TrainerDashboard.aspx";
            }
            else if (isManager)
            {
                homeUrl = "~/Manager/ManagerDashboard.aspx";
            }

            NavHome.HRef = ResolveUrl(homeUrl);
            BrandHomeLink.HRef = ResolveUrl(homeUrl);

            if (isMember)
            {
                UserAccountLink.HRef =
                    ResolveUrl("~/Member/MemberProfile.aspx");

                lblUserName.Text = GetMemberName();
            }
            else if (isTrainer)
            {
                UserAccountLink.HRef =
                    ResolveUrl("~/Trainer/TrainerProfile.aspx");

                lblUserName.Text = GetStaffName("Trainer");
            }
            else if (isManager)
            {
                UserAccountLink.HRef =
                    ResolveUrl("~/Manager/ManagerDashboard.aspx");

                lblUserName.Text = GetStaffName("Manager");
            }
            else
            {
                UserAccountLink.HRef = ResolveUrl("~/Default.aspx");
                lblUserName.Text = "User";
            }
        }

        private string GetMemberName()
        {
            string firstName = Session["MemberFirstName"] == null
                ? ""
                : Session["MemberFirstName"].ToString().Trim();

            string lastName = Session["MemberLastName"] == null
                ? ""
                : Session["MemberLastName"].ToString().Trim();

            string fullName = (firstName + " " + lastName).Trim();

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            if (Session["MemberID"] == null)
            {
                return "Member";
            }

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT Member_FName, Member_LName
                        FROM Member
                        WHERE Member_ID = @Member_ID;";

                    using (SqlCommand command =
                        new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Member_ID", SqlDbType.VarChar, 20
                        ).Value = Session["MemberID"].ToString();

                        connection.Open();

                        using (SqlDataReader reader =
                            command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return "Member";
                            }

                            firstName =
                                Convert.ToString(reader["Member_FName"]).Trim();

                            lastName =
                                Convert.ToString(reader["Member_LName"]).Trim();
                        }
                    }
                }

                Session["MemberFirstName"] = firstName;
                Session["MemberLastName"] = lastName;

                fullName = (firstName + " " + lastName).Trim();

                return string.IsNullOrWhiteSpace(fullName)
                    ? "Member"
                    : fullName;
            }
            catch
            {
                return "Member";
            }
        }

        private string GetStaffName(string role)
        {
            string firstSessionKey = role + "FirstName";
            string lastSessionKey = role + "LastName";

            string firstName = Session[firstSessionKey] == null
                ? ""
                : Session[firstSessionKey].ToString().Trim();

            string lastName = Session[lastSessionKey] == null
                ? ""
                : Session[lastSessionKey].ToString().Trim();

            string fullName = (firstName + " " + lastName).Trim();

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                return fullName;
            }

            if (Context.User == null ||
                Context.User.Identity == null ||
                string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                return role;
            }

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT Staff_FName, Staff_LName
                        FROM Staff
                        WHERE Staff_Email = @Email
                          AND Staff_Role = @Role;";

                    using (SqlCommand command =
                        new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Email", SqlDbType.VarChar, 100
                        ).Value = Context.User.Identity.Name;

                        command.Parameters.Add(
                            "@Role", SqlDbType.VarChar, 20
                        ).Value = role;

                        connection.Open();

                        using (SqlDataReader reader =
                            command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return role;
                            }

                            firstName =
                                Convert.ToString(reader["Staff_FName"]).Trim();

                            lastName =
                                Convert.ToString(reader["Staff_LName"]).Trim();
                        }
                    }
                }

                Session[firstSessionKey] = firstName;
                Session[lastSessionKey] = lastName;

                fullName = (firstName + " " + lastName).Trim();

                return string.IsNullOrWhiteSpace(fullName)
                    ? role
                    : fullName;
            }
            catch
            {
                return role;
            }
        }

        private void LoadGlobalSessionReminder()
        {
            GlobalReminderPanel.Visible = false;

            if (!Request.IsAuthenticated ||
                Session["UserRole"] == null ||
                Session["MemberID"] == null)
            {
                return;
            }

            string role = Session["UserRole"].ToString().Trim();

            if (!role.Equals(
                "Member", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string currentPage = Request
                .AppRelativeCurrentExecutionFilePath
                .ToLowerInvariant();

            if (currentPage.EndsWith("/memberdashboard.aspx"))
            {
                return;
            }

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT TOP 1
                            T.Training_ID,
                            S.Session_Type,
                            T.Session_Date,
                            TI.Start_Time,
                            ISNULL(
                                ST.Staff_FName + ' ' + ST.Staff_LName,
                                'your trainer'
                            ) AS Trainer_Name
                        FROM Training_Session AS T
                        INNER JOIN Session_Table AS S
                            ON S.Session_ID = T.Session_ID
                        INNER JOIN Time_Slot AS TI
                            ON TI.Time_ID = T.Time_ID
                        LEFT JOIN Staff AS ST
                            ON ST.Staff_ID = T.Staff_ID
                        WHERE T.Member_ID = @Member_ID
                          AND ISNULL(T.Session_Status, 'Pending') = 'Pending'
                          AND DATEADD(
                                SECOND,
                                DATEDIFF(
                                    SECOND,
                                    CAST('00:00:00' AS TIME),
                                    TI.Start_Time
                                ),
                                CAST(T.Session_Date AS DATETIME)
                              ) > GETDATE()
                          AND DATEADD(
                                SECOND,
                                DATEDIFF(
                                    SECOND,
                                    CAST('00:00:00' AS TIME),
                                    TI.Start_Time
                                ),
                                CAST(T.Session_Date AS DATETIME)
                              ) <= DATEADD(HOUR, 24, GETDATE())
                        ORDER BY T.Session_Date, TI.Start_Time;";

                    using (SqlCommand command =
                        new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Member_ID", SqlDbType.VarChar, 20
                        ).Value = Session["MemberID"].ToString();

                        connection.Open();

                        using (SqlDataReader reader =
                            command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return;
                            }

                            int trainingId =
                                Convert.ToInt32(reader["Training_ID"]);

                            if (Session["DismissedReminderTrainingID"] != null &&
                                Session["DismissedReminderTrainingID"].ToString()
                                    == trainingId.ToString())
                            {
                                return;
                            }

                            DateTime sessionDate =
                                Convert.ToDateTime(reader["Session_Date"]).Date;

                            TimeSpan startTime =
                                (TimeSpan)reader["Start_Time"];

                            DateTime sessionStart =
                                sessionDate.Add(startTime);

                            ViewState["GlobalReminderTrainingID"] =
                                trainingId;

                            lblGlobalReminderTitle.Text =
                                Convert.ToString(reader["Session_Type"]) +
                                " with " +
                                Convert.ToString(reader["Trainer_Name"]);

                            lblGlobalReminderMessage.Text =
                                "Begins " +
                                sessionStart.ToString(
                                    "dd MMMM yyyy 'at' HH:mm") +
                                " · " +
                                GetRemainingTime(sessionStart) +
                                " remaining.";

                            GlobalReminderPanel.Visible = true;
                        }
                    }
                }
            }
            catch
            {
                GlobalReminderPanel.Visible = false;
            }
        }

        private static string GetRemainingTime(DateTime sessionStart)
        {
            TimeSpan remaining = sessionStart - DateTime.Now;

            if (remaining.TotalMinutes <= 0)
            {
                return "less than 1 minute";
            }

            if (remaining.TotalHours < 1)
            {
                int minutes = Math.Max(
                    1,
                    Convert.ToInt32(
                        Math.Ceiling(remaining.TotalMinutes)));

                return minutes +
                    (minutes == 1 ? " minute" : " minutes");
            }

            int hours = Convert.ToInt32(
                Math.Floor(remaining.TotalHours));

            int remainingMinutes = remaining.Minutes;

            string result = hours +
                (hours == 1 ? " hour" : " hours");

            if (remainingMinutes > 0)
            {
                result += " " + remainingMinutes +
                    (remainingMinutes == 1 ? " minute" : " minutes");
            }

            return result;
        }

        protected void btnDismissGlobalReminder_Click(
            object sender, EventArgs e)
        {
            if (ViewState["GlobalReminderTrainingID"] != null)
            {
                Session["DismissedReminderTrainingID"] =
                    ViewState["GlobalReminderTrainingID"].ToString();
            }

            GlobalReminderPanel.Visible = false;
            GlobalReminderUpdatePanel.Update();
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Default.aspx");
        }
    }
}
