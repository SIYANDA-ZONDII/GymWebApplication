using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Manager
{
    public partial class ManagerDashboard : Page
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
            if (!IsManagerLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private bool IsManagerLoggedIn()
        {
            if (!Request.IsAuthenticated)
                return false;

            if (Context.User != null &&
                Context.User.IsInRole("Manager"))
                return true;

            return Session["UserRole"] != null &&
                   Session["UserRole"].ToString().Trim()
                       .Equals("Manager",
                           StringComparison.OrdinalIgnoreCase);
        }

        private void LoadDashboard()
        {
            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    AutoCompletePastSessions(connection);
                    LoadManagerName(connection);
                    LoadStatistics(connection);
                    LoadManagerNotifications(
                        connection,
                        GetManagerUserId(connection));
                    LoadTodaySessions(connection);
                }
            }
            catch (Exception)
            {
                lblManagerName.Text = "Manager";

                lblMemberCount.Text = "0";
                lblActiveMemberCount.Text = "0";
                lblTrainerCount.Text = "0";
                lblAvailableTrainerCount.Text = "0";
                lblTodaySessionCount.Text = "0";
                lblUpcomingSessionCount.Text = "0";
                lblCompletedSessionCount.Text = "0";
                lblCancelledSessionCount.Text = "0";
                lblRevenue.Text = "0.00";

                lblNotificationCount.Text = "0 unread";
                lblNotificationEmpty.Text =
                    "Notifications could not be loaded.";
                lblNotificationEmpty.Visible = true;

                rptNotifications.DataSource = new DataTable();
                rptNotifications.DataBind();

                TodaySessionsGrid.DataSource = new DataTable();
                TodaySessionsGrid.DataBind();
            }
        }

        private string GetManagerUserId(
            SqlConnection connection)
        {
            string staffId = Session["StaffID"] == null
                ? string.Empty
                : Session["StaffID"].ToString().Trim();

            if (!string.IsNullOrWhiteSpace(staffId))
                return staffId;

            const string sql = @"
                SELECT Staff_ID
                FROM Staff
                WHERE Staff_Email = @Email
                  AND Staff_Role = 'Manager';";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email", SqlDbType.VarChar, 100)
                    .Value = User.Identity.Name;

                object result = command.ExecuteScalar();

                staffId = result == null ||
                          result == DBNull.Value
                    ? string.Empty
                    : Convert.ToString(result).Trim();
            }

            if (!string.IsNullOrWhiteSpace(staffId))
                Session["StaffID"] = staffId;

            return staffId;
        }

        private void LoadManagerNotifications(
            SqlConnection connection,
            string userId)
        {
            DataTable notifications = new DataTable();

            if (string.IsNullOrWhiteSpace(userId))
            {
                rptNotifications.DataSource = notifications;
                rptNotifications.DataBind();

                lblNotificationCount.Text = "0 unread";
                lblNotificationEmpty.Text =
                    "No manager notifications are available.";
                lblNotificationEmpty.Visible = true;
                return;
            }

            const string sql = @"
                SELECT
                    N.Notification_ID,
                    N.Title,
                    N.Message,
                    N.Created_Date AS Created_At,
                    CASE
                        WHEN LOWER(
                            LTRIM(RTRIM(
                                CONVERT(VARCHAR(10), T.Is_Read)
                            ))
                        ) IN ('1', 'true', 'yes')
                        THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS Is_Read
                FROM dbo.Notifications AS N
                INNER JOIN dbo.Notification_Target AS T
                    ON T.Notification_ID = N.Notification_ID
                WHERE LTRIM(RTRIM(T.User_ID)) = @User_ID
                  AND N.Created_Date >=
                      DATEADD(DAY, -7, SYSDATETIME())
                ORDER BY N.Created_Date DESC;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@User_ID", SqlDbType.VarChar, 9)
                    .Value = userId.Trim();

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
                    unreadCount++;
            }

            lblNotificationCount.Text =
                unreadCount + " unread";

            lblNotificationEmpty.Text =
                "You have no announcements at the moment.";

            lblNotificationEmpty.Visible =
                notifications.Rows.Count == 0;
        }

        protected void rptNotifications_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (!string.Equals(
                e.CommandName,
                "MarkRead",
                StringComparison.OrdinalIgnoreCase))
                return;

            if (!IsManagerLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            int notificationId;

            if (!Int32.TryParse(
                Convert.ToString(e.CommandArgument),
                out notificationId))
                return;

            string userId = Session["StaffID"] == null
                ? string.Empty
                : Session["StaffID"].ToString().Trim();

            if (string.IsNullOrWhiteSpace(userId))
                return;

            const string sql = @"
                UPDATE T
                SET
                    T.Is_Read = '1',
                    T.Read_At = CAST(GETDATE() AS TIME)
                FROM dbo.Notification_Target AS T
                WHERE T.Notification_ID = @Notification_ID
                  AND LTRIM(RTRIM(T.User_ID)) = @User_ID
                  AND ISNULL(
                      LOWER(LTRIM(RTRIM(
                          CONVERT(VARCHAR(10), T.Is_Read)
                      ))),
                      '0'
                  ) NOT IN ('1', 'true', 'yes');";

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Notification_ID", SqlDbType.Int)
                    .Value = notificationId;

                command.Parameters.Add(
                    "@User_ID", SqlDbType.VarChar, 9)
                    .Value = userId;

                connection.Open();
                command.ExecuteNonQuery();
            }

            LoadDashboard();
        }

        private void AutoCompletePastSessions(
            SqlConnection connection)
        {
            const string sql = @"
                UPDATE T
                SET T.Session_Status = 'Completed'
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE ISNULL(T.Session_Status, 'Pending')
                      = 'Pending'
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
                command.ExecuteNonQuery();
            }
        }

        private void LoadManagerName(
            SqlConnection connection)
        {
            string firstName =
                Session["ManagerFirstName"] == null
                ? ""
                : Session["ManagerFirstName"].ToString().Trim();

            string lastName =
                Session["ManagerLastName"] == null
                ? ""
                : Session["ManagerLastName"].ToString().Trim();

            if (!string.IsNullOrWhiteSpace(firstName) ||
                !string.IsNullOrWhiteSpace(lastName))
            {
                lblManagerName.Text =
                    (firstName + " " + lastName).Trim();
                return;
            }

            const string sql = @"
                SELECT Staff_FName, Staff_LName
                FROM Staff
                WHERE Staff_Email = @Email
                  AND Staff_Role = 'Manager';";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Email", SqlDbType.VarChar, 100)
                    .Value = User.Identity.Name;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        lblManagerName.Text = "Manager";
                        return;
                    }

                    firstName =
                        Convert.ToString(reader["Staff_FName"]).Trim();

                    lastName =
                        Convert.ToString(reader["Staff_LName"]).Trim();

                    lblManagerName.Text =
                        (firstName + " " + lastName).Trim();

                    if (string.IsNullOrWhiteSpace(lblManagerName.Text))
                        lblManagerName.Text = "Manager";
                }
            }
        }

        private void LoadStatistics(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT
                    (SELECT COUNT(*)
                     FROM Member) AS Member_Count,

                    (SELECT COUNT(*)
                     FROM Member
                     WHERE Member_Status = 'Active')
                     AS Active_Member_Count,

                    (SELECT COUNT(*)
                     FROM Staff
                     WHERE Staff_Role = 'Trainer')
                     AS Trainer_Count,

                    (SELECT COUNT(DISTINCT Staff_ID)
                     FROM Trainer_Schedule
                     WHERE Availability = 'Available'
                       AND (
                           Session_Date > CAST(GETDATE() AS DATE)
                           OR (
                               Session_Date = CAST(GETDATE() AS DATE)
                               AND End_Time > CAST(GETDATE() AS TIME)
                           )
                       )) AS Available_Trainer_Count,

                    (SELECT COUNT(DISTINCT
                         CAST(T.Session_ID AS VARCHAR(20))
                         + '|' + CAST(T.Staff_ID AS VARCHAR(20))
                         + '|' + CAST(T.Time_ID AS VARCHAR(20)))
                     FROM Training_Session AS T
                     WHERE T.Session_Date = CAST(GETDATE() AS DATE)
                       AND ISNULL(T.Session_Status, 'Pending')
                           <> 'Cancelled')
                     AS Today_Session_Count,

                    (SELECT COUNT(DISTINCT
                         CAST(T.Session_ID AS VARCHAR(20))
                         + '|' + CAST(T.Staff_ID AS VARCHAR(20))
                         + '|' + CONVERT(VARCHAR(8), T.Session_Date, 112)
                         + '|' + CAST(T.Time_ID AS VARCHAR(20)))
                     FROM Training_Session AS T
                     WHERE ISNULL(T.Session_Status, 'Pending') = 'Pending')
                     AS Upcoming_Session_Count,

                    (SELECT COUNT(DISTINCT
                         CAST(T.Session_ID AS VARCHAR(20))
                         + '|' + CAST(T.Staff_ID AS VARCHAR(20))
                         + '|' + CONVERT(VARCHAR(8), T.Session_Date, 112)
                         + '|' + CAST(T.Time_ID AS VARCHAR(20)))
                     FROM Training_Session AS T
                     WHERE T.Session_Status = 'Completed')
                     AS Completed_Session_Count,

                    (SELECT COUNT(DISTINCT
                         CAST(T.Session_ID AS VARCHAR(20))
                         + '|' + CAST(T.Staff_ID AS VARCHAR(20))
                         + '|' + CONVERT(VARCHAR(8), T.Session_Date, 112)
                         + '|' + CAST(T.Time_ID AS VARCHAR(20)))
                     FROM Training_Session AS T
                     WHERE T.Session_Status = 'Cancelled')
                     AS Cancelled_Session_Count,

                    (SELECT ISNULL(
                         SUM(CAST(Payment_Balance AS DECIMAL(18, 2))),
                         0)
                     FROM Payment) AS Recorded_Revenue;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.Read())
                    return;

                lblMemberCount.Text =
                    GetInt(reader["Member_Count"]).ToString();

                lblActiveMemberCount.Text =
                    GetInt(reader["Active_Member_Count"]).ToString();

                lblTrainerCount.Text =
                    GetInt(reader["Trainer_Count"]).ToString();

                lblAvailableTrainerCount.Text =
                    GetInt(reader["Available_Trainer_Count"]).ToString();

                lblTodaySessionCount.Text =
                    GetInt(reader["Today_Session_Count"]).ToString();

                lblUpcomingSessionCount.Text =
                    GetInt(reader["Upcoming_Session_Count"]).ToString();

                lblCompletedSessionCount.Text =
                    GetInt(reader["Completed_Session_Count"]).ToString();

                lblCancelledSessionCount.Text =
                    GetInt(reader["Cancelled_Session_Count"]).ToString();

                lblRevenue.Text =
                    GetDecimal(reader["Recorded_Revenue"])
                    .ToString("N2");
            }
        }

        private void LoadTodaySessions(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT
                    S.Session_Type,
                    ST.Staff_FName + ' ' + ST.Staff_LName
                        AS Trainer_Name,
                    TI.Start_Time,
                    TI.End_Time,
                    COUNT(T.Member_ID) AS Member_Count,
                    MAX(T.Session_Status) AS Session_Status
                FROM Training_Session AS T
                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID
                INNER JOIN Staff AS ST
                    ON ST.Staff_ID = T.Staff_ID
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Session_Date = CAST(GETDATE() AS DATE)
                GROUP BY
                    S.Session_Type,
                    ST.Staff_FName,
                    ST.Staff_LName,
                    TI.Start_Time,
                    TI.End_Time
                ORDER BY TI.Start_Time;";

            DataTable table = new DataTable();

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            using (SqlDataAdapter adapter =
                   new SqlDataAdapter(command))
            {
                adapter.Fill(table);
            }

            TodaySessionsGrid.DataSource = table;
            TodaySessionsGrid.DataBind();
        }

        private int GetInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            return Convert.ToInt32(value);
        }

        private decimal GetDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            return Convert.ToDecimal(value);
        }
    }
}