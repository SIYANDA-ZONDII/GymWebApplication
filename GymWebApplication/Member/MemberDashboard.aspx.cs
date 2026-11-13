using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace GymWebApplication.Member
{
    public partial class MemberDashboard : Page
    {
        // =========================================================
        // CONNECTION STRING
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;
            }
        }


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsMemberLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }


        // =========================================================
        // CHECK MEMBER LOGIN
        // =========================================================

        private bool IsMemberLoggedIn()
        {
            return Request.IsAuthenticated
                && Session["UserRole"] != null
                && Session["MemberID"] != null
                && Session["UserRole"]
                    .ToString()
                    .Trim()
                    .Equals(
                        "Member",
                        StringComparison.OrdinalIgnoreCase
                    );
        }


        // =========================================================
        // LOAD DASHBOARD
        // =========================================================

        private void LoadDashboard()
        {
            string memberId =
                Session["MemberID"]
                    .ToString()
                    .Trim();

            lblAccountEmail.Text =
                User.Identity.Name;

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Automatically complete old sessions.
                    AutoCompletePastSessions(
                        connection,
                        memberId
                    );

                    // Update membership status before displaying it.
                    UpdateExpiredMembershipStatus(
                        connection,
                        memberId
                    );

                    // Member information.
                    LoadMemberInformation(
                        connection,
                        memberId
                    );

                    // Session statistics.
                    LoadSessionStatistics(
                        connection,
                        memberId
                    );

                    // Membership information.
                    LoadMembershipInformation(
                        connection,
                        memberId
                    );

                    // Membership notifications.
                    LoadMembershipNotification(
                        connection,
                        memberId
                    );

                    // Receptionist announcements.
                    LoadMemberNotifications(
                        connection,
                        memberId
                    );

                    // Upcoming session reminder.
                    LoadSessionReminder(
                        connection,
                        memberId
                    );
                }
            }
            catch (Exception)
            {
                SessionReminderPanel.Visible = false;

                lblUpcomingSessions.Text = "0";
                lblCompletedSessions.Text = "0";
                lblCancelledSessions.Text = "0";

                lblMembershipStatus.Text = "UNAVAILABLE";

                lblMembershipExpiry.Text =
                    "Information could not be loaded";
            }
        }


        // =========================================================
        // LOAD ANNOUNCEMENTS SENT TO THIS MEMBER
        // =========================================================







        private void LoadMemberNotifications(SqlConnection connection, string memberId)
        {
            const string sql = @"
        SELECT
    N.Notification_ID,
    N.Title,
    N.Message,
    N.Created_Date AS Created_At,
    CASE
        WHEN LOWER(LTRIM(RTRIM(CONVERT(VARCHAR(10), T.Is_Read))))
             IN ('1', 'true', 'yes')
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS Is_Read
FROM dbo.Notifications AS N
INNER JOIN dbo.Notification_Target AS T
    ON T.Notification_ID = N.Notification_ID
WHERE LTRIM(RTRIM(T.User_ID)) = @Member_ID
  AND N.Created_Date >= DATEADD(DAY, -7, SYSDATETIME())
ORDER BY N.Created_Date DESC;";

            try
            {
                DataTable notifications = new DataTable();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Member_ID", SqlDbType.VarChar, 9)
                        .Value = memberId.Trim();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(notifications);
                    }
                }

                rptNotifications.DataSource = notifications;
                rptNotifications.DataBind();

                int unreadCount = 0;

                foreach (DataRow row in notifications.Rows)
                {
                    if (!Convert.ToBoolean(row["Is_Read"]))
                    {
                        unreadCount++;
                    }
                }

                lblNotificationCount.Text = unreadCount + " unread";

                lblNotificationEmpty.Text =
                    "You have no announcements at the moment.";

                lblNotificationEmpty.Visible = notifications.Rows.Count == 0;
            }
            catch (Exception ex)
            {
                rptNotifications.DataSource = null;
                rptNotifications.DataBind();

                lblNotificationCount.Text = "Error";

                lblNotificationEmpty.Text =
                    "Notification error: " + ex.Message;

                lblNotificationEmpty.Visible = true;

                System.Diagnostics.Debug.WriteLine(
                    "MEMBER NOTIFICATION ERROR: " + ex.ToString());
            }
        }







        // =========================================================
        // MARK A MEMBER'S NOTIFICATION AS READ
        // =========================================================


        protected void rptNotifications_ItemCommand(
            object source,
            System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (!e.CommandName.Equals(
                "MarkRead",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!IsMemberLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            int notificationId;

            if (!Int32.TryParse(
                Convert.ToString(e.CommandArgument),
                out notificationId))
            {
                return;
            }

            string memberId = Session["MemberID"].ToString().Trim();

            const string sql = @"
        UPDATE T
        SET T.Is_Read = 1
        FROM dbo.Notification_Target AS T
        INNER JOIN dbo.Notifications AS N
            ON N.Notification_ID = T.Notification_ID
        WHERE T.Notification_ID = @Notification_ID
          AND LTRIM(RTRIM(T.User_ID)) = @User_ID
          AND ISNULL(
                LOWER(LTRIM(RTRIM(CONVERT(VARCHAR(10), T.Is_Read)))),
                '0'
              ) NOT IN ('1', 'true', 'yes');";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Notification_ID", SqlDbType.Int)
                    .Value = notificationId;

                command.Parameters.Add("@User_ID", SqlDbType.VarChar, 20)
                    .Value = memberId;

                connection.Open();
                command.ExecuteNonQuery();
            }

            LoadDashboard();
        }



        // =========================================================
        // AUTO COMPLETE PAST TRAINING SESSIONS
        // =========================================================

        private void AutoCompletePastSessions(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                UPDATE T
                SET T.Session_Status = 'Completed'
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Member_ID = @Member_ID
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) = 'Pending'
                  AND DATEADD(
                      SECOND,
                      DATEDIFF(
                          SECOND,
                          CAST('00:00:00' AS TIME),
                          TI.End_Time
                      ),
                      CAST(T.Session_Date AS DATETIME)
                  ) <= GETDATE();";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // UPDATE EXPIRED MEMBERSHIP STATUS
        // =========================================================

        private void UpdateExpiredMembershipStatus(
            SqlConnection connection,
            string memberId)
        {
            DateTime? expiry =
                CalculateMembershipExpiry(
                    connection,
                    memberId
                );

            if (!expiry.HasValue)
            {
                SetMemberStatus(
                    connection,
                    memberId,
                    "Inactive"
                );

                Session["MemberStatus"] = "Inactive";
                return;
            }

            if (expiry.Value.Date < DateTime.Today)
            {
                SetMemberStatus(
                    connection,
                    memberId,
                    "Inactive"
                );

                Session["MemberStatus"] = "Inactive";
                return;
            }

            SetMemberStatus(
                connection,
                memberId,
                "Active"
            );

            Session["MemberStatus"] = "Active";
        }


        // =========================================================
        // SET MEMBER STATUS
        // =========================================================

        private void SetMemberStatus(
            SqlConnection connection,
            string memberId,
            string status)
        {
            const string sql = @"
                UPDATE Member
                SET Member_Status = @Member_Status
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_Status",
                    SqlDbType.VarChar,
                    20
                ).Value = status;

                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // LOAD MEMBER INFORMATION
        // =========================================================

        private void LoadMemberInformation(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT
                    Member_FName,
                    Member_LName,
                    Member_Status
                FROM Member
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        lblWelcomeName.Text = "Member";
                        lblAccountStatus.Text = "UNKNOWN";
                        return;
                    }

                    string firstName =
                        Convert.ToString(
                            reader["Member_FName"]
                        ).Trim();

                    string lastName =
                        Convert.ToString(
                            reader["Member_LName"]
                        ).Trim();

                    lblWelcomeName.Text =
                        (firstName + " " + lastName).Trim();

                    if (string.IsNullOrWhiteSpace(
                        lblWelcomeName.Text))
                    {
                        lblWelcomeName.Text = "Member";
                    }

                    lblAccountStatus.Text =
                        Convert.ToString(
                            reader["Member_Status"]
                        ).Trim().ToUpperInvariant();
                }
            }
        }


        // =========================================================
        // LOAD SESSION STATISTICS
        // =========================================================

        private void LoadSessionStatistics(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT
                    SUM(
                        CASE
                            WHEN ISNULL(
                                Session_Status,
                                'Pending'
                            ) = 'Pending'
                            THEN 1
                            ELSE 0
                        END
                    ) AS Upcoming_Count,

                    SUM(
                        CASE
                            WHEN Session_Status = 'Completed'
                            THEN 1
                            ELSE 0
                        END
                    ) AS Completed_Count,

                    SUM(
                        CASE
                            WHEN Session_Status = 'Cancelled'
                            THEN 1
                            ELSE 0
                        END
                    ) AS Cancelled_Count
                FROM Training_Session
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        SetSessionStatisticsToZero();
                        return;
                    }

                    lblUpcomingSessions.Text =
                        GetCount(
                            reader["Upcoming_Count"]
                        ).ToString();

                    lblCompletedSessions.Text =
                        GetCount(
                            reader["Completed_Count"]
                        ).ToString();

                    lblCancelledSessions.Text =
                        GetCount(
                            reader["Cancelled_Count"]
                        ).ToString();
                }
            }
        }


        // =========================================================
        // LOAD MEMBERSHIP INFORMATION
        // =========================================================

        private void LoadMembershipInformation(
            SqlConnection connection,
            string memberId)
        {
            string memberStatus = "";

            const string statusSql = @"
                SELECT Member_Status
                FROM Member
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(statusSql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    memberStatus = result.ToString().Trim();
                }
            }

            DateTime? expiry =
                CalculateMembershipExpiry(
                    connection,
                    memberId
                );

            bool active =
                memberStatus.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase
                )
                && expiry.HasValue
                && expiry.Value.Date >= DateTime.Today;

            if (active)
            {
                lblMembershipStatus.Text = "ACTIVE";

                lblMembershipExpiry.Text =
                    "Valid until " +
                    expiry.Value.ToString("dd MMMM yyyy");
            }
            else
            {
                lblMembershipStatus.Text = "INACTIVE";

                if (expiry.HasValue)
                {
                    lblMembershipExpiry.Text =
                        "Expired " +
                        expiry.Value.ToString("dd MMMM yyyy");
                }
                else
                {
                    lblMembershipExpiry.Text =
                        "No membership payment found";
                }
            }
        }


        // =========================================================
        // CALCULATE MEMBERSHIP EXPIRY
        // Every successful payment extends membership by one month.
        // =========================================================

        private DateTime? CalculateMembershipExpiry(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT Payment_Date
                FROM Payment
                WHERE Member_ID = @Member_ID
                ORDER BY Payment_Date, Payment_ID;";

            DateTime? expiry = null;

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime paymentDate =
                            Convert.ToDateTime(
                                reader["Payment_Date"]
                            ).Date;

                        if (!expiry.HasValue ||
                            paymentDate > expiry.Value.Date)
                        {
                            expiry = paymentDate.AddMonths(1);
                        }
                        else
                        {
                            expiry = expiry.Value.AddMonths(1);
                        }
                    }
                }
            }

            return expiry;
        }


        // =========================================================
        // LOAD MEMBERSHIP NOTIFICATION
        // =========================================================

        private void LoadMembershipNotification(
            SqlConnection connection,
            string memberId)
        {
            string memberStatus =
                GetMemberStatus(connection, memberId);

            DateTime? expiry =
                CalculateMembershipExpiry(
                    connection,
                    memberId
                );

            if (!expiry.HasValue)
            {
                AddMembershipNotification(
                    "Membership Payment Required",
                    "Your membership is currently inactive because " +
                    "no membership payment has been recorded. " +
                    "Make a payment to activate your membership.",
                    "MAKE PAYMENT",
                    false
                );

                return;
            }

            if (expiry.Value.Date < DateTime.Today)
            {
                AddMembershipNotification(
                    "Your Membership Has Expired",
                    "Your membership expired on " +
                    expiry.Value.ToString("dd MMMM yyyy") +
                    ". Make a payment to reactivate your membership " +
                    "and continue using the gym services.",
                    "MAKE PAYMENT",
                    false
                );

                return;
            }

            if (memberStatus.Equals(
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
            {
                AddMembershipNotification(
                    "Your Membership Is Inactive",
                    "Your membership is currently inactive. " +
                    "Make a payment to reactivate your membership.",
                    "MAKE PAYMENT",
                    false
                );

                return;
            }

            int daysRemaining =
                (expiry.Value.Date - DateTime.Today).Days;

            if (daysRemaining == 0)
            {
                AddMembershipNotification(
                    "Your Membership Expires Today",
                    "Your membership expires today. Renew your " +
                    "membership now to avoid an interruption " +
                    "to your gym access.",
                    "RENEW MEMBERSHIP",
                    true
                );

                return;
            }

            if (daysRemaining >= 1 && daysRemaining <= 7)
            {
                string dayWord =
                    daysRemaining == 1 ? "day" : "days";

                AddMembershipNotification(
                    "Membership Expiring Soon",
                    "Your membership expires in " +
                    daysRemaining + " " + dayWord + ", on " +
                    expiry.Value.ToString("dd MMMM yyyy") +
                    ". Renew now to avoid an interruption " +
                    "to your membership.",
                    "RENEW MEMBERSHIP",
                    true
                );
            }
        }


        // =========================================================
        // GET MEMBER STATUS
        // =========================================================

        private string GetMemberStatus(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT Member_Status
                FROM Member
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                object result = command.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    return "";
                }

                return result.ToString().Trim();
            }
        }


        // =========================================================
        // CREATE MEMBERSHIP NOTIFICATION
        // =========================================================

        private void AddMembershipNotification(
            string title,
            string message,
            string buttonText,
            bool expiringSoon)
        {
            if (SessionReminderPanel == null ||
                SessionReminderPanel.Parent == null)
            {
                return;
            }

            string borderColour;
            string backgroundColour;
            string titleColour;

            if (expiringSoon)
            {
                borderColour = "rgba(255, 157, 0, 0.40)";
                backgroundColour = "rgba(255, 157, 0, 0.10)";
                titleColour = "#ff9d00";
            }
            else
            {
                borderColour = "rgba(255, 92, 92, 0.40)";
                backgroundColour = "rgba(255, 92, 92, 0.09)";
                titleColour = "#ff8a8a";
            }

            string safeTitle =
                HttpUtility.HtmlEncode(title);

            string safeMessage =
                HttpUtility.HtmlEncode(message);

            string safeButtonText =
                HttpUtility.HtmlEncode(buttonText);

            string paymentUrl =
                ResolveUrl("~/Member/MakePayment.aspx");

            string notificationHtml =
                @"
                <div style=""
                    width: 100%;
                    margin: 0 0 25px 0;
                    padding: 22px 24px;
                    border: 1px solid " + borderColour + @";
                    border-radius: 14px;
                    background: " + backgroundColour + @";
                    box-sizing: border-box;
                "">

                    <div style=""
                        display: flex;
                        align-items: center;
                        justify-content: space-between;
                        gap: 25px;
                        flex-wrap: wrap;
                    "">

                        <div style=""
                            flex: 1;
                            min-width: 250px;
                        "">

                            <div style=""
                                color: " + titleColour + @";
                                font-size: 11px;
                                font-weight: 800;
                                letter-spacing: 1.5px;
                                margin-bottom: 7px;
                            "">
                                MEMBERSHIP NOTICE
                            </div>

                            <div style=""
                                color: #ffffff;
                                font-size: 19px;
                                font-weight: 700;
                                margin-bottom: 7px;
                            "">
                                " + safeTitle + @"
                            </div>

                            <div style=""
                                color: #a9bdc4;
                                font-size: 13px;
                                line-height: 1.7;
                                max-width: 700px;
                            "">
                                " + safeMessage + @"
                            </div>

                        </div>

                        <div>
                            <a href=""" + paymentUrl + @"""
                                style=""
                                    display: inline-flex;
                                    align-items: center;
                                    justify-content: center;
                                    min-width: 165px;
                                    min-height: 46px;
                                    padding: 0 20px;
                                    border-radius: 8px;
                                    background: #ff9d00;
                                    color: #082f39;
                                    text-decoration: none;
                                    font-size: 11px;
                                    font-weight: 800;
                                    letter-spacing: 0.8px;
                                    box-sizing: border-box;
                                "">
                                " + safeButtonText + @"
                                &nbsp; →
                            </a>
                        </div>

                    </div>
                </div>";

            LiteralControl notification =
                new LiteralControl(notificationHtml);

            int reminderIndex =
                SessionReminderPanel.Parent.Controls.IndexOf(
                    SessionReminderPanel
                );

            if (reminderIndex >= 0)
            {
                SessionReminderPanel.Parent.Controls.AddAt(
                    reminderIndex,
                    notification
                );
            }
            else
            {
                SessionReminderPanel.Parent.Controls.Add(
                    notification
                );
            }
        }


        // =========================================================
        // LOAD UPCOMING SESSION REMINDER
        // =========================================================

        private void LoadSessionReminder(
            SqlConnection connection,
            string memberId)
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
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) = 'Pending'
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
                ORDER BY
                    T.Session_Date,
                    TI.Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        SessionReminderPanel.Visible = false;
                        return;
                    }

                    int trainingId =
                        Convert.ToInt32(reader["Training_ID"]);

                    object dismissedId =
                        Session["DismissedReminderTrainingID"];

                    if (dismissedId != null &&
                        Convert.ToString(dismissedId) ==
                        trainingId.ToString())
                    {
                        SessionReminderPanel.Visible = false;
                        return;
                    }

                    DateTime sessionDate =
                        Convert.ToDateTime(
                            reader["Session_Date"]
                        ).Date;

                    TimeSpan startTime =
                        (TimeSpan)reader["Start_Time"];

                    DateTime sessionStart =
                        sessionDate.Add(startTime);

                    TimeSpan remaining =
                        sessionStart - DateTime.Now;

                    ViewState["ReminderTrainingID"] = trainingId;

                    lblReminderTitle.Text =
                        reader["Session_Type"] +
                        " with " +
                        reader["Trainer_Name"];

                    lblReminderMessage.Text =
                        BuildReminderMessage(
                            sessionStart,
                            remaining
                        );

                    SessionReminderPanel.Visible = true;
                }
            }
        }


        // =========================================================
        // BUILD SESSION REMINDER MESSAGE
        // =========================================================

        private static string BuildReminderMessage(
            DateTime sessionStart,
            TimeSpan remaining)
        {
            string remainingText;

            if (remaining.TotalHours < 1)
            {
                int minutes =
                    Math.Max(
                        1,
                        Convert.ToInt32(
                            Math.Ceiling(
                                remaining.TotalMinutes
                            )
                        )
                    );

                remainingText =
                    minutes +
                    (minutes == 1 ? " minute" : " minutes");
            }
            else
            {
                int hours =
                    Convert.ToInt32(
                        Math.Floor(
                            remaining.TotalHours
                        )
                    );

                int minutes = remaining.Minutes;

                remainingText =
                    hours +
                    (hours == 1 ? " hour" : " hours");

                if (minutes > 0)
                {
                    remainingText +=
                        " and " +
                        minutes +
                        (minutes == 1 ? " minute" : " minutes");
                }
            }

            return
                "Your session begins on " +
                sessionStart.ToString("dd MMMM yyyy 'at' HH:mm") +
                ", in approximately " +
                remainingText +
                ".";
        }


        // =========================================================
        // DISMISS SESSION REMINDER
        // =========================================================

        protected void btnDismissReminder_Click(
            object sender,
            EventArgs e)
        {
            if (ViewState["ReminderTrainingID"] != null)
            {
                Session["DismissedReminderTrainingID"] =
                    ViewState["ReminderTrainingID"].ToString();
            }

            SessionReminderPanel.Visible = false;
            ReminderUpdatePanel.Update();
        }


        // =========================================================
        // GET STATISTIC COUNT
        // =========================================================

        private static int GetCount(object value)
        {
            return value == null || value == DBNull.Value
                ? 0
                : Convert.ToInt32(value);
        }


        // =========================================================
        // SET SESSION STATISTICS TO ZERO
        // =========================================================

        private void SetSessionStatisticsToZero()
        {
            lblUpcomingSessions.Text = "0";
            lblCompletedSessions.Text = "0";
            lblCancelledSessions.Text = "0";
        }
    }
}