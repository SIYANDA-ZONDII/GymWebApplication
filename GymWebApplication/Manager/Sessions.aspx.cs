using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Manager
{
    public partial class Sessions : Page
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
                txtSessionDate.Attributes["min"] =
                    DateTime.Today.ToString("yyyy-MM-dd");

                LoadTrainers();
                LoadTimeOptions();
                LoadSessions();
            }
        }

        private bool IsManagerLoggedIn()
        {
            if (!Request.IsAuthenticated)
            {
                return false;
            }

            if (Context.User != null &&
                Context.User.IsInRole("Manager"))
            {
                return true;
            }

            return Session["UserRole"] != null
                && Session["UserRole"]
                    .ToString()
                    .Trim()
                    .Equals(
                        "Manager",
                        StringComparison.OrdinalIgnoreCase
                    );
        }

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
            LoadSessions();
        }

        protected void ddlFilterStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
            LoadSessions();
        }

        protected void btnClearFilters_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            ddlFilterStatus.SelectedValue = "All";

            ClearMessage();
            LoadSessions();
        }

        private void LoadTrainers()
        {
            ddlEditTrainer.Items.Clear();

            ddlEditTrainer.Items.Add(
                new ListItem(
                    "Select trainer",
                    ""
                )
            );

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                const string sql = @"
                    SELECT
                        Staff_ID,
                        Staff_FName + ' ' +
                        Staff_LName AS Trainer_Name
                    FROM Staff
                    WHERE Staff_Role = 'Trainer'
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
                            ddlEditTrainer.Items.Add(
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

        private void LoadTimeOptions()
        {
            ddlTime.Items.Clear();

            ddlTime.Items.Add(
                new ListItem(
                    "Select time",
                    ""
                )
            );

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                const string sql = @"
                    SELECT
                        Time_ID,
                        CONVERT(
                            VARCHAR(5),
                            Start_Time,
                            108
                        ) AS Start_Time,
                        CONVERT(
                            VARCHAR(5),
                            End_Time,
                            108
                        ) AS End_Time,
                        DATEDIFF(
                            MINUTE,
                            Start_Time,
                            End_Time
                        ) AS Duration_Minutes
                    FROM Time_Slot
                    ORDER BY Start_Time;";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int duration =
                                Convert.ToInt32(
                                    reader["Duration_Minutes"]
                                );

                            ddlTime.Items.Add(
                                new ListItem(
                                    reader["Start_Time"]
                                    + " - "
                                    + reader["End_Time"]
                                    + " · "
                                    + FormatDuration(duration),
                                    reader["Time_ID"].ToString()
                                )
                            );
                        }
                    }
                }
            }
        }

        private void LoadSessions()
        {
            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    AutoCompletePastSessions(connection);

                    DataTable table =
                        GetSessions(
                            connection,
                            txtSearch.Text.Trim(),
                            ddlFilterStatus.SelectedValue
                        );

                    SessionsGrid.DataSource = table;
                    SessionsGrid.DataBind();

                    UpdateSummary(table);
                }
            }
            catch (Exception)
            {
                ShowError(
                    "Sessions could not be loaded. "
                    + "Please refresh the page."
                );
            }
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
                WHERE ISNULL(
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
                command.ExecuteNonQuery();
            }
        }

        private DataTable GetSessions(
            SqlConnection connection,
            string search,
            string status)
        {
            DataTable table = new DataTable();

            const string sql = @"
                WITH SessionGroups AS
                (
                    SELECT
                        T.Session_ID,
                        T.Staff_ID,
                        T.Session_Date,
                        T.Time_ID,
                        S.Session_Type,
                        S.Session_Capacity,
                        ST.Staff_FName,
                        ST.Staff_LName,
                        TI.Start_Time,
                        TI.End_Time,

                        MAX(
                            T.Session_Duration
                        ) AS Session_Duration,

                        SUM
                        (
                            CASE
                                WHEN ISNULL(
                                    T.Session_Status,
                                    'Pending'
                                ) <> 'Cancelled'
                                    THEN 1
                                ELSE 0
                            END
                        ) AS Member_Count,

                        CASE
                            WHEN SUM
                            (
                                CASE
                                    WHEN ISNULL(
                                        T.Session_Status,
                                        'Pending'
                                    ) = 'Pending'
                                        THEN 1
                                    ELSE 0
                                END
                            ) > 0
                                THEN 'Pending'

                            WHEN SUM
                            (
                                CASE
                                    WHEN T.Session_Status =
                                         'Completed'
                                        THEN 1
                                    ELSE 0
                                END
                            ) > 0
                                THEN 'Completed'

                            ELSE 'Cancelled'
                        END AS Session_Status

                    FROM Training_Session AS T

                    INNER JOIN Session_Table AS S
                        ON S.Session_ID = T.Session_ID

                    INNER JOIN Staff AS ST
                        ON ST.Staff_ID = T.Staff_ID

                    INNER JOIN Time_Slot AS TI
                        ON TI.Time_ID = T.Time_ID

                    GROUP BY
                        T.Session_ID,
                        T.Staff_ID,
                        T.Session_Date,
                        T.Time_ID,
                        S.Session_Type,
                        S.Session_Capacity,
                        ST.Staff_FName,
                        ST.Staff_LName,
                        TI.Start_Time,
                        TI.End_Time
                )

                SELECT
                    CAST(Session_ID AS VARCHAR(20))
                    + '|'
                    + CAST(Staff_ID AS VARCHAR(20))
                    + '|'
                    + CONVERT(
                        VARCHAR(8),
                        Session_Date,
                        112
                    )
                    + '|'
                    + CAST(Time_ID AS VARCHAR(20))
                    AS Session_Key,

                    Session_ID,
                    Staff_ID,
                    Session_Type,

                    Staff_FName + ' ' +
                    Staff_LName AS Trainer_Name,

                    Session_Date,

                    CONVERT(
                        VARCHAR(5),
                        Start_Time,
                        108
                    ) AS Start_Time,

                    CONVERT(
                        VARCHAR(5),
                        End_Time,
                        108
                    ) AS End_Time,

                    Session_Duration,
                    Member_Count,
                    Session_Capacity,
                    Session_Status

                FROM SessionGroups AS G

                WHERE
                (
                    @Status = 'All'
                    OR G.Session_Status = @Status
                )

                AND
                (
                    @Search = ''
                    OR G.Session_Type LIKE
                       '%' + @Search + '%'
                    OR G.Staff_FName LIKE
                       '%' + @Search + '%'
                    OR G.Staff_LName LIKE
                       '%' + @Search + '%'
                    OR
                    (
                        G.Staff_FName + ' ' +
                        G.Staff_LName
                    ) LIKE '%' + @Search + '%'

                    OR EXISTS
                    (
                        SELECT 1
                        FROM Training_Session AS TM
                        INNER JOIN Member AS M
                            ON M.Member_ID = TM.Member_ID
                        WHERE TM.Staff_ID = G.Staff_ID
                          AND TM.Session_ID = G.Session_ID
                          AND TM.Session_Date = G.Session_Date
                          AND TM.Time_ID = G.Time_ID
                          AND
                          (
                              M.Member_FName LIKE
                                  '%' + @Search + '%'
                              OR M.Member_LName LIKE
                                  '%' + @Search + '%'
                              OR
                              (
                                  M.Member_FName + ' ' +
                                  M.Member_LName
                              ) LIKE '%' + @Search + '%'
                          )
                    )
                )

                ORDER BY
                    G.Session_Date DESC,
                    G.Start_Time DESC,
                    Trainer_Name;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
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

        private void UpdateSummary(DataTable table)
        {
            int pending = 0;
            int completed = 0;
            int cancelled = 0;

            foreach (DataRow row in table.Rows)
            {
                string status =
                    row["Session_Status"]
                        .ToString()
                        .Trim();

                if (status.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    pending++;
                }
                else if (status.Equals(
                        "Completed",
                        StringComparison.OrdinalIgnoreCase))
                {
                    completed++;
                }
                else if (status.Equals(
                        "Cancelled",
                        StringComparison.OrdinalIgnoreCase))
                {
                    cancelled++;
                }
            }

            lblTotalSessions.Text =
                table.Rows.Count.ToString();

            lblPendingSessions.Text =
                pending.ToString();

            lblCompletedSessions.Text =
                completed.ToString();

            lblCancelledSessions.Text =
                cancelled.ToString();
        }

        protected void SessionsGrid_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            SessionKey key;

            if (!SessionKey.TryParse(
                    Convert.ToString(e.CommandArgument),
                    out key))
            {
                ShowError(
                    "The selected session is invalid."
                );

                return;
            }

            if (e.CommandName == "EditSession")
            {
                LoadEditSession(key);
            }
            else if (e.CommandName == "CancelSession")
            {
                CancelSession(key);
            }
        }

        private void LoadEditSession(SessionKey key)
        {
            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                connection.Open();

                const string sql = @"
                    SELECT
                        S.Session_Type,
                        T.Staff_ID,
                        T.Session_Date,
                        T.Time_ID,

                        MAX(
                            T.Session_Duration
                        ) AS Session_Duration,

                        SUM
                        (
                            CASE
                                WHEN ISNULL(
                                    T.Session_Status,
                                    'Pending'
                                ) <> 'Cancelled'
                                    THEN 1
                                ELSE 0
                            END
                        ) AS Member_Count

                    FROM Training_Session AS T

                    INNER JOIN Session_Table AS S
                        ON S.Session_ID = T.Session_ID

                    WHERE T.Session_ID = @Session_ID
                      AND T.Staff_ID = @Staff_ID
                      AND T.Session_Date = @Session_Date
                      AND T.Time_ID = @Time_ID
                      AND ISNULL(
                          T.Session_Status,
                          'Pending'
                      ) = 'Pending'

                    GROUP BY
                        S.Session_Type,
                        T.Staff_ID,
                        T.Session_Date,
                        T.Time_ID;";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    AddKeyParameters(command, key);

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowError(
                                "Only an upcoming pending session "
                                + "can be edited."
                            );

                            return;
                        }

                        lblEditSessionType.Text =
                            reader["Session_Type"].ToString();

                        lblEditMemberCount.Text =
                            reader["Member_Count"]
                            + " member(s)";

                        string trainerId =
                            reader["Staff_ID"].ToString();

                        if (ddlEditTrainer.Items
                            .FindByValue(trainerId) != null)
                        {
                            ddlEditTrainer.SelectedValue =
                                trainerId;
                        }

                        txtSessionDate.Text =
                            Convert.ToDateTime(
                                reader["Session_Date"]
                            ).ToString("yyyy-MM-dd");

                        string timeId =
                            reader["Time_ID"].ToString();

                        if (ddlTime.Items.FindByValue(timeId) != null)
                        {
                            ddlTime.SelectedValue = timeId;
                        }

                        lblEditDuration.Text =
                            reader["Session_Duration"].ToString();

                        ddlEditStatus.SelectedValue =
                            "Pending";

                        ViewState["EditSessionID"] =
                            key.SessionId;

                        ViewState["EditStaffID"] =
                            key.StaffId;

                        ViewState["EditSessionDate"] =
                            key.SessionDate.ToString(
                                "yyyy-MM-dd"
                            );

                        ViewState["EditTimeID"] =
                            key.TimeId;

                        EditPanel.Visible = true;
                    }
                }
            }
        }

        protected void ddlTime_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                    ddlTime.SelectedValue))
            {
                lblEditDuration.Text = "—";
                return;
            }

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                connection.Open();

                TimeDetails time =
                    GetTimeDetails(
                        connection,
                        null,
                        ddlTime.SelectedValue
                    );

                lblEditDuration.Text =
                    FormatDuration(
                        time.DurationMinutes
                    );
            }
        }

        protected void btnCloseEdit_Click(
            object sender,
            EventArgs e)
        {
            EditPanel.Visible = false;
            ClearMessage();
        }

        protected void btnUpdateSession_Click(
            object sender,
            EventArgs e)
        {
            if (ViewState["EditSessionID"] == null ||
                ViewState["EditStaffID"] == null ||
                ViewState["EditSessionDate"] == null ||
                ViewState["EditTimeID"] == null)
            {
                ShowError(
                    "Select a session to edit first."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlEditTrainer.SelectedValue))
            {
                ShowError(
                    "Please select a trainer."
                );

                return;
            }

            DateTime newDate;

            if (!DateTime.TryParse(
                    txtSessionDate.Text,
                    out newDate)
                || newDate.Date < DateTime.Today)
            {
                ShowError(
                    "Please select today or a future date."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlTime.SelectedValue))
            {
                ShowError(
                    "Please select a time."
                );

                return;
            }

            SessionKey original =
                new SessionKey
                {
                    SessionId =
                        Convert.ToInt32(
                            ViewState["EditSessionID"]
                        ),

                    StaffId =
                        ViewState["EditStaffID"].ToString(),

                    SessionDate =
                        Convert.ToDateTime(
                            ViewState["EditSessionDate"]
                        ).Date,

                    TimeId =
                        ViewState["EditTimeID"].ToString()
                };

            string newStaffId =
                ddlEditTrainer.SelectedValue.Trim();

            string requestedStatus =
                ddlEditStatus.SelectedValue == "Completed"
                ? "Completed"
                : "Pending";

            try
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
                            TimeDetails newTime =
                                GetTimeDetails(
                                    connection,
                                    transaction,
                                    ddlTime.SelectedValue
                                );

                            DateTime newStart =
                                newDate.Date.Add(
                                    newTime.StartTime
                                );

                            if (requestedStatus == "Pending" &&
                                newStart <= DateTime.Now)
                            {
                                transaction.Rollback();

                                ShowError(
                                    "A pending session must start "
                                    + "in the future."
                                );

                                return;
                            }

                            if (requestedStatus == "Pending" &&
                                !TrainerAvailable(
                                    connection,
                                    transaction,
                                    newStaffId,
                                    newDate.Date,
                                    newTime))
                            {
                                transaction.Rollback();

                                ShowError(
                                    "The selected trainer is not "
                                    + "available for that date and time."
                                );

                                return;
                            }

                            if (requestedStatus == "Pending" &&
                                TrainerHasConflict(
                                    connection,
                                    transaction,
                                    newStaffId,
                                    original,
                                    newDate.Date,
                                    newTime))
                            {
                                transaction.Rollback();

                                ShowError(
                                    "The selected trainer has another "
                                    + "overlapping session."
                                );

                                return;
                            }

                            if (requestedStatus == "Pending" &&
                                MembersHaveConflict(
                                    connection,
                                    transaction,
                                    original,
                                    newDate.Date,
                                    newTime))
                            {
                                transaction.Rollback();

                                ShowError(
                                    "At least one booked member has "
                                    + "another overlapping session."
                                );

                                return;
                            }

                            int affected =
                                UpdateSessionGroup(
                                    connection,
                                    transaction,
                                    original,
                                    newStaffId,
                                    newDate.Date,
                                    ddlTime.SelectedValue,
                                    FormatDuration(
                                        newTime.DurationMinutes
                                    ),
                                    requestedStatus
                                );

                            if (affected == 0)
                            {
                                throw new InvalidOperationException(
                                    "No pending rows were updated."
                                );
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                EditPanel.Visible = false;
                LoadSessions();

                ShowSuccess(
                    "The session was updated successfully "
                    + "for every booked member."
                );
            }
            catch (Exception)
            {
                ShowError(
                    "The session could not be updated. "
                    + "Please verify the trainer, date and time."
                );
            }
        }

        private void CancelSession(SessionKey key)
        {
            try
            {
                int affected;

                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        UPDATE Training_Session
                        SET Session_Status = 'Cancelled'
                        WHERE Session_ID = @Session_ID
                          AND Staff_ID = @Staff_ID
                          AND Session_Date = @Session_Date
                          AND Time_ID = @Time_ID
                          AND ISNULL(
                              Session_Status,
                              'Pending'
                          ) = 'Pending';";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        AddKeyParameters(command, key);

                        connection.Open();

                        affected =
                            command.ExecuteNonQuery();
                    }
                }

                LoadSessions();

                if (affected > 0)
                {
                    ShowSuccess(
                        "The entire session was cancelled for "
                        + affected
                        + (affected == 1
                            ? " member."
                            : " members.")
                    );
                }
                else
                {
                    ShowError(
                        "Only a pending session can be cancelled."
                    );
                }
            }
            catch (Exception)
            {
                ShowError(
                    "The session could not be cancelled."
                );
            }
        }

        private TimeDetails GetTimeDetails(
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
                            "The selected time no longer exists."
                        );
                    }

                    TimeSpan start =
                        (TimeSpan)reader["Start_Time"];

                    TimeSpan end =
                        (TimeSpan)reader["End_Time"];

                    return new TimeDetails
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

        private bool TrainerAvailable(
            SqlConnection connection,
            SqlTransaction transaction,
            string staffId,
            DateTime date,
            TimeDetails time)
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
                AddScheduleParameters(
                    command,
                    staffId,
                    date,
                    time
                );

                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }

        private bool TrainerHasConflict(
            SqlConnection connection,
            SqlTransaction transaction,
            string newStaffId,
            SessionKey original,
            DateTime newDate,
            TimeDetails newTime)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @New_Staff_ID
                  AND T.Session_Date = @New_Date
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) <> 'Cancelled'
                  AND TI.Start_Time < @End_Time
                  AND TI.End_Time > @Start_Time
                  AND NOT
                  (
                      T.Session_ID = @Session_ID
                      AND T.Staff_ID = @Old_Staff_ID
                      AND T.Session_Date = @Old_Date
                      AND T.Time_ID = @Old_Time_ID
                  );";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                AddConflictParameters(
                    command,
                    newStaffId,
                    original,
                    newDate,
                    newTime
                );

                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }

        private bool MembersHaveConflict(
            SqlConnection connection,
            SqlTransaction transaction,
            SessionKey original,
            DateTime newDate,
            TimeDetails newTime)
        {
            const string sql = @"
                SELECT COUNT(*)

                FROM Training_Session AS Moving

                INNER JOIN Training_Session AS OtherBooking
                    ON OtherBooking.Member_ID =
                       Moving.Member_ID

                INNER JOIN Time_Slot AS OtherTime
                    ON OtherTime.Time_ID =
                       OtherBooking.Time_ID

                WHERE Moving.Session_ID = @Session_ID
                  AND Moving.Staff_ID = @Old_Staff_ID
                  AND Moving.Session_Date = @Old_Date
                  AND Moving.Time_ID = @Old_Time_ID
                  AND ISNULL(
                      Moving.Session_Status,
                      'Pending'
                  ) = 'Pending'

                  AND OtherBooking.Session_Date =
                      @New_Date

                  AND ISNULL(
                      OtherBooking.Session_Status,
                      'Pending'
                  ) <> 'Cancelled'

                  AND OtherTime.Start_Time <
                      @End_Time

                  AND OtherTime.End_Time >
                      @Start_Time

                  AND NOT
                  (
                      OtherBooking.Session_ID =
                          @Session_ID
                      AND OtherBooking.Staff_ID =
                          @Old_Staff_ID
                      AND OtherBooking.Session_Date =
                          @Old_Date
                      AND OtherBooking.Time_ID =
                          @Old_Time_ID
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
                ).Value = original.SessionId;

                command.Parameters.Add(
                    "@Old_Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = original.StaffId;

                command.Parameters.Add(
                    "@Old_Date",
                    SqlDbType.Date
                ).Value = original.SessionDate.Date;

                command.Parameters.Add(
                    "@Old_Time_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = original.TimeId;

                command.Parameters.Add(
                    "@New_Date",
                    SqlDbType.Date
                ).Value = newDate.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = newTime.StartTime;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = newTime.EndTime;

                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }

        private int UpdateSessionGroup(
            SqlConnection connection,
            SqlTransaction transaction,
            SessionKey original,
            string newStaffId,
            DateTime newDate,
            string newTimeId,
            string duration,
            string status)
        {
            const string sql = @"
                UPDATE Training_Session
                SET
                    Staff_ID = @New_Staff_ID,
                    Session_Date = @New_Date,
                    Time_ID = @New_Time_ID,
                    Session_Duration = @Duration,
                    Session_Status = @Status
                WHERE Session_ID = @Session_ID
                  AND Staff_ID = @Old_Staff_ID
                  AND Session_Date = @Old_Date
                  AND Time_ID = @Old_Time_ID
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
                    "@New_Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = newStaffId;

                command.Parameters.Add(
                    "@New_Date",
                    SqlDbType.Date
                ).Value = newDate.Date;

                command.Parameters.Add(
                    "@New_Time_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = newTimeId;

                command.Parameters.Add(
                    "@Duration",
                    SqlDbType.VarChar,
                    50
                ).Value = duration;

                command.Parameters.Add(
                    "@Status",
                    SqlDbType.VarChar,
                    20
                ).Value = status;

                command.Parameters.Add(
                    "@Session_ID",
                    SqlDbType.Int
                ).Value = original.SessionId;

                command.Parameters.Add(
                    "@Old_Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = original.StaffId;

                command.Parameters.Add(
                    "@Old_Date",
                    SqlDbType.Date
                ).Value = original.SessionDate.Date;

                command.Parameters.Add(
                    "@Old_Time_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = original.TimeId;

                return command.ExecuteNonQuery();
            }
        }

        private static void AddScheduleParameters(
            SqlCommand command,
            string staffId,
            DateTime date,
            TimeDetails time)
        {
            command.Parameters.Add(
                "@Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = staffId;

            command.Parameters.Add(
                "@Session_Date",
                SqlDbType.Date
            ).Value = date.Date;

            command.Parameters.Add(
                "@Start_Time",
                SqlDbType.Time
            ).Value = time.StartTime;

            command.Parameters.Add(
                "@End_Time",
                SqlDbType.Time
            ).Value = time.EndTime;
        }

        private static void AddConflictParameters(
            SqlCommand command,
            string newStaffId,
            SessionKey original,
            DateTime newDate,
            TimeDetails newTime)
        {
            command.Parameters.Add(
                "@New_Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = newStaffId;

            command.Parameters.Add(
                "@Session_ID",
                SqlDbType.Int
            ).Value = original.SessionId;

            command.Parameters.Add(
                "@Old_Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = original.StaffId;

            command.Parameters.Add(
                "@Old_Date",
                SqlDbType.Date
            ).Value = original.SessionDate.Date;

            command.Parameters.Add(
                "@Old_Time_ID",
                SqlDbType.VarChar,
                20
            ).Value = original.TimeId;

            command.Parameters.Add(
                "@New_Date",
                SqlDbType.Date
            ).Value = newDate.Date;

            command.Parameters.Add(
                "@Start_Time",
                SqlDbType.Time
            ).Value = newTime.StartTime;

            command.Parameters.Add(
                "@End_Time",
                SqlDbType.Time
            ).Value = newTime.EndTime;
        }

        private static void AddKeyParameters(
            SqlCommand command,
            SessionKey key)
        {
            command.Parameters.Add(
                "@Session_ID",
                SqlDbType.Int
            ).Value = key.SessionId;

            command.Parameters.Add(
                "@Staff_ID",
                SqlDbType.VarChar,
                20
            ).Value = key.StaffId;

            command.Parameters.Add(
                "@Session_Date",
                SqlDbType.Date
            ).Value = key.SessionDate.Date;

            command.Parameters.Add(
                "@Time_ID",
                SqlDbType.VarChar,
                20
            ).Value = key.TimeId;
        }

        protected bool CanManageSession(
            object statusValue,
            object dateValue,
            object startValue)
        {
            string status =
                Convert.ToString(statusValue).Trim();

            if (!status.Equals(
                    "Pending",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            DateTime date;
            TimeSpan start;

            if (!DateTime.TryParse(
                    Convert.ToString(dateValue),
                    out date)
                || !TimeSpan.TryParse(
                    Convert.ToString(startValue),
                    out start))
            {
                return false;
            }

            return date.Date.Add(start) > DateTime.Now;
        }

        protected string GetStatusClass(object value)
        {
            string status =
                Convert.ToString(value).Trim();

            if (status.Equals(
                    "Completed",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "manager-session-status "
                    + "manager-status-completed";
            }

            if (status.Equals(
                    "Cancelled",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "manager-session-status "
                    + "manager-status-cancelled";
            }

            return "manager-session-status "
                + "manager-status-pending";
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

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass =
                "manager-session-message";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "manager-session-message "
                + "manager-message-success";
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "manager-session-message "
                + "manager-message-error";
        }

        private sealed class TimeDetails
        {
            public TimeSpan StartTime { get; set; }

            public TimeSpan EndTime { get; set; }

            public int DurationMinutes { get; set; }
        }

        private sealed class SessionKey
        {
            public int SessionId { get; set; }

            public string StaffId { get; set; }

            public DateTime SessionDate { get; set; }

            public string TimeId { get; set; }

            public static bool TryParse(
                string value,
                out SessionKey key)
            {
                key = null;

                if (string.IsNullOrWhiteSpace(value))
                {
                    return false;
                }

                string[] parts =
                    value.Split('|');

                int sessionId;
                DateTime date;

                if (parts.Length != 4 ||
                    !int.TryParse(parts[0], out sessionId) ||
                    !DateTime.TryParseExact(
                        parts[2],
                        "yyyyMMdd",
                        System.Globalization
                            .CultureInfo
                            .InvariantCulture,
                        System.Globalization
                            .DateTimeStyles.None,
                        out date))
                {
                    return false;
                }

                key = new SessionKey
                {
                    SessionId = sessionId,
                    StaffId = parts[1],
                    SessionDate = date.Date,
                    TimeId = parts[3]
                };

                return !string.IsNullOrWhiteSpace(
                           key.StaffId
                       )
                    && !string.IsNullOrWhiteSpace(
                           key.TimeId
                       );
            }
        }
    }
}