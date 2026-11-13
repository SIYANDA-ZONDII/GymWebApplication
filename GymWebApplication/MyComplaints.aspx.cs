using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication
{
    public partial class MyComplaints : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            string role = Convert.ToString(Session["UserRole"]).Trim();

            if (!role.Equals("Member", StringComparison.OrdinalIgnoreCase) &&
                !role.Equals("Trainer", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
                LoadMyComplaints(role);
        }

        private void LoadMyComplaints(string role)
        {
            bool isMember =
                role.Equals("Member", StringComparison.OrdinalIgnoreCase);

            string accountId = isMember
                ? Convert.ToString(Session["MemberID"]).Trim()
                : Convert.ToString(Session["StaffID"]).Trim();

            if (string.IsNullOrWhiteSpace(accountId))
            {
                lblError.Text =
                    "Your account could not be identified. Please log in again.";
                lblError.Visible = true;
                return;
            }

            const string sql = @"
                SELECT Complaint_ID, Category, Location, Description,
                       Submitted_At, Status,
                       CASE
                           WHEN Manager_Response IS NULL
                                OR LTRIM(RTRIM(Manager_Response)) = ''
                           THEN 'No response yet.'
                           ELSE Manager_Response
                       END AS Display_Response,
                       CASE
                           WHEN Updated_At IS NULL THEN 'Not updated yet'
                           ELSE CONVERT(varchar(16), Updated_At, 120)
                       END AS Display_Updated_At
                FROM Complaints
                WHERE Is_Anonymous = 'No'
                  AND (
                       (@IsMember = 1 AND Member_ID = @AccountID)
                       OR
                       (@IsMember = 0 AND Staff_ID = @AccountID)
                  )
                ORDER BY Submitted_At DESC, Complaint_ID DESC;";

            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["DefaultConnection"]
                        .ConnectionString;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IsMember", SqlDbType.Bit)
                        .Value = isMember;

                    command.Parameters.Add("@AccountID", SqlDbType.VarChar, 9)
                        .Value = accountId;

                    DataTable complaints = new DataTable();

                    using (SqlDataAdapter adapter =
                        new SqlDataAdapter(command))
                    {
                        adapter.Fill(complaints);
                    }

                    rptComplaints.DataSource = complaints;
                    rptComplaints.DataBind();
                    pnlEmpty.Visible = complaints.Rows.Count == 0;
                }
            }
            catch (SqlException)
            {
                lblError.Text =
                    "Your complaints could not be loaded. Please try again.";
                lblError.Visible = true;
            }
        }
    }
}