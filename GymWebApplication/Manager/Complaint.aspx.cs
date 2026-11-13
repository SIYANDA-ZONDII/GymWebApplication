using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Manager
{
    public partial class Complaint : Page
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
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            string role = Convert.ToString(Session["UserRole"]).Trim();

            if (!role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
                LoadComplaints();
        }

        private void LoadComplaints()
        {
            const string sql = @"
                SELECT Complaint_ID, Category, Location, Submitted_At, Status,
                    CASE
                        WHEN Is_Anonymous = 'Yes' THEN 'Anonymous'
                        WHEN Member_ID IS NOT NULL THEN 'Member: ' + Member_ID
                        WHEN Staff_ID IS NOT NULL THEN 'Trainer: ' + Staff_ID
                        ELSE 'Anonymous'
                    END AS Submitted_By
                FROM Complaints
                WHERE (@Status = '' OR Status = @Status)
                  AND (@Search = ''
                       OR CONVERT(varchar(20), Complaint_ID) LIKE @SearchPattern
                       OR Category LIKE @SearchPattern
                       OR Description LIKE @SearchPattern)
                ORDER BY Submitted_At DESC, Complaint_ID DESC;";

            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                string search = txtSearch.Text.Trim();

                command.Parameters.Add("@Status", SqlDbType.VarChar, 20)
                    .Value = ddlFilterStatus.SelectedValue;

                command.Parameters.Add("@Search", SqlDbType.VarChar, 100)
                    .Value = search;

                command.Parameters.Add("@SearchPattern", SqlDbType.VarChar, 102)
                    .Value = "%" + search + "%";

                DataTable complaints = new DataTable();

                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(command))
                {
                    adapter.Fill(complaints);
                }

                gvComplaints.DataSource = complaints;
                gvComplaints.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            pnlDetails.Visible = false;

            try
            {
                LoadComplaints();
            }
            catch (SqlException)
            {
                ShowMessage("Complaints could not be loaded.", false);
            }
        }

        protected void gvComplaints_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (gvComplaints.SelectedDataKey == null)
                return;

            int complaintId =
                Convert.ToInt32(gvComplaints.SelectedDataKey.Value);

            try
            {
                LoadComplaintDetails(complaintId);
            }
            catch (SqlException)
            {
                ShowMessage("Complaint details could not be loaded.", false);
            }
        }

        private void LoadComplaintDetails(int complaintId)
        {
            const string sql = @"
                SELECT Complaint_ID, Category, Location, Description,
                       Is_Anonymous, Member_ID, Staff_ID,
                       Status, Manager_Response
                FROM Complaints
                WHERE Complaint_ID = @ComplaintID;";

            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@ComplaintID", SqlDbType.Int)
                    .Value = complaintId;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        pnlDetails.Visible = false;
                        ShowMessage("This complaint could not be found.", false);
                        return;
                    }

                    lblComplaintId.Text =
                        Convert.ToString(reader["Complaint_ID"]);

                    string submittedBy = "Anonymous";

                    if (Convert.ToString(reader["Is_Anonymous"])
                        .Equals("No", StringComparison.OrdinalIgnoreCase))
                    {
                        if (reader["Member_ID"] != DBNull.Value)
                            submittedBy =
                                "Member: " + Convert.ToString(reader["Member_ID"]);
                        else if (reader["Staff_ID"] != DBNull.Value)
                            submittedBy =
                                "Trainer: " + Convert.ToString(reader["Staff_ID"]);
                    }

                    lblSubmittedBy.Text =
                        Server.HtmlEncode(submittedBy);

                    lblCategoryLocation.Text =
                        Server.HtmlEncode(
                            Convert.ToString(reader["Category"]) + " — " +
                            Convert.ToString(reader["Location"]));

                    lblDescription.Text =
                        Server.HtmlEncode(
                            Convert.ToString(reader["Description"]))
                            .Replace("\r\n", "<br />")
                            .Replace("\n", "<br />");

                    string status = Convert.ToString(reader["Status"]);

                    ListItem statusItem =
                        ddlUpdateStatus.Items.FindByValue(status);

                    ddlUpdateStatus.ClearSelection();

                    if (statusItem != null)
                        statusItem.Selected = true;
                    else
                        ddlUpdateStatus.SelectedValue = "Submitted";

                    txtManagerResponse.Text =
                        reader["Manager_Response"] == DBNull.Value
                            ? ""
                            : Convert.ToString(reader["Manager_Response"]);

                    ViewState["SelectedComplaintID"] = complaintId;
                    pnlDetails.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedComplaintID"] == null)
            {
                ShowMessage("Select a complaint first.", false);
                return;
            }

            string response = txtManagerResponse.Text.Trim();

            if (response.Length > 2000)
            {
                ShowMessage(
                    "The manager response cannot exceed 2000 characters.",
                    false);
                return;
            }

            int complaintId =
                Convert.ToInt32(ViewState["SelectedComplaintID"]);

            const string sql = @"
                UPDATE Complaints
                SET Status = @Status,
                    Manager_Response = @Response,
                    Updated_At = SYSDATETIME()
                WHERE Complaint_ID = @ComplaintID;";

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Status", SqlDbType.VarChar, 20)
                        .Value = ddlUpdateStatus.SelectedValue;

                    command.Parameters.Add("@Response", SqlDbType.VarChar, 2000)
                        .Value = response.Length == 0
                            ? (object)DBNull.Value : response;

                    command.Parameters.Add("@ComplaintID", SqlDbType.Int)
                        .Value = complaintId;

                    connection.Open();

                    int updated = command.ExecuteNonQuery();

                    if (updated == 0)
                    {
                        ShowMessage("This complaint could not be found.", false);
                        pnlDetails.Visible = false;
                        return;
                    }
                }

                LoadComplaints();
                LoadComplaintDetails(complaintId);
                ShowMessage("Complaint #" + complaintId + " was updated.", true);
            }
            catch (SqlException)
            {
                ShowMessage("The complaint could not be updated.", false);
            }
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;
            lblMessage.CssClass = success
                ? "status-message success-message"
                : "status-message error-message";
        }
    }
}