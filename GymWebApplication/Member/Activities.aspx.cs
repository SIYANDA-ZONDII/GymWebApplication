using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Member
{
    public partial class Activities : Page
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
            if (!IsMemberLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtDate.Attributes["min"] =
                    DateTime.Today.ToString("yyyy-MM-dd");

                LoadSessionTypes();
                LoadTrainers();
                LoadTimePlaceholder();
                LoadActivities();
            }
        }


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


        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
            LoadActivities();
        }


        protected void ddlStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
            LoadActivities();
        }


        protected void btnClearFilters_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedValue = "All";

            ClearMessage();
            LoadActivities();
        }


        private void LoadActivities()
        {
            string memberId =
                Session["MemberID"].ToString().Trim();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    AutoCompletePastSessions(
                        connection,
                        memberId
                    );

                    DataTable activities =
                        GetMemberActivities(
                            connection,
                            memberId,
                            txtSearch.Text.Trim(),
                            ddlStatus.SelectedValue
                        );

                    ActivitiesGrid.DataSource =
                        activities;

                    ActivitiesGrid.DataBind();

                    LoadSummary(
                        connection,
                        memberId
                    );

                    lblActivityNotice.Text =
                        activities.Rows.Count
                        + (activities.Rows.Count == 1
                            ? " session matches the selected filters."
                            : " sessions match the selected filters.");
                }
            }
            catch (Exception)
            {
                ShowActivityError(
                    "Your training history could not be loaded. "
                    + "Please refresh the page and try again."
                );
            }
        }


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
                  AND DATEADD
                  (
                      SECOND,
                      DATEDIFF
                      (
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


        private DataTable GetMemberActivities(
            SqlConnection connection,
            string memberId,
            string search,
            string status)
        {
            DataTable table = new DataTable();

            const string sql = @"
                SELECT
                    T.Training_ID,
                    T.Session_ID,
                    T.Staff_ID,
                    T.Time_ID,
                    S.Session_Type,
                    T.Session_Date,

                    CONVERT(
                        VARCHAR(5),
                        TI.Start_Time,
                        108
                    ) AS Start_Time,

                    CONVERT(
                        VARCHAR(5),
                        TI.End_Time,
                        108
                    ) AS End_Time,

                    T.Session_Duration,

                    ISNULL(
                        ST.Staff_FName + ' ' +
                        ST.Staff_LName,
                        'Not assigned'
                    ) AS Trainer_Name,

                    ISNULL(
                        T.Member_Attendance,
                        0
                    ) AS Member_Attendance,

                    ISNULL(
                        T.Session_Status,
                        'Pending'
                    ) AS Session_Status

                FROM Training_Session AS T

                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID

                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID

                LEFT JOIN Staff AS ST
                    ON ST.Staff_ID = T.Staff_ID

                WHERE T.Member_ID = @Member_ID

                  AND
                  (
                      @Status = 'All'
                      OR ISNULL(
                          T.Session_Status,
                          'Pending'
                      ) = @Status
                  )

                  AND
                  (
                      @Search = ''
                      OR S.Session_Type LIKE
                         '%' + @Search + '%'
                      OR ST.Staff_FName LIKE
                         '%' + @Search + '%'
                      OR ST.Staff_LName LIKE
                         '%' + @Search + '%'
                      OR
                      (
                          ST.Staff_FName + ' ' +
                          ST.Staff_LName
                      ) LIKE '%' + @Search + '%'
                  )

                ORDER BY
                    CASE
                        WHEN ISNULL(
                            T.Session_Status,
                            'Pending'
                        ) = 'Pending'
                            THEN 0
                        WHEN T.Session_Status = 'Completed'
                            THEN 1
                        ELSE 2
                    END,
                    T.Session_Date DESC,
                    TI.Start_Time DESC,
                    T.Training_ID DESC;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.Parameters.Add(
                    "@Search",
                    SqlDbType.VarChar,
                    100
                ).Value = search ?? "";

                command.Parameters.Add(
                    "@Status",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    string.IsNullOrWhiteSpace(status)
                    ? "All"
                    : status;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }


        private void LoadSummary(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT
                    SUM
                    (
                        CASE
                            WHEN ISNULL(
                                Session_Status,
                                'Pending'
                            ) = 'Pending'
                                THEN 1
                            ELSE 0
                        END
                    ) AS Upcoming_Count,

                    SUM
                    (
                        CASE
                            WHEN Session_Status =
                                 'Completed'
                                THEN 1
                            ELSE 0
                        END
                    ) AS Completed_Count,

                    SUM
                    (
                        CASE
                            WHEN Session_Status =
                                 'Cancelled'
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
                        SetSummaryToZero();
                        return;
                    }

                    lblUpcomingCount.Text =
                        GetCount(
                            reader["Upcoming_Count"]
                        ).ToString();

                    lblCompletedCount.Text =
                        GetCount(
                            reader["Completed_Count"]
                        ).ToString();

                    lblCancelledCount.Text =
                        GetCount(
                            reader["Cancelled_Count"]
                        ).ToString();
                }
            }
        }


        private static int GetCount(object value)
        {
            return value == null ||
                   value == DBNull.Value
                ? 0
                : Convert.ToInt32(value);
        }


        private void SetSummaryToZero()
        {
            lblUpcomingCount.Text = "0";
            lblCompletedCount.Text = "0";
            lblCancelledCount.Text = "0";
        }


        protected bool CanCancelBooking(
            object sessionDateValue,
            object startTimeValue,
            object statusValue)
        {
            string status =
                Convert.ToString(statusValue).Trim();

            if (!status.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            DateTime sessionDate;
            TimeSpan startTime;

            if (!DateTime.TryParse(
                    Convert.ToString(sessionDateValue),
                    out sessionDate)
                || !TimeSpan.TryParse(
                    Convert.ToString(startTimeValue),
                    out startTime))
            {
                return false;
            }

            return sessionDate.Date.Add(startTime)
                   > DateTime.Now;
        }


        protected void ActivitiesGrid_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals(
                    "CancelBooking",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            int trainingId;

            if (!int.TryParse(
                    Convert.ToString(e.CommandArgument),
                    out trainingId))
            {
                ShowActivityError(
                    "The selected booking is invalid."
                );

                return;
            }

            try
            {
                CancelMemberBooking(
                    trainingId,
                    Session["MemberID"]
                        .ToString()
                        .Trim()
                );
            }
            catch (Exception)
            {
                ShowActivityError(
                    "The booking could not be cancelled. "
                    + "Please refresh the page and try again."
                );
            }
        }


        private void CancelMemberBooking(
            int trainingId,
            string memberId)
        {
            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction =
                       connection.BeginTransaction(
                           IsolationLevel.Serializable))
                {
                    try
                    {
                        BookingDetails booking =
                            GetBookingDetails(
                                connection,
                                transaction,
                                trainingId,
                                memberId
                            );

                        if (booking == null)
                        {
                            transaction.Rollback();

                            ShowActivityError(
                                "This booking was not found."
                            );

                            return;
                        }

                        if (!booking.Status.Equals(
                                "Pending",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            transaction.Rollback();

                            ShowActivityError(
                                "Only pending bookings can be cancelled."
                            );

                            return;
                        }

                        DateTime sessionStart =
                            booking.SessionDate.Date.Add(
                                booking.StartTime
                            );

                        if (sessionStart <= DateTime.Now)
                        {
                            transaction.Rollback();

                            ShowActivityError(
                                "The session has already started "
                                + "and can no longer be cancelled."
                            );

                            LoadActivities();
                            return;
                        }

                        int affected =
                            MarkBookingCancelled(
                                connection,
                                transaction,
                                trainingId,
                                memberId
                            );

                        if (affected != 1)
                        {
                            throw new InvalidOperationException(
                                "The booking changed before cancellation."
                            );
                        }

                        int remaining =
                            CountRemainingMembers(
                                connection,
                                transaction,
                                booking.SessionId,
                                booking.StaffId,
                                booking.SessionDate,
                                booking.TimeId
                            );

                        UpdateMatchingAttendance(
                            connection,
                            transaction,
                            booking.SessionId,
                            booking.StaffId,
                            booking.SessionDate,
                            booking.TimeId,
                            remaining
                        );

                        transaction.Commit();

                        LoadActivities();

                        ShowActivitySuccess(
                            booking.SessionType
                            + " with "
                            + booking.TrainerName
                            + " on "
                            + booking.SessionDate.ToString(
                                "dd MMMM yyyy"
                            )
                            + " at "
                            + booking.StartTime.ToString(
                                @"hh\:mm"
                            )
                            + " was cancelled successfully."
                        );
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        private BookingDetails GetBookingDetails(
            SqlConnection connection,
            SqlTransaction transaction,
            int trainingId,
            string memberId)
        {
            const string sql = @"
                SELECT
                    T.Session_ID,
                    T.Staff_ID,
                    T.Time_ID,
                    T.Session_Date,
                    ISNULL(
                        T.Session_Status,
                        'Pending'
                    ) AS Session_Status,
                    TI.Start_Time,
                    S.Session_Type,

                    ISNULL(
                        ST.Staff_FName + ' ' +
                        ST.Staff_LName,
                        'the selected trainer'
                    ) AS Trainer_Name

                FROM Training_Session AS T WITH (UPDLOCK)

                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID

                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID

                LEFT JOIN Staff AS ST
                    ON ST.Staff_ID = T.Staff_ID

                WHERE T.Training_ID = @Training_ID
                  AND T.Member_ID = @Member_ID;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Training_ID",
                    SqlDbType.Int
                ).Value = trainingId;

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
                        return null;
                    }

                    return new BookingDetails
                    {
                        SessionId =
                            Convert.ToInt32(
                                reader["Session_ID"]
                            ),

                        StaffId =
                            reader["Staff_ID"].ToString(),

                        TimeId =
                            reader["Time_ID"].ToString(),

                        SessionDate =
                            Convert.ToDateTime(
                                reader["Session_Date"]
                            ).Date,

                        StartTime =
                            (TimeSpan)reader["Start_Time"],

                        Status =
                            reader["Session_Status"].ToString(),

                        SessionType =
                            reader["Session_Type"].ToString(),

                        TrainerName =
                            reader["Trainer_Name"].ToString()
                    };
                }
            }
        }


        private int MarkBookingCancelled(
            SqlConnection connection,
            SqlTransaction transaction,
            int trainingId,
            string memberId)
        {
            const string sql = @"
                UPDATE Training_Session
                SET Session_Status = 'Cancelled'
                WHERE Training_ID = @Training_ID
                  AND Member_ID = @Member_ID
                  AND ISNULL(
                      Session_Status,
                      'Pending'
                  ) = 'Pending';";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Training_ID",
                    SqlDbType.Int
                ).Value = trainingId;

                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                return command.ExecuteNonQuery();
            }
        }


        private int CountRemainingMembers(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Training_Session
                WHERE Session_ID = @Session_ID
                  AND Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                  AND Time_ID = @Time_ID
                  AND ISNULL(
                      Session_Status,
                      'Pending'
                  ) <> 'Cancelled';";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                AddGroupParameters(
                    command,
                    sessionId,
                    staffId,
                    sessionDate,
                    timeId
                );

                return Convert.ToInt32(
                    command.ExecuteScalar()
                );
            }
        }


        private void UpdateMatchingAttendance(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId,
            int attendance)
        {
            const string sql = @"
                UPDATE Training_Session
                SET Member_Attendance =
                    @Member_Attendance
                WHERE Session_ID = @Session_ID
                  AND Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                  AND Time_ID = @Time_ID;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Member_Attendance",
                    SqlDbType.Int
                ).Value = attendance;

                AddGroupParameters(
                    command,
                    sessionId,
                    staffId,
                    sessionDate,
                    timeId
                );

                command.ExecuteNonQuery();
            }
        }


        private static void AddGroupParameters(
            SqlCommand command,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId)
        {
            command.Parameters.Add(
                "@Session_ID",
                SqlDbType.Int
            ).Value = sessionId;

            command.Parameters.Add(
                "@Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = staffId;

            command.Parameters.Add(
                "@Session_Date",
                SqlDbType.Date
            ).Value = sessionDate.Date;

            command.Parameters.Add(
                "@Time_ID",
                SqlDbType.VarChar,
                20
            ).Value = timeId;
        }


        protected string GetStatusClass(object value)
        {
            string status =
                Convert.ToString(value).Trim();

            if (status.Equals(
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "activity-status activity-status-completed";
            }

            if (status.Equals(
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "activity-status activity-status-cancelled";
            }

            return "activity-status activity-status-upcoming";
        }


        private void ClearMessage()
        {
            ActivitiesMessage.Text = "";
            ActivitiesMessage.CssClass =
                "activities-message";
        }


        private void ShowActivityError(string message)
        {
            ActivitiesMessage.Text = message;
            ActivitiesMessage.CssClass =
                "activities-message activities-error";
        }


        private void ShowActivitySuccess(string message)
        {
            ActivitiesMessage.Text = message;
            ActivitiesMessage.CssClass =
                "activities-message activities-success";
        }


        private void LoadSessionTypes()
        {
            ddlSessionType.Items.Clear();

            ddlSessionType.Items.Add(
                new ListItem(
                    "Select training type",
                    ""
                )
            );

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                const string sql = @"
                    SELECT
                        Session_ID,
                        Session_Type
                    FROM Session_Table
                    ORDER BY Session_Type;";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlSessionType.Items.Add(
                                new ListItem(
                                    reader["Session_Type"].ToString(),
                                    reader["Session_ID"].ToString()
                                )
                            );
                        }
                    }
                }
            }
        }


        private void LoadTrainers()
        {
            ddlTrainer.Items.Clear();

            ddlTrainer.Items.Add(
                new ListItem(
                    "Select trainer",
                    ""
                )
            );

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                const string sql = @"
                    SELECT DISTINCT
                        S.Staff_ID,
                        S.Staff_FName + ' ' +
                        S.Staff_LName AS Trainer_Name
                    FROM Staff AS S
                    INNER JOIN Trainer_Schedule AS TS
                        ON TS.Staff_ID = S.Staff_ID
                    WHERE S.Staff_Role = 'Trainer'
                    ORDER BY Trainer_Name;";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlTrainer.Items.Add(
                                new ListItem(
                                    reader["Trainer_Name"].ToString(),
                                    reader["Staff_ID"].ToString()
                                )
                            );
                        }
                    }
                }
            }
        }


        private void LoadTimePlaceholder()
        {
            ddlTime.Items.Clear();

            ddlTime.Items.Add(
                new ListItem(
                    "Select training type, trainer and date first",
                    ""
                )
            );
        }


        protected void ddlSessionType_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ClearBookingMessage();
            LoadAvailableTimes();
        }


        protected void ddlTrainer_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ClearBookingMessage();
            LoadAvailableTimes();
        }


        protected void txtDate_TextChanged(
            object sender,
            EventArgs e)
        {
            ClearBookingMessage();
            LoadAvailableTimes();
        }


        private void LoadAvailableTimes()
        {
            ddlTime.Items.Clear();

            ddlTime.Items.Add(
                new ListItem(
                    "Select available time",
                    ""
                )
            );

            if (string.IsNullOrWhiteSpace(
                    ddlSessionType.SelectedValue)
                || string.IsNullOrWhiteSpace(
                    ddlTrainer.SelectedValue))
            {
                return;
            }

            DateTime sessionDate;

            if (!DateTime.TryParse(
                    txtDate.Text,
                    out sessionDate))
            {
                return;
            }

            if (sessionDate.Date < DateTime.Today)
            {
                ShowBookingError(
                    "Past sessions cannot be booked."
                );

                return;
            }

            int sessionId;

            if (!int.TryParse(
                    ddlSessionType.SelectedValue,
                    out sessionId))
            {
                ShowBookingError(
                    "Please select a valid training type."
                );

                return;
            }

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT
                            TI.Time_ID,

                            CONVERT(
                                VARCHAR(5),
                                TI.Start_Time,
                                108
                            ) AS Start_Time,

                            CONVERT(
                                VARCHAR(5),
                                TI.End_Time,
                                108
                            ) AS End_Time,

                            DATEDIFF(
                                MINUTE,
                                TI.Start_Time,
                                TI.End_Time
                            ) AS Duration_Minutes,

                            S.Session_Capacity,

                            (
                                SELECT COUNT(*)
                                FROM Training_Session AS B
                                WHERE B.Staff_ID = TS.Staff_ID
                                  AND B.Session_ID = @Session_ID
                                  AND B.Session_Date = TS.Session_Date
                                  AND B.Time_ID = TI.Time_ID
                                  AND ISNULL(
                                      B.Session_Status,
                                      'Pending'
                                  ) <> 'Cancelled'
                            ) AS Booked_Count

                        FROM Trainer_Schedule AS TS

                        INNER JOIN Time_Slot AS TI
                            ON TI.Start_Time = TS.Start_Time
                           AND TI.End_Time = TS.End_Time

                        CROSS JOIN Session_Table AS S

                        WHERE TS.Staff_ID = @Staff_ID
                          AND TS.Session_Date = @Session_Date
                          AND TS.Availability = 'Available'
                          AND S.Session_ID = @Session_ID

                          AND
                          (
                              TS.Session_Date >
                                  CAST(GETDATE() AS DATE)

                              OR
                              (
                                  TS.Session_Date =
                                      CAST(GETDATE() AS DATE)

                                  AND TI.Start_Time >
                                      CAST(GETDATE() AS TIME)
                              )
                          )

                          AND NOT EXISTS
                          (
                              SELECT 1
                              FROM Training_Session AS Existing
                              INNER JOIN Time_Slot AS ExistingTime
                                  ON ExistingTime.Time_ID =
                                     Existing.Time_ID
                              WHERE Existing.Staff_ID =
                                    TS.Staff_ID
                                AND Existing.Session_Date =
                                    TS.Session_Date
                                AND Existing.Session_ID <>
                                    @Session_ID
                                AND ISNULL(
                                    Existing.Session_Status,
                                    'Pending'
                                ) <> 'Cancelled'
                                AND ExistingTime.Start_Time <
                                    TI.End_Time
                                AND ExistingTime.End_Time >
                                    TI.Start_Time
                          )

                        ORDER BY TI.Start_Time;";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Session_ID",
                            SqlDbType.Int
                        ).Value = sessionId;

                        command.Parameters.Add(
                            "@Staff_ID",
                            SqlDbType.VarChar,
                            20
                        ).Value =
                            ddlTrainer.SelectedValue.Trim();

                        command.Parameters.Add(
                            "@Session_Date",
                            SqlDbType.Date
                        ).Value = sessionDate.Date;

                        connection.Open();

                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int capacity =
                                    Convert.ToInt32(
                                        reader["Session_Capacity"]
                                    );

                                int booked =
                                    Convert.ToInt32(
                                        reader["Booked_Count"]
                                    );

                                if (capacity <= 0 ||
                                    booked >= capacity)
                                {
                                    continue;
                                }

                                int duration =
                                    Convert.ToInt32(
                                        reader["Duration_Minutes"]
                                    );

                                int spaces =
                                    capacity - booked;

                                ddlTime.Items.Add(
                                    new ListItem(
                                        reader["Start_Time"]
                                        + " - "
                                        + reader["End_Time"]
                                        + " · "
                                        + FormatDuration(duration)
                                        + " · "
                                        + spaces
                                        + (spaces == 1
                                            ? " space"
                                            : " spaces"),
                                        reader["Time_ID"].ToString()
                                    )
                                );
                            }
                        }
                    }
                }

                if (ddlTime.Items.Count == 1)
                {
                    ShowBookingError(
                        "No available future time slots were found."
                    );
                }
            }
            catch (Exception)
            {
                ShowBookingError(
                    "Available times could not be loaded. Please try again."
                );
            }
        }


        protected void btnBook_Click(
            object sender,
            EventArgs e)
        {
            ClearBookingMessage();

            if (string.IsNullOrWhiteSpace(
                    ddlSessionType.SelectedValue))
            {
                ShowBookingError(
                    "Please select a training type."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlTrainer.SelectedValue))
            {
                ShowBookingError(
                    "Please select a trainer."
                );

                return;
            }

            DateTime sessionDate;

            if (!DateTime.TryParse(
                    txtDate.Text,
                    out sessionDate))
            {
                ShowBookingError(
                    "Please select a valid date."
                );

                return;
            }

            if (sessionDate.Date < DateTime.Today)
            {
                ShowBookingError(
                    "Past sessions cannot be booked."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlTime.SelectedValue))
            {
                ShowBookingError(
                    "Please select an available time."
                );

                return;
            }

            int sessionId;

            if (!int.TryParse(
                    ddlSessionType.SelectedValue,
                    out sessionId))
            {
                ShowBookingError(
                    "The selected training type is invalid."
                );

                return;
            }

            string memberId =
                Session["MemberID"].ToString().Trim();

            string staffId =
                ddlTrainer.SelectedValue.Trim();

            string timeId =
                ddlTime.SelectedValue.Trim();

            string sessionType =
                ddlSessionType.SelectedItem.Text;

            string trainerName =
                ddlTrainer.SelectedItem.Text;

            int finalAttendance = 0;
            int capacity = 0;
            string durationText = "";

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    DateTime? membershipExpiry =
                        GetMembershipExpiry(
                            connection,
                            memberId
                        );

                    if (!membershipExpiry.HasValue)
                    {
                        ShowBookingError(
                            "You do not have an active membership. "
                            + "Please make a membership payment before booking."
                        );

                        return;
                    }

                    if (membershipExpiry.Value.Date <
                        sessionDate.Date)
                    {
                        ShowBookingError(
                            "Your membership expires on "
                            + membershipExpiry.Value.ToString(
                                "dd MMMM yyyy"
                            )
                            + ". It must remain active on the "
                            + "selected session date."
                        );

                        return;
                    }

                    using (SqlTransaction transaction =
                           connection.BeginTransaction(
                               IsolationLevel.Serializable))
                    {
                        try
                        {
                            TimeSlotDetails selectedTime =
                                GetTimeSlot(
                                    connection,
                                    transaction,
                                    timeId
                                );

                            DateTime sessionStart =
                                sessionDate.Date.Add(
                                    selectedTime.StartTime
                                );

                            if (sessionStart <= DateTime.Now)
                            {
                                transaction.Rollback();

                                ShowBookingError(
                                    "This session has already started or passed."
                                );

                                LoadAvailableTimes();
                                return;
                            }

                            durationText =
                                FormatDuration(
                                    selectedTime.DurationMinutes
                                );

                            if (!TrainerIsAvailable(
                                    connection,
                                    transaction,
                                    staffId,
                                    sessionDate.Date,
                                    selectedTime))
                            {
                                transaction.Rollback();

                                ShowBookingError(
                                    "The trainer is no longer available "
                                    + "for the selected time."
                                );

                                LoadAvailableTimes();
                                return;
                            }

                            string trainerConflict =
                                GetTrainerConflict(
                                    connection,
                                    transaction,
                                    sessionId,
                                    staffId,
                                    sessionDate.Date,
                                    selectedTime
                                );

                            if (!string.IsNullOrWhiteSpace(
                                    trainerConflict))
                            {
                                transaction.Rollback();

                                ShowBookingError(
                                    trainerName
                                    + " already has an overlapping "
                                    + trainerConflict
                                    + " session."
                                );

                                LoadAvailableTimes();
                                return;
                            }

                            string memberConflict =
                                GetMemberConflict(
                                    connection,
                                    transaction,
                                    memberId,
                                    sessionDate.Date,
                                    selectedTime
                                );

                            if (!string.IsNullOrWhiteSpace(
                                    memberConflict))
                            {
                                transaction.Rollback();

                                ShowBookingError(
                                    "This booking overlaps your existing "
                                    + memberConflict
                                    + " session."
                                );

                                return;
                            }

                            capacity =
                                GetSessionCapacity(
                                    connection,
                                    transaction,
                                    sessionId
                                );

                            int currentAttendance =
                                GetCurrentAttendance(
                                    connection,
                                    transaction,
                                    sessionId,
                                    staffId,
                                    sessionDate.Date,
                                    timeId
                                );

                            if (capacity <= 0 ||
                                currentAttendance >= capacity)
                            {
                                transaction.Rollback();

                                ShowBookingError(
                                    "This session is full."
                                );

                                LoadAvailableTimes();
                                return;
                            }

                            finalAttendance =
                                currentAttendance + 1;

                            InsertBooking(
                                connection,
                                transaction,
                                sessionId,
                                memberId,
                                staffId,
                                timeId,
                                sessionDate.Date,
                                finalAttendance,
                                durationText
                            );

                            UpdateMatchingAttendanceForBooking(
                                connection,
                                transaction,
                                sessionId,
                                staffId,
                                sessionDate.Date,
                                timeId,
                                finalAttendance
                            );

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                int remaining =
                    capacity - finalAttendance;

                LoadAvailableTimes();

                // Refresh the booking history on the same page.
                LoadActivities();
                ActivitiesUpdatePanel.Update();

                ShowBookingSuccess(
                    sessionType
                    + " was booked successfully with "
                    + trainerName
                    + " on "
                    + sessionDate.ToString("dd MMMM yyyy")
                    + ". Duration: "
                    + durationText
                    + ". "
                    + remaining
                    + (remaining == 1
                        ? " space remains."
                        : " spaces remain.")
                );
            }
            catch (Exception)
            {
                ShowBookingError(
                    "The booking could not be completed. "
                    + "Please refresh the page and try again."
                );
            }
        }


        private DateTime? GetMembershipExpiry(
            SqlConnection connection,
            string memberId)
        {
            const string statusSql = @"
                SELECT Member_Status
                FROM Member
                WHERE Member_ID = @Member_ID;";

            using (SqlCommand statusCommand =
                   new SqlCommand(statusSql, connection))
            {
                statusCommand.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                object statusResult =
                    statusCommand.ExecuteScalar();

                if (statusResult == null ||
                    statusResult == DBNull.Value ||
                    !statusResult.ToString().Trim().Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    return null;
                }
            }

            const string paymentSql = @"
                SELECT Payment_Date
                FROM Payment
                WHERE Member_ID = @Member_ID
                ORDER BY Payment_Date,
                         Payment_ID;";

            DateTime? expiry = null;

            using (SqlCommand paymentCommand =
                   new SqlCommand(paymentSql, connection))
            {
                paymentCommand.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataReader reader =
                       paymentCommand.ExecuteReader())
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
                            expiry =
                                paymentDate.AddMonths(1);
                        }
                        else
                        {
                            expiry =
                                expiry.Value.AddMonths(1);
                        }
                    }
                }
            }

            return expiry;
        }


        private TimeSlotDetails GetTimeSlot(
            SqlConnection connection,
            SqlTransaction transaction,
            string timeId)
        {
            const string sql = @"
                SELECT
                    Start_Time,
                    End_Time
                FROM Time_Slot
                WHERE Time_ID = @Time_ID;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Time_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = timeId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "The selected time slot no longer exists."
                        );
                    }

                    TimeSpan start =
                        (TimeSpan)reader["Start_Time"];

                    TimeSpan end =
                        (TimeSpan)reader["End_Time"];

                    return new TimeSlotDetails
                    {
                        StartTime = start,
                        EndTime = end,
                        DurationMinutes =
                            Convert.ToInt32(
                                (end - start).TotalMinutes
                            )
                    };
                }
            }
        }


        private bool TrainerIsAvailable(
            SqlConnection connection,
            SqlTransaction transaction,
            string staffId,
            DateTime sessionDate,
            TimeSlotDetails selectedTime)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Trainer_Schedule
                WHERE Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                  AND Availability = 'Available'
                  AND Start_Time <= @Start_Time
                  AND End_Time >= @End_Time;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = sessionDate.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = selectedTime.StartTime;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = selectedTime.EndTime;

                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }


        private string GetTrainerConflict(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            TimeSlotDetails selectedTime)
        {
            const string sql = @"
                SELECT TOP 1
                    S.Session_Type
                FROM Training_Session AS T
                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @Staff_ID
                  AND T.Session_Date = @Session_Date
                  AND T.Session_ID <> @Session_ID
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) <> 'Cancelled'
                  AND TI.Start_Time < @End_Time
                  AND TI.End_Time > @Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = sessionDate.Date;

                command.Parameters.Add(
                    "@Session_ID",
                    SqlDbType.Int
                ).Value = sessionId;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = selectedTime.StartTime;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = selectedTime.EndTime;

                object result =
                    command.ExecuteScalar();

                return result == null ||
                       result == DBNull.Value
                    ? ""
                    : result.ToString();
            }
        }


        private string GetMemberConflict(
            SqlConnection connection,
            SqlTransaction transaction,
            string memberId,
            DateTime sessionDate,
            TimeSlotDetails selectedTime)
        {
            const string sql = @"
                SELECT TOP 1
                    S.Session_Type
                FROM Training_Session AS T
                INNER JOIN Session_Table AS S
                    ON S.Session_ID = T.Session_ID
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Member_ID = @Member_ID
                  AND T.Session_Date = @Session_Date
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) <> 'Cancelled'
                  AND TI.Start_Time < @End_Time
                  AND TI.End_Time > @Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = sessionDate.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = selectedTime.StartTime;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = selectedTime.EndTime;

                object result =
                    command.ExecuteScalar();

                return result == null ||
                       result == DBNull.Value
                    ? ""
                    : result.ToString();
            }
        }


        private int GetSessionCapacity(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId)
        {
            const string sql = @"
                SELECT Session_Capacity
                FROM Session_Table
                WHERE Session_ID = @Session_ID;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Session_ID",
                    SqlDbType.Int
                ).Value = sessionId;

                object result =
                    command.ExecuteScalar();

                return result == null ||
                       result == DBNull.Value
                    ? 0
                    : Convert.ToInt32(result);
            }
        }


        private int GetCurrentAttendance(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Training_Session
                WHERE Session_ID = @Session_ID
                  AND Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                  AND Time_ID = @Time_ID
                  AND ISNULL(
                      Session_Status,
                      'Pending'
                  ) <> 'Cancelled';";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                AddSessionParameters(
                    command,
                    sessionId,
                    staffId,
                    sessionDate,
                    timeId
                );

                return Convert.ToInt32(
                    command.ExecuteScalar()
                );
            }
        }


        private void InsertBooking(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string memberId,
            string staffId,
            string timeId,
            DateTime sessionDate,
            int attendance,
            string duration)
        {
            const string sql = @"
                INSERT INTO Training_Session
                (
                    Session_ID,
                    Member_ID,
                    Staff_ID,
                    Time_ID,
                    Session_Date,
                    Staff_Attendance,
                    Member_Attendance,
                    Session_Duration,
                    Session_Status
                )
                VALUES
                (
                    @Session_ID,
                    @Member_ID,
                    @Staff_ID,
                    @Time_ID,
                    @Session_Date,
                    @Staff_Attendance,
                    @Member_Attendance,
                    @Session_Duration,
                    'Pending'
                );";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Session_ID",
                    SqlDbType.Int
                ).Value = sessionId;

                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.Parameters.Add(
                    "@Time_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = timeId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = sessionDate.Date;

                command.Parameters.Add(
                    "@Staff_Attendance",
                    SqlDbType.VarChar,
                    20
                ).Value = "N/A";

                command.Parameters.Add(
                    "@Member_Attendance",
                    SqlDbType.Int
                ).Value = attendance;

                command.Parameters.Add(
                    "@Session_Duration",
                    SqlDbType.VarChar,
                    50
                ).Value = duration;

                command.ExecuteNonQuery();
            }
        }


        private static void AddSessionParameters(
            SqlCommand command,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId)
        {
            command.Parameters.Add(
                "@Session_ID",
                SqlDbType.Int
            ).Value = sessionId;

            command.Parameters.Add(
                "@Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = staffId;

            command.Parameters.Add(
                "@Session_Date",
                SqlDbType.Date
            ).Value = sessionDate.Date;

            command.Parameters.Add(
                "@Time_ID",
                SqlDbType.VarChar,
                20
            ).Value = timeId;
        }


        private static string FormatDuration(
            int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            if (hours > 0 && minutes > 0)
            {
                return hours
                    + (hours == 1 ? " hour " : " hours ")
                    + minutes
                    + (minutes == 1
                        ? " minute"
                        : " minutes");
            }

            if (hours > 0)
            {
                return hours
                    + (hours == 1
                        ? " hour"
                        : " hours");
            }

            return minutes
                + (minutes == 1
                    ? " minute"
                    : " minutes");
        }


        private void UpdateMatchingAttendanceForBooking(
            SqlConnection connection,
            SqlTransaction transaction,
            int sessionId,
            string staffId,
            DateTime sessionDate,
            string timeId,
            int attendance)
        {
            const string sql = @"
                UPDATE Training_Session
                SET Member_Attendance = @Member_Attendance
                WHERE Session_ID = @Session_ID
                  AND Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                  AND Time_ID = @Time_ID
                  AND ISNULL(
                      Session_Status,
                      'Pending'
                  ) <> 'Cancelled';";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Member_Attendance",
                    SqlDbType.Int
                ).Value = attendance;

                AddSessionParameters(
                    command,
                    sessionId,
                    staffId,
                    sessionDate,
                    timeId
                );

                command.ExecuteNonQuery();
            }
        }

        private void ClearBookingMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "booking-message";
        }

        private void ShowBookingError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "booking-message booking-error";
        }

        private void ShowBookingSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "booking-message booking-success";
        }

        private sealed class BookingDetails
        {
            public int SessionId { get; set; }

            public string StaffId { get; set; }

            public string TimeId { get; set; }

            public DateTime SessionDate { get; set; }

            public TimeSpan StartTime { get; set; }

            public string Status { get; set; }

            public string SessionType { get; set; }

            public string TrainerName { get; set; }
        }

        private sealed class TimeSlotDetails
        {
            public TimeSpan StartTime { get; set; }

            public TimeSpan EndTime { get; set; }

            public int DurationMinutes { get; set; }
        }
    }
}
