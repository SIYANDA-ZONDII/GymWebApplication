using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication
{
    public partial class Complaint : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureSubmissionType();
        }

        private bool IsLoggedInMemberOrTrainer()
        {
            if (!Request.IsAuthenticated || Session["UserRole"] == null)
                return false;

            string role = Session["UserRole"].ToString().Trim();

            return role.Equals("Member", StringComparison.OrdinalIgnoreCase)
                || role.Equals("Trainer", StringComparison.OrdinalIgnoreCase);
        }

        private void ConfigureSubmissionType()
        {
            bool loggedIn = IsLoggedInMemberOrTrainer();

            pnlLoggedInAnonymousOption.Visible = loggedIn;
            pnlAnonymousVisitor.Visible = !loggedIn;
            chkSubmitAnonymously.Checked = !loggedIn;
        }

        protected void btnSubmitComplaint_Click(object sender, EventArgs e)
        {
            Page.Validate("ComplaintValidation");

            if (!Page.IsValid)
                return;

            string category = ddlComplaintCategory.SelectedValue;
            string location = ddlIncidentLocation.SelectedValue;
            string description = txtComplaintDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(description) ||
                description.Length > 2000)
            {
                ShowMessage("Please complete all required fields.", false);
                return;
            }

            bool anonymous = !IsLoggedInMemberOrTrainer()
                || chkSubmitAnonymously.Checked;

            string memberId = null;
            string staffId = null;

            if (!anonymous)
            {
                string role = Session["UserRole"].ToString().Trim();

                if (role.Equals("Member", StringComparison.OrdinalIgnoreCase))
                    memberId = Convert.ToString(Session["MemberID"]).Trim();
                else if (role.Equals("Trainer", StringComparison.OrdinalIgnoreCase))
                    staffId = Convert.ToString(Session["StaffID"]).Trim();

                if (string.IsNullOrWhiteSpace(memberId) &&
                    string.IsNullOrWhiteSpace(staffId))
                {
                    ShowMessage(
                        "Your account could not be identified. Please log in again.",
                        false);
                    return;
                }
            }

            const string sql = @"
                INSERT INTO Complaints
                    (Category, Location, Description, Submitted_At,
                     Is_Anonymous, Member_ID, Staff_ID, Status)
                OUTPUT INSERTED.Complaint_ID
                VALUES
                    (@Category, @Location, @Description, SYSDATETIME(),
                     @IsAnonymous, @MemberID, @StaffID, @Status);";

            try
            {
                string connectionString =
                    ConfigurationManager.ConnectionStrings["DefaultConnection"]
                        .ConnectionString;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Category", SqlDbType.VarChar, 30)
                        .Value = category;

                    command.Parameters.Add("@Location", SqlDbType.VarChar, 50)
                        .Value = location;

                    command.Parameters.Add("@Description", SqlDbType.VarChar, 2000)
                        .Value = description;

                    command.Parameters.Add("@IsAnonymous", SqlDbType.VarChar, 3)
                        .Value = anonymous ? "Yes" : "No";

                    command.Parameters.Add("@MemberID", SqlDbType.VarChar, 9)
                        .Value = memberId == null
                            ? (object)DBNull.Value : memberId;

                    command.Parameters.Add("@StaffID", SqlDbType.VarChar, 6)
                        .Value = staffId == null
                            ? (object)DBNull.Value : staffId;

                    command.Parameters.Add("@Status", SqlDbType.VarChar, 20)
                        .Value = "Submitted";

                    connection.Open();

                    int complaintId =
                        Convert.ToInt32(command.ExecuteScalar());

                    ClearForm();

                    ShowMessage(
                        "Complaint #" + complaintId +
                        " was submitted successfully.",
                        true);
                }
            }
            catch (SqlException)
            {
                ShowMessage(
                    "The complaint could not be saved. Please try again.",
                    false);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblMessage.Text = "";
            lblMessage.Visible = false;
        }

        private void ClearForm()
        {
            ddlComplaintCategory.SelectedIndex = 0;
            ddlIncidentLocation.SelectedIndex = 0;
            txtComplaintDescription.Text = "";
            chkSubmitAnonymously.Checked = !IsLoggedInMemberOrTrainer();
        }

        private void ShowMessage(string message, bool success)
        {
            lblMessage.Text = message;
            lblMessage.Visible = true;

            lblMessage.CssClass = success
                ? "status-message success-message"
                : "status-message validation-summary";
        }
    }
}