using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Manager
{
    public partial class Trainers : Page
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

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsManagerLoggedIn())
            {
                Response.Redirect(
                    "~/Account/Login.aspx"
                );

                return;
            }

            if (!IsPostBack)
            {
                LoadSpecialisations();
                LoadTrainers();
            }
        }

        private bool IsManagerLoggedIn()
        {
            if (!Request.IsAuthenticated)
            {
                return false;
            }

            if (
                Context.User != null
                &&
                Context.User.IsInRole("Manager")
            )
            {
                return true;
            }

            return Session["UserRole"] != null
                &&
                Session["UserRole"]
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
            HideStatusMessage();
            LoadTrainers();
        }

        protected void ddlStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            HideStatusMessage();
            LoadTrainers();
        }

        protected void ddlSpecialisation_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            HideStatusMessage();
            LoadTrainers();
        }

        protected void btnClearFilters_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedValue = "All";
            ddlSpecialisation.SelectedValue = "All";

            HideStatusMessage();
            LoadTrainers();
        }

        protected void gvTrainers_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (!e.CommandName.Equals(
                    "ToggleStatus",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string staffId =
                Convert.ToString(
                    e.CommandArgument
                ).Trim();

            if (string.IsNullOrWhiteSpace(staffId))
            {
                ShowStatusMessage(
                    "The selected trainer record is invalid."
                );

                return;
            }

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        UPDATE Staff

                        SET Staff_Status =
                            CASE
                                WHEN ISNULL(
                                    Staff_Status,
                                    'Inactive'
                                ) = 'Active'
                                    THEN 'Inactive'

                                ELSE 'Active'
                            END

                        OUTPUT
                            inserted.Staff_Status

                        WHERE Staff_ID = @Staff_ID
                          AND Staff_Role = 'Trainer';";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Staff_ID",
                            SqlDbType.VarChar,
                            20
                        ).Value = staffId;

                        connection.Open();

                        object result =
                            command.ExecuteScalar();

                        if (
                            result == null
                            ||
                            result == DBNull.Value
                        )
                        {
                            ShowStatusMessage(
                                "The selected trainer could not be found."
                            );

                            return;
                        }

                        string newStatus =
                            result
                                .ToString()
                                .Trim();

                        ShowStatusMessage(
                            "Trainer "
                            + staffId
                            + " is now "
                            + newStatus
                                .ToLowerInvariant()
                            + "."
                        );
                    }
                }
            }
            catch (SqlException)
            {
                ShowStatusMessage(
                    "The trainer status could not be changed because of a database error."
                );
            }
            catch (Exception)
            {
                ShowStatusMessage(
                    "The trainer status could not be changed. Please try again."
                );
            }

            LoadTrainers();
            TrainersUpdatePanel.Update();
        }

        private void LoadSpecialisations()
        {
            ddlSpecialisation.Items.Clear();

            ddlSpecialisation.Items.Add(
                new ListItem(
                    "All specialisations",
                    "All"
                )
            );

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT DISTINCT
                            LTRIM(
                                RTRIM(Specialisation)
                            ) AS Specialisation

                        FROM Staff

                        WHERE Staff_Role = 'Trainer'
                          AND Specialisation IS NOT NULL
                          AND LTRIM(
                              RTRIM(Specialisation)
                          ) <> ''

                        ORDER BY
                            Specialisation;";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string specialisation =
                                    Convert.ToString(
                                        reader[
                                            "Specialisation"
                                        ]
                                    ).Trim();

                                if (
                                    string.IsNullOrWhiteSpace(
                                        specialisation
                                    )
                                )
                                {
                                    continue;
                                }

                                ddlSpecialisation.Items.Add(
                                    new ListItem(
                                        specialisation,
                                        specialisation
                                    )
                                );
                            }
                        }
                    }
                }
            }
            catch
            {
                // The "All specialisations" option remains available.
            }
        }

        private void LoadTrainers()
        {
            DataTable table =
                new DataTable();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    const string sql = @"
                        SELECT
                            S.Staff_ID,

                            LTRIM(
                                RTRIM(
                                    ISNULL(
                                        S.Staff_FName,
                                        ''
                                    )
                                    + ' '
                                    + ISNULL(
                                        S.Staff_LName,
                                        ''
                                    )
                                )
                            ) AS Name,

                            ISNULL(
                                NULLIF(
                                    LTRIM(
                                        RTRIM(
                                            S.Staff_Email
                                        )
                                    ),
                                    ''
                                ),
                                '—'
                            ) AS Email,

                            ISNULL(
                                NULLIF(
                                    LTRIM(
                                        RTRIM(
                                            S.Staff_Phone_Number
                                        )
                                    ),
                                    ''
                                ),
                                '—'
                            ) AS Phone,

                            ISNULL(
                                NULLIF(
                                    LTRIM(
                                        RTRIM(
                                            S.Specialisation
                                        )
                                    ),
                                    ''
                                ),
                                '—'
                            ) AS Specialisation,

                            ISNULL(
                                NULLIF(
                                    LTRIM(
                                        RTRIM(
                                            S.Staff_Status
                                        )
                                    ),
                                    ''
                                ),
                                'Inactive'
                            ) AS Status,

                            (
                                SELECT COUNT(*)

                                FROM Trainer_Schedule AS TS

                                WHERE TS.Staff_ID =
                                      S.Staff_ID

                                  AND TS.Availability =
                                      'Available'

                                  AND
                                  (
                                      TS.Session_Date >
                                      CAST(
                                          GETDATE()
                                          AS DATE
                                      )

                                      OR
                                      (
                                          TS.Session_Date =
                                          CAST(
                                              GETDATE()
                                              AS DATE
                                          )

                                          AND TS.End_Time >
                                          CAST(
                                              GETDATE()
                                              AS TIME
                                          )
                                      )
                                  )
                            ) AS Available_Slots

                        FROM Staff AS S

                        WHERE S.Staff_Role = 'Trainer'

                          AND
                          (
                              @Status = 'All'

                              OR ISNULL(
                                  NULLIF(
                                      LTRIM(
                                          RTRIM(
                                              S.Staff_Status
                                          )
                                      ),
                                      ''
                                  ),
                                  'Inactive'
                              ) = @Status
                          )

                          AND
                          (
                              @Specialisation = 'All'

                              OR ISNULL(
                                  S.Specialisation,
                                  ''
                              ) = @Specialisation
                          )

                          AND
                          (
                              @Search = ''

                              OR S.Staff_ID LIKE
                                 '%' + @Search + '%'

                              OR S.Staff_FName LIKE
                                 '%' + @Search + '%'

                              OR S.Staff_LName LIKE
                                 '%' + @Search + '%'

                              OR
                              (
                                  ISNULL(
                                      S.Staff_FName,
                                      ''
                                  )
                                  + ' '
                                  + ISNULL(
                                      S.Staff_LName,
                                      ''
                                  )
                              ) LIKE
                                '%' + @Search + '%'

                              OR S.Staff_Email LIKE
                                 '%' + @Search + '%'

                              OR S.Staff_Phone_Number LIKE
                                 '%' + @Search + '%'

                              OR S.Staff_Telephone LIKE
                                 '%' + @Search + '%'

                              OR S.Specialisation LIKE
                                 '%' + @Search + '%'
                          )

                        ORDER BY
                            S.Staff_LName,
                            S.Staff_FName,
                            S.Staff_ID;";

                    using (SqlCommand command =
                           new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add(
                            "@Search",
                            SqlDbType.VarChar,
                            100
                        ).Value =
                            txtSearch.Text.Trim();

                        command.Parameters.Add(
                            "@Status",
                            SqlDbType.VarChar,
                            20
                        ).Value =
                            ddlStatus.SelectedValue;

                        command.Parameters.Add(
                            "@Specialisation",
                            SqlDbType.VarChar,
                            100
                        ).Value =
                            ddlSpecialisation
                                .SelectedValue;

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

                ShowStatusMessage(
                    "The trainer records could not be loaded."
                );
            }

            gvTrainers.DataSource = table;
            gvTrainers.DataBind();

            UpdateStatistics(table);

            bool hasTrainers =
                table.Rows.Count > 0;

            gvTrainers.Visible =
                hasTrainers;

            lblNoTrainers.Visible =
                !hasTrainers;
        }

        private void UpdateStatistics(
            DataTable table)
        {
            int active = 0;
            int available = 0;

            foreach (DataRow row in table.Rows)
            {
                string status =
                    Convert.ToString(
                        row["Status"]
                    ).Trim();

                if (status.Equals(
                        "Active",
                        StringComparison.OrdinalIgnoreCase))
                {
                    active++;
                }

                if (
                    row["Available_Slots"] != DBNull.Value
                    &&
                    Convert.ToInt32(
                        row["Available_Slots"]
                    ) > 0
                )
                {
                    available++;
                }
            }

            lblTrainerCount.Text =
                table.Rows.Count.ToString();

            lblActiveTrainerCount.Text =
                active.ToString();

            lblAvailableTrainerCount.Text =
                available.ToString();
        }

        private void ShowStatusMessage(
            string message)
        {
            lblStatusMessage.Text =
                message;

            lblStatusMessage.Visible =
                true;
        }

        private void HideStatusMessage()
        {
            lblStatusMessage.Text = "";
            lblStatusMessage.Visible = false;
        }

        protected string GetStatusClass(
            object value)
        {
            return IsActive(value)
                ? "manager-trainer-status manager-trainer-active"
                : "manager-trainer-status manager-trainer-inactive";
        }

        protected string GetActionText(
            object value)
        {
            return IsActive(value)
                ? "DEACTIVATE"
                : "ACTIVATE";
        }

        protected string GetActionClass(
            object value)
        {
            return IsActive(value)
                ? "manager-status-action manager-action-deactivate"
                : "manager-status-action manager-action-activate";
        }

        protected string GetConfirmation(
            object value)
        {
            string action =
                IsActive(value)
                    ? "deactivate"
                    : "activate";

            return
                "return confirm('Are you sure you want to "
                + action
                + " this trainer?');";
        }

        private static bool IsActive(
            object value)
        {
            return Convert
                .ToString(value)
                .Trim()
                .Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase
                );
        }
    }
}