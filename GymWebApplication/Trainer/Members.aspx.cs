using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication.Trainer
{
    public partial class Members : Page
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
                LoadMembers();
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

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            LoadMembers();
        }

        protected void ddlMemberStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadMembers();
        }

        protected void ddlSessionFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadMembers();
        }

        protected void btnClearFilters_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            ddlMemberStatus.SelectedValue = "All";
            ddlSessionFilter.SelectedValue = "All";

            LoadMembers();
        }

        private string GetTrainerId(
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
                ).Value = User.Identity.Name;

                object result =
                    command.ExecuteScalar();

                return result == null ||
                       result == DBNull.Value
                    ? null
                    : result.ToString();
            }
        }

        private void LoadMembers()
        {
            DataTable table = new DataTable();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string staffId =
                        GetTrainerId(connection);

                    if (string.IsNullOrWhiteSpace(staffId))
                    {
                        BindEmptyTable(table);
                        return;
                    }

                    AutoCompletePastSessions(
                        connection,
                        staffId
                    );

                    const string sql = @"
                        SELECT
                            M.Member_ID,

                            M.Member_FName + ' ' +
                            M.Member_LName AS Name,

                            ISNULL(
                                M.Member_Email,
                                '—'
                            ) AS Email,

                            ISNULL(
                                M.Member_Phone_Number,
                                '—'
                            ) AS Phone,

                            ISNULL(
                                M.Member_Type,
                                '—'
                            ) AS Member_Type,

                            ISNULL(
                                M.Member_Status,
                                'Inactive'
                            ) AS Status,

                            COUNT(
                                T.Training_ID
                            ) AS Total_Sessions,

                            SUM
                            (
                                CASE
                                    WHEN ISNULL(
                                        T.Session_Status,
                                        'Pending'
                                    ) = 'Pending'
                                        THEN 1
                                    ELSE 0
                                END
                            ) AS Upcoming_Sessions,

                            SUM
                            (
                                CASE
                                    WHEN T.Session_Status =
                                         'Completed'
                                        THEN 1
                                    ELSE 0
                                END
                            ) AS Completed_Sessions,

                            SUM
                            (
                                CASE
                                    WHEN T.Session_Status =
                                         'Cancelled'
                                        THEN 1
                                    ELSE 0
                                END
                            ) AS Cancelled_Sessions

                        FROM Member AS M

                        INNER JOIN Training_Session AS T
                            ON T.Member_ID = M.Member_ID

                        WHERE T.Staff_ID = @Staff_ID

                          AND
                          (
                              @Member_Status = 'All'
                              OR ISNULL(
                                  M.Member_Status,
                                  'Inactive'
                              ) = @Member_Status
                          )

                          AND
                          (
                              @Search = ''
                              OR M.Member_ID LIKE
                                 '%' + @Search + '%'
                              OR M.Member_FName LIKE
                                 '%' + @Search + '%'
                              OR M.Member_LName LIKE
                                 '%' + @Search + '%'
                              OR
                              (
                                  M.Member_FName + ' ' +
                                  M.Member_LName
                              ) LIKE '%' + @Search + '%'
                              OR M.Member_Email LIKE
                                 '%' + @Search + '%'
                              OR M.Member_Phone_Number LIKE
                                 '%' + @Search + '%'
                              OR M.Member_Type LIKE
                                 '%' + @Search + '%'
                          )

                          AND
                          (
                              @Session_Filter = 'All'

                              OR EXISTS
                              (
                                  SELECT 1
                                  FROM Training_Session AS TF
                                  WHERE TF.Member_ID =
                                        M.Member_ID
                                    AND TF.Staff_ID =
                                        @Staff_ID
                                    AND ISNULL(
                                        TF.Session_Status,
                                        'Pending'
                                    ) = @Session_Filter
                              )
                          )

                        GROUP BY
                            M.Member_ID,
                            M.Member_FName,
                            M.Member_LName,
                            M.Member_Email,
                            M.Member_Phone_Number,
                            M.Member_Type,
                            M.Member_Status

                        ORDER BY
                            M.Member_LName,
                            M.Member_FName,
                            M.Member_ID;";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Staff_ID",
                            SqlDbType.VarChar,
                            20
                        ).Value = staffId;

                        command.Parameters.Add(
                            "@Search",
                            SqlDbType.VarChar,
                            100
                        ).Value =
                            txtSearch.Text.Trim();

                        command.Parameters.Add(
                            "@Member_Status",
                            SqlDbType.VarChar,
                            20
                        ).Value =
                            ddlMemberStatus.SelectedValue;

                        command.Parameters.Add(
                            "@Session_Filter",
                            SqlDbType.VarChar,
                            20
                        ).Value =
                            ddlSessionFilter.SelectedValue;

                        using (SqlDataAdapter adapter =
                               new SqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }
                }
            }
            catch
            {
                table.Clear();
            }

            BindTable(table);
        }

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
                    "@Staff_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = staffId;

                command.ExecuteNonQuery();
            }
        }

        private void BindTable(DataTable table)
        {
            gvMembers.DataSource = table;
            gvMembers.DataBind();

            UpdateStatistics(table);

            bool hasMembers =
                table.Rows.Count > 0;

            gvMembers.Visible = hasMembers;
            lblNoMembers.Visible = !hasMembers;
        }

        private void BindEmptyTable(DataTable table)
        {
            table.Clear();
            BindTable(table);
        }

        private void UpdateStatistics(DataTable table)
        {
            int active = 0;
            int withUpcoming = 0;

            foreach (DataRow row in table.Rows)
            {
                string status =
                    row["Status"].ToString().Trim();

                if (status.Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase))
                {
                    active++;
                }

                if (row["Upcoming_Sessions"] != DBNull.Value &&
                    Convert.ToInt32(
                        row["Upcoming_Sessions"]
                    ) > 0)
                {
                    withUpcoming++;
                }
            }

            lblMemberCount.Text =
                table.Rows.Count.ToString();

            lblActiveMemberCount.Text =
                active.ToString();

            lblUpcomingMemberCount.Text =
                withUpcoming.ToString();
        }

        protected string GetStatusClass(object value)
        {
            string status =
                Convert.ToString(value).Trim();

            if (status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "trainer-member-status "
                    + "trainer-member-active";
            }

            return "trainer-member-status "
                + "trainer-member-inactive";
        }
    }
}