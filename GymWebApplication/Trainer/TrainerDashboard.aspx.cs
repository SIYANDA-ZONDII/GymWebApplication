
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication.Trainer
{
    public partial class TrainerDashboard : Page
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
            if (!IsTrainerLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private bool IsTrainerLoggedIn()
        {
            return Request.IsAuthenticated
                && Session["UserRole"] != null
                && Session["UserRole"]
                    .ToString()
                    .Trim()
                    .Equals(
                        "Trainer",
                        StringComparison.OrdinalIgnoreCase
                    );
        }

        private void LoadDashboard()
        {
            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    TrainerDetails trainer =
                        GetTrainerDetails(connection);

                    if (trainer == null)
                    {
                        lblTrainerName.Text = "Trainer";
                        lblTrainerEmail.Text = User.Identity.Name;
                        lblTrainerStatus.Text = "NOT FOUND";

                        SetStatisticsToZero();
                        NextSessionPanel.Visible = false;

                        rptNotifications.DataSource = null;
                        rptNotifications.DataBind();
                        lblNotificationCount.Text = "0 unread";
                        lblNotificationEmpty.Text =
                            "Trainer details could not be found.";
                        lblNotificationEmpty.Visible = true;

                        return;
                    }

                    lblTrainerName.Text = trainer.FullName;
                    lblTrainerEmail.Text = trainer.Email;
                    lblTrainerStatus.Text =
                        (trainer.Status ?? "Unknown").ToUpperInvariant();

                    AutoCompletePastSessions(
                        connection,
                        trainer.StaffId
                    );

                    LoadStatistics(
                        connection,
                        trainer.StaffId
                    );

                    LoadAvailability(
                        connection,
                        trainer.StaffId
                    );

                    LoadNextSession(
                        connection,
                        trainer.StaffId
                    );

                    LoadTrainerNotifications(
                        connection,
                        trainer.StaffId
                    );
                }
            }
            catch (Exception)
            {
                SetStatisticsToZero();

                lblScheduleStatus.Text = "UNAVAILABLE";
                NextSessionPanel.Visible = false;

                rptNotifications.DataSource = null;
                rptNotifications.DataBind();
                lblNotificationCount.Text = "Unavailable";
                lblNotificationEmpty.Text =
                    "Notifications could not be loaded. Please try again later.";
                lblNotificationEmpty.Visible = true;
            }
        }

        // =========================================================
        // LOAD ANNOUNCEMENTS TARGETED TO THIS TRAINER
        // =========================================================

        private void LoadTrainerNotifications(
            SqlConnection connection,
            string staffId)
        {
            const string sql = @"
                SELECT
                    N.Notification_ID,
                    N.Title,
                    N.Message,
                    CONVERT(VARCHAR(8), N.Created_At, 108) AS Created_At,
                    CASE
                        WHEN LOWER(LTRIM(RTRIM(
                            CONVERT(VARCHAR(10), T.Is_Read)
                        ))) IN ('1', 'true', 'yes')
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS Is_Read
                FROM dbo.Notifications AS N
                INNER JOIN dbo.Notification_Target AS T
                    ON T.Notification_ID = N.Notification_ID
                WHERE LTRIM(RTRIM(T.User_ID)) = @User_ID
                ORDER BY N.Notification_ID DESC;";

            DataTable notifications = new DataTable();

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@User_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId.Trim();

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
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

            lblNotificationCount.Text =
                unreadCount + " unread";

            lblNotificationEmpty.Text =
                "You have no announcements at the moment.";

            lblNotificationEmpty.Visible =
                notifications.Rows.Count == 0;
        }

        // =========================================================
        // MARK A TRAINER'S NOTIFICATION AS READ
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

            if (!IsTrainerLoggedIn())
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

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                connection.Open();

                TrainerDetails trainer =
                    GetTrainerDetails(connection);

                if (trainer == null)
                {
                    return;
                }

                const string sql = @"
                    UPDATE dbo.Notification_Target
                    SET Is_Read = 1,
                        Read_At = SYSDATETIME()
                    WHERE Notification_ID = @Notification_ID
                      AND LTRIM(RTRIM(User_ID)) = @User_ID
                      AND LOWER(LTRIM(RTRIM(
                          CONVERT(VARCHAR(10), Is_Read)
                      ))) NOT IN ('1', 'true', 'yes');";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@Notification_ID",
                        SqlDbType.Int
                    ).Value = notificationId;

                    command.Parameters.Add(
                        "@User_ID",
                        SqlDbType.VarChar,
                        20
                    ).Value = trainer.StaffId.Trim();

                    command.ExecuteNonQuery();
                }
            }

            LoadDashboard();
        }

        private TrainerDetails GetTrainerDetails(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT
                    Staff_ID,
                    Staff_FName,
                    Staff_LName,
                    Staff_Email,
                    Staff_Status
                FROM Staff
                WHERE Staff_Email = @Email
                  AND Staff_Role = 'Trainer';";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email",
                    SqlDbType.VarChar,
                    100
                ).Value = User.Identity.Name;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string firstName =
                        Convert.ToString(reader["Staff_FName"]).Trim();

                    string lastName =
                        Convert.ToString(reader["Staff_LName"]).Trim();

                    string fullName =
                        (firstName + " " + lastName).Trim();

                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        fullName = "Trainer";
                    }

                    return new TrainerDetails
                    {
                        StaffId = reader["Staff_ID"].ToString(),
                        FullName = fullName,
                        Email = Convert.ToString(reader["Staff_Email"]),
                        Status = Convert.ToString(reader["Staff_Status"])
                    };
                }
            }
        }

        // =========================================================
        // COMPLETE PAST TRAINING SESSIONS
        // =========================================================

        private void AutoCompletePastSessions(
            SqlConnection connection,
            string staffId)
        {
            const string sql = @"
                UPDATE T
                SET T.Session_Status = 'Completed'
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @Staff_ID
                  AND ISNULL(T.Session_Status, 'Pending') = 'Pending'
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
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // TRAINER STATISTICS
        // =========================================================

        private void LoadStatistics(
            SqlConnection connection,
            string staffId)
        {
            const string sql = @"
                SELECT
                    SUM(CASE
                        WHEN T.Session_Date = CAST(GETDATE() AS DATE)
                         AND T.Session_Status = 'Pending'
                        THEN 1 ELSE 0 END) AS TodaySessions,

                    SUM(CASE
                        WHEN T.Session_Date >= CAST(GETDATE() AS DATE)
                         AND T.Session_Status = 'Pending'
                        THEN 1 ELSE 0 END) AS UpcomingSessions,

                    SUM(CASE
                        WHEN T.Session_Status = 'Completed'
                        THEN 1 ELSE 0 END) AS CompletedSessions,

                    COUNT(DISTINCT T.Member_ID) AS MemberCount
                FROM Training_Session AS T
                WHERE T.Staff_ID = @Staff_ID;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblTodaySessions.Text =
                            GetCount(reader["TodaySessions"]).ToString();

                        lblUpcomingSessions.Text =
                            GetCount(reader["UpcomingSessions"]).ToString();

                        lblCompletedSessions.Text =
                            GetCount(reader["CompletedSessions"]).ToString();

                        lblMemberCount.Text =
                            GetCount(reader["MemberCount"]).ToString();
                    }
                }
            }
        }

        // =========================================================
        // TRAINER AVAILABILITY
        // =========================================================

        private void LoadAvailability(
            SqlConnection connection,
            string staffId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Trainer_Schedule
                WHERE Staff_ID = @Staff_ID
                  AND Schedule_Date >= CAST(GETDATE() AS DATE);";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                int count = Convert.ToInt32(command.ExecuteScalar());

                lblScheduleStatus.Text =
                    count > 0 ? count + " AVAILABLE" : "NOT SET";
            }
        }

        // =========================================================
        // NEXT TRAINING SESSION
        // =========================================================

        private void LoadNextSession(
            SqlConnection connection,
            string staffId)
        {
            const string sql = @"
                SELECT TOP 1
                    S.Session_Type,
                    T.Session_Date,
                    TI.Start_Time,
                    TI.End_Time,
                    COUNT(*) AS Member_Count
                FROM Training_Session AS T
                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @Staff_ID
                  AND T.Session_Status = 'Pending'
                  AND DATEADD(
                      SECOND,
                      DATEDIFF(
                          SECOND,
                          CAST('00:00:00' AS TIME),
                          TI.Start_Time
                      ),
                      CAST(T.Session_Date AS DATETIME)
                  ) > GETDATE()
                GROUP BY
                    S.Session_Type,
                    T.Session_Date,
                    TI.Start_Time,
                    TI.End_Time
                ORDER BY
                    T.Session_Date,
                    TI.Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        NextSessionPanel.Visible = false;
                        return;
                    }

                    string sessionType =
                        Convert.ToString(reader["Session_Type"]);

                    DateTime sessionDate =
                        Convert.ToDateTime(reader["Session_Date"]);

                    TimeSpan start =
                        (TimeSpan)reader["Start_Time"];

                    TimeSpan end =
                        (TimeSpan)reader["End_Time"];

                    int memberCount =
                        GetCount(reader["Member_Count"]);

                    lblNextSessionTitle.Text = sessionType;

                    lblNextSessionDetails.Text =
                        sessionDate.ToString("dd MMM yyyy")
                        + " · "
                        + start.ToString(@"hh\:mm")
                        + " - "
                        + end.ToString(@"hh\:mm")
                        + " · "
                        + memberCount
                        + (memberCount == 1
                            ? " member"
                            : " members");

                    NextSessionPanel.Visible = true;
                }
            }
        }

        private static int GetCount(object value)
        {
            return value == null || value == DBNull.Value
                ? 0
                : Convert.ToInt32(value);
        }

        private void SetStatisticsToZero()
        {
            lblTodaySessions.Text = "0";
            lblUpcomingSessions.Text = "0";
            lblCompletedSessions.Text = "0";
            lblMemberCount.Text = "0";
        }

        private sealed class TrainerDetails
        {
            public string StaffId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
        }
    }
}
