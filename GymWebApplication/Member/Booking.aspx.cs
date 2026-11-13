using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace GymWebApplication.Member
{
    public partial class Booking : System.Web.UI.Page
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
            ClearMessage();
            LoadAvailableTimes();
        }

        protected void ddlTrainer_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
            LoadAvailableTimes();
        }

        protected void txtDate_TextChanged(
            object sender,
            EventArgs e)
        {
            ClearMessage();
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
                ShowError(
                    "Past sessions cannot be booked."
                );

                return;
            }

            int sessionId;

            if (!int.TryParse(
                    ddlSessionType.SelectedValue,
                    out sessionId))
            {
                ShowError(
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
                    ShowError(
                        "No available future time slots were found."
                    );
                }
            }
            catch (Exception)
            {
                ShowError(
                    "Available times could not be loaded. Please try again."
                );
            }
        }

        protected void btnBook_Click(
            object sender,
            EventArgs e)
        {
            ClearMessage();

            if (string.IsNullOrWhiteSpace(
                    ddlSessionType.SelectedValue))
            {
                ShowError(
                    "Please select a training type."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlTrainer.SelectedValue))
            {
                ShowError(
                    "Please select a trainer."
                );

                return;
            }

            DateTime sessionDate;

            if (!DateTime.TryParse(
                    txtDate.Text,
                    out sessionDate))
            {
                ShowError(
                    "Please select a valid date."
                );

                return;
            }

            if (sessionDate.Date < DateTime.Today)
            {
                ShowError(
                    "Past sessions cannot be booked."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ddlTime.SelectedValue))
            {
                ShowError(
                    "Please select an available time."
                );

                return;
            }

            int sessionId;

            if (!int.TryParse(
                    ddlSessionType.SelectedValue,
                    out sessionId))
            {
                ShowError(
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
                        ShowError(
                            "You do not have an active membership. "
                            + "Please make a membership payment before booking."
                        );

                        return;
                    }

                    if (membershipExpiry.Value.Date <
                        sessionDate.Date)
                    {
                        ShowError(
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

                                ShowError(
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

                                ShowError(
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

                                ShowError(
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

                                ShowError(
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

                                ShowError(
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

                            UpdateMatchingAttendance(
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

                ShowSuccess(
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
                ShowError(
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

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass = "booking-message";
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "booking-message booking-error";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "booking-message booking-success";
        }

        private sealed class TimeSlotDetails
        {
            public TimeSpan StartTime { get; set; }

            public TimeSpan EndTime { get; set; }

            public int DurationMinutes { get; set; }
        }
    }
}