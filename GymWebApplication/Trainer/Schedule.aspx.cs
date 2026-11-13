using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Trainer
{
    public partial class Schedule : Page
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

        private DataTable SlotTable
        {
            get
            {
                return ViewState["ScheduleSlotTable"]
                    as DataTable;
            }

            set
            {
                ViewState["ScheduleSlotTable"] = value;
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
                txtDate.Attributes["min"] =
                    DateTime.Today.ToString("yyyy-MM-dd");

                txtDate.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");

                LoadTimeSlots(DateTime.Today);
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

        protected void btnLoadDate_Click(
            object sender,
            EventArgs e)
        {
            ClearMessage();

            DateTime selectedDate;

            if (!DateTime.TryParse(
                    txtDate.Text,
                    out selectedDate))
            {
                ShowError(
                    "Please select a valid date."
                );

                pnlSlots.Visible = false;
                return;
            }

            if (selectedDate.Date < DateTime.Today)
            {
                ShowError(
                    "Past availability cannot be changed."
                );

                pnlSlots.Visible = false;
                return;
            }

            ddlSlotFilter.SelectedValue = "All";

            LoadTimeSlots(selectedDate.Date);
        }

        protected void ddlSlotFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindFilteredSlots();
        }

        private string GetTrainerStaffId(
            SqlConnection connection)
        {
            const string sql = @"
                SELECT Staff_ID
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
                ).Value =
                    Context.User.Identity.Name;

                object result =
                    command.ExecuteScalar();

                return result == null ||
                       result == DBNull.Value
                    ? null
                    : result.ToString().Trim();
            }
        }

        private void LoadTimeSlots(DateTime selectedDate)
        {
            DataTable table = CreateSlotTable();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string staffId =
                        GetTrainerStaffId(connection);

                    if (string.IsNullOrWhiteSpace(staffId))
                    {
                        ShowError(
                            "Your trainer account could not be found."
                        );

                        pnlSlots.Visible = false;
                        return;
                    }

                    DataTable savedSlots =
                        GetSavedSlots(
                            connection,
                            staffId,
                            selectedDate
                        );

                    for (int hour = 8; hour < 18; hour++)
                    {
                        TimeSpan start =
                            new TimeSpan(hour, 0, 0);

                        TimeSpan end =
                            new TimeSpan(hour + 1, 0, 0);

                        bool available =
                            GetSavedAvailability(
                                savedSlots,
                                start,
                                end
                            );

                        int bookingCount =
                            GetBookingCount(
                                connection,
                                staffId,
                                selectedDate,
                                start,
                                end
                            );

                        DateTime slotStart =
                            selectedDate.Date.Add(start);

                        bool isPast =
                            slotStart <= DateTime.Now;

                        bool canEdit =
                            !isPast &&
                            bookingCount == 0;

                        string status;
                        string statusCss;

                        if (bookingCount > 0)
                        {
                            status = "Booked";
                            statusCss =
                                "slot-status booked";

                            available = true;
                        }
                        else if (isPast)
                        {
                            status = "Past";
                            statusCss =
                                "slot-status past";
                        }
                        else if (available)
                        {
                            status = "Available";
                            statusCss =
                                "slot-status available";
                        }
                        else
                        {
                            status = "Not Available";
                            statusCss =
                                "slot-status unavailable";
                        }

                        DataRow row = table.NewRow();

                        row["TimeSlot"] =
                            start.ToString(@"hh\:mm")
                            + " — "
                            + end.ToString(@"hh\:mm");

                        row["SlotStart"] =
                            start.ToString(@"hh\:mm");

                        row["SlotEnd"] =
                            end.ToString(@"hh\:mm");

                        row["IsAvailable"] = available;
                        row["CanEdit"] = canEdit;
                        row["BookingCount"] = bookingCount;
                        row["Status"] = status;
                        row["StatusCss"] = statusCss;

                        table.Rows.Add(row);
                    }
                }

                SlotTable = table;

                lblSelectedDate.Text =
                    selectedDate.ToString(
                        "dddd, dd MMMM yyyy"
                    );

                pnlSlots.Visible = true;

                UpdateSlotSummary(table);
                BindFilteredSlots();
            }
            catch
            {
                ShowError(
                    "Availability could not be loaded."
                );

                pnlSlots.Visible = false;
            }
        }

        private static DataTable CreateSlotTable()
        {
            DataTable table = new DataTable();

            table.Columns.Add(
                "TimeSlot",
                typeof(string)
            );

            table.Columns.Add(
                "SlotStart",
                typeof(string)
            );

            table.Columns.Add(
                "SlotEnd",
                typeof(string)
            );

            table.Columns.Add(
                "IsAvailable",
                typeof(bool)
            );

            table.Columns.Add(
                "CanEdit",
                typeof(bool)
            );

            table.Columns.Add(
                "BookingCount",
                typeof(int)
            );

            table.Columns.Add(
                "Status",
                typeof(string)
            );

            table.Columns.Add(
                "StatusCss",
                typeof(string)
            );

            return table;
        }

        private DataTable GetSavedSlots(
            SqlConnection connection,
            string staffId,
            DateTime selectedDate)
        {
            DataTable table = new DataTable();

            const string sql = @"
                SELECT
                    Start_Time,
                    End_Time,
                    Availability
                FROM Trainer_Schedule
                WHERE Staff_ID = @Staff_ID
                  AND Session_Date = @Session_Date
                ORDER BY Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = selectedDate.Date;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        private static bool GetSavedAvailability(
            DataTable savedSlots,
            TimeSpan start,
            TimeSpan end)
        {
            foreach (DataRow row in savedSlots.Rows)
            {
                TimeSpan savedStart =
                    (TimeSpan)row["Start_Time"];

                TimeSpan savedEnd =
                    (TimeSpan)row["End_Time"];

                if (savedStart == start &&
                    savedEnd == end)
                {
                    return row["Availability"]
                        .ToString()
                        .Trim()
                        .Equals(
                            "Available",
                            StringComparison.OrdinalIgnoreCase
                        );
                }
            }

            return false;
        }

        private int GetBookingCount(
            SqlConnection connection,
            string staffId,
            DateTime selectedDate,
            TimeSpan start,
            TimeSpan end)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @Staff_ID
                  AND T.Session_Date = @Session_Date
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) = 'Pending'
                  AND TI.Start_Time < @End_Time
                  AND TI.End_Time > @Start_Time;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.Parameters.Add(
                    "@Session_Date",
                    SqlDbType.Date
                ).Value = selectedDate.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = start;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = end;

                return Convert.ToInt32(
                    command.ExecuteScalar()
                );
            }
        }

        private void BindFilteredSlots()
        {
            DataTable table = SlotTable;

            if (table == null)
            {
                gvTimeSlots.DataSource = null;
                gvTimeSlots.DataBind();
                return;
            }

            string filter =
                ddlSlotFilter.SelectedValue;

            DataView view =
                new DataView(table);

            if (filter == "Available")
            {
                view.RowFilter =
                    "Status = 'Available'";
            }
            else if (filter == "Not Available")
            {
                view.RowFilter =
                    "Status = 'Not Available'";
            }
            else if (filter == "Booked")
            {
                view.RowFilter =
                    "Status = 'Booked'";
            }

            gvTimeSlots.DataSource = view;
            gvTimeSlots.DataBind();
        }

        private void UpdateSlotSummary(DataTable table)
        {
            int available = 0;
            int booked = 0;

            foreach (DataRow row in table.Rows)
            {
                string status =
                    row["Status"].ToString();

                if (status == "Available")
                {
                    available++;
                }
                else if (status == "Booked")
                {
                    booked++;
                }
            }

            lblAvailableCount.Text =
                available.ToString();

            lblBookedCount.Text =
                booked.ToString();
        }

        protected void btnSaveAvailability_Click(
            object sender,
            EventArgs e)
        {
            ClearMessage();

            DateTime selectedDate;

            if (!DateTime.TryParse(
                    txtDate.Text,
                    out selectedDate)
                || selectedDate.Date < DateTime.Today)
            {
                ShowError(
                    "Please select today or a future date."
                );

                return;
            }

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string staffId =
                        GetTrainerStaffId(connection);

                    if (string.IsNullOrWhiteSpace(staffId))
                    {
                        ShowError(
                            "Your trainer account could not be found."
                        );

                        return;
                    }

                    using (SqlTransaction transaction =
                           connection.BeginTransaction(
                               IsolationLevel.Serializable))
                    {
                        try
                        {
                            foreach (GridViewRow row
                                in gvTimeSlots.Rows)
                            {
                                CheckBox checkbox =
                                    row.FindControl(
                                        "chkAvailable"
                                    ) as CheckBox;

                                DataKey key =
                                    gvTimeSlots.DataKeys[
                                        row.RowIndex
                                    ];

                                if (checkbox == null ||
                                    key == null ||
                                    !checkbox.Enabled)
                                {
                                    continue;
                                }

                                TimeSpan start =
                                    TimeSpan.Parse(
                                        key.Values[
                                            "SlotStart"
                                        ].ToString()
                                    );

                                TimeSpan end =
                                    TimeSpan.Parse(
                                        key.Values[
                                            "SlotEnd"
                                        ].ToString()
                                    );

                                if (!checkbox.Checked &&
                                    HasPendingBooking(
                                        connection,
                                        transaction,
                                        staffId,
                                        selectedDate.Date,
                                        start,
                                        end))
                                {
                                    throw new InvalidOperationException(
                                        start.ToString(@"hh\:mm")
                                        + " - "
                                        + end.ToString(@"hh\:mm")
                                        + " contains a pending booking."
                                    );
                                }

                                SaveTimeSlot(
                                    connection,
                                    transaction,
                                    staffId,
                                    selectedDate.Date,
                                    start,
                                    end,
                                    checkbox.Checked
                                        ? "Available"
                                        : "Not Available"
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

                LoadTimeSlots(selectedDate.Date);

                ShowSuccess(
                    "Your availability was saved successfully."
                );
            }
            catch (InvalidOperationException exception)
            {
                ShowError(exception.Message);
            }
            catch
            {
                ShowError(
                    "Availability could not be saved."
                );
            }
        }

        private bool HasPendingBooking(
            SqlConnection connection,
            SqlTransaction transaction,
            string staffId,
            DateTime date,
            TimeSpan start,
            TimeSpan end)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM Training_Session AS T
                INNER JOIN Time_Slot AS TI
                    ON TI.Time_ID = T.Time_ID
                WHERE T.Staff_ID = @Staff_ID
                  AND T.Session_Date = @Session_Date
                  AND ISNULL(
                      T.Session_Status,
                      'Pending'
                  ) = 'Pending'
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
                ).Value = date.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = start;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = end;

                return Convert.ToInt32(
                    command.ExecuteScalar()
                ) > 0;
            }
        }

        private void SaveTimeSlot(
            SqlConnection connection,
            SqlTransaction transaction,
            string staffId,
            DateTime date,
            TimeSpan start,
            TimeSpan end,
            string availability)
        {
            const string sql = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM Trainer_Schedule
                    WHERE Staff_ID = @Staff_ID
                      AND Session_Date = @Session_Date
                      AND Start_Time = @Start_Time
                      AND End_Time = @End_Time
                )
                BEGIN
                    UPDATE Trainer_Schedule
                    SET Availability = @Availability
                    WHERE Staff_ID = @Staff_ID
                      AND Session_Date = @Session_Date
                      AND Start_Time = @Start_Time
                      AND End_Time = @End_Time;
                END
                ELSE
                BEGIN
                    INSERT INTO Trainer_Schedule
                    (
                        Staff_ID,
                        Session_Date,
                        Start_Time,
                        End_Time,
                        Availability
                    )
                    VALUES
                    (
                        @Staff_ID,
                        @Session_Date,
                        @Start_Time,
                        @End_Time,
                        @Availability
                    );
                END;";

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
                ).Value = date.Date;

                command.Parameters.Add(
                    "@Start_Time",
                    SqlDbType.Time
                ).Value = start;

                command.Parameters.Add(
                    "@End_Time",
                    SqlDbType.Time
                ).Value = end;

                command.Parameters.Add(
                    "@Availability",
                    SqlDbType.VarChar,
                    50
                ).Value = availability;

                command.ExecuteNonQuery();
            }
        }

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblMessage.CssClass =
                "trainer-message";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "trainer-message schedule-message-success";
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "trainer-message schedule-message-error";
        }
    }
}