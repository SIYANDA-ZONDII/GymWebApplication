using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication.Manager
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
            if (!IsManagerLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadMemberTypes();
                LoadMembers();
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
            LoadMembers();
        }

        protected void ddlStatus_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadMembers();
        }

        protected void ddlMemberType_SelectedIndexChanged(
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
            ddlStatus.SelectedValue = "All";
            ddlMemberType.SelectedValue = "All";

            LoadMembers();
        }

        private void LoadMemberTypes()
        {
            ddlMemberType.Items.Clear();

            ddlMemberType.Items.Add(
                new System.Web.UI.WebControls.ListItem(
                    "All member types",
                    "All"
                )
            );

            using (SqlConnection connection =
                   new SqlConnection(ConnectionString))
            {
                const string sql = @"
                    SELECT DISTINCT
                        LTRIM(RTRIM(Member_Type))
                        AS Member_Type
                    FROM Member
                    WHERE Member_Type IS NOT NULL
                      AND LTRIM(RTRIM(Member_Type)) <> ''
                    ORDER BY Member_Type;";

                using (SqlCommand command =
                       new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader =
                           command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string memberType =
                                reader["Member_Type"]
                                .ToString();

                            ddlMemberType.Items.Add(
                                new System.Web.UI.WebControls.ListItem(
                                    memberType,
                                    memberType
                                )
                            );
                        }
                    }
                }
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
                    const string sql = @"
                        SELECT
                            Member_ID,

                            Member_FName + ' ' +
                            Member_LName AS Name,

                            ISNULL(
                                Member_Email,
                                '—'
                            ) AS Email,

                            ISNULL(
                                Member_Phone_Number,
                                '—'
                            ) AS Phone,

                            ISNULL(
                                Member_Type,
                                '—'
                            ) AS Member_Type,

                            ISNULL(
                                Member_Status,
                                'Inactive'
                            ) AS Status

                        FROM Member

                        WHERE
                        (
                            @Status = 'All'
                            OR ISNULL(
                                Member_Status,
                                'Inactive'
                            ) = @Status
                        )

                        AND
                        (
                            @Member_Type = 'All'
                            OR ISNULL(
                                Member_Type,
                                ''
                            ) = @Member_Type
                        )

                        AND
                        (
                            @Search = ''
                            OR Member_ID LIKE
                               '%' + @Search + '%'
                            OR Member_FName LIKE
                               '%' + @Search + '%'
                            OR Member_LName LIKE
                               '%' + @Search + '%'
                            OR
                            (
                                Member_FName + ' ' +
                                Member_LName
                            ) LIKE '%' + @Search + '%'
                            OR Member_Email LIKE
                               '%' + @Search + '%'
                            OR Member_Phone_Number LIKE
                               '%' + @Search + '%'
                            OR Member_Telephone LIKE
                               '%' + @Search + '%'
                            OR Member_Type LIKE
                               '%' + @Search + '%'
                            OR Member_Status LIKE
                               '%' + @Search + '%'
                        )

                        ORDER BY
                            Member_LName,
                            Member_FName,
                            Member_ID;";

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
                            "@Member_Type",
                            SqlDbType.VarChar,
                            50
                        ).Value =
                            ddlMemberType.SelectedValue;

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

            gvMembers.DataSource = table;
            gvMembers.DataBind();

            UpdateStatistics(table);

            bool hasMembers =
                table.Rows.Count > 0;

            gvMembers.Visible = hasMembers;
            lblNoMembers.Visible = !hasMembers;
        }

        private void UpdateStatistics(DataTable table)
        {
            int active = 0;
            int inactive = 0;

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
                else
                {
                    inactive++;
                }
            }

            lblMemberCount.Text =
                table.Rows.Count.ToString();

            lblActiveCount.Text =
                active.ToString();

            lblInactiveCount.Text =
                inactive.ToString();
        }

        protected string GetStatusClass(object value)
        {
            string status =
                Convert.ToString(value).Trim();

            if (status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "manager-member-status "
                    + "manager-member-active";
            }

            return "manager-member-status "
                + "manager-member-inactive";
        }
    }
}