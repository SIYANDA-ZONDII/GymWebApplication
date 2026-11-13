using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication.Member
{
    public partial class Membership : Page
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
                LoadMembership();
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

        private void LoadMembership()
        {
            string memberId =
                Session["MemberID"].ToString().Trim();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    MemberInformation member =
                        LoadMemberInformation(
                            connection,
                            memberId
                        );

                    if (member == null)
                    {
                        Response.Redirect(
                            "~/Account/Login.aspx"
                        );

                        return;
                    }

                    DataTable payments =
                        LoadMembershipRecords(
                            connection,
                            memberId
                        );

                    MembershipGrid.DataSource = payments;
                    MembershipGrid.DataBind();

                    DisplayMembership(
                        member,
                        payments
                    );
                }
            }
            catch
            {
                DisplayUnavailableMembership();
            }
        }

        private MemberInformation LoadMemberInformation(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT
                    Member_ID,
                    Member_FName,
                    Member_LName,
                    Member_Type,
                    Member_Status,
                    Member_Join_Date
                FROM Member
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
                        return null;
                    }

                    string firstName =
                        GetReaderString(
                            reader,
                            "Member_FName",
                            ""
                        );

                    string lastName =
                        GetReaderString(
                            reader,
                            "Member_LName",
                            ""
                        );

                    return new MemberInformation
                    {
                        MemberId =
                            GetReaderString(
                                reader,
                                "Member_ID"
                            ),

                        FullName =
                            (firstName + " " + lastName)
                            .Trim(),

                        MemberType =
                            GetReaderString(
                                reader,
                                "Member_Type"
                            ),

                        StoredStatus =
                            GetReaderString(
                                reader,
                                "Member_Status",
                                "Inactive"
                            ),

                        JoinDate =
                            reader["Member_Join_Date"] ==
                            DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(
                                reader["Member_Join_Date"]
                            ).Date
                    };
                }
            }
        }

        private DataTable LoadMembershipRecords(
            SqlConnection connection,
            string memberId)
        {
            DataTable table = new DataTable();

            const string sql = @"
                SELECT
                    P.Payment_ID,
                    P.Member_ID,
                    P.Membership_ID,

                    ISNULL(
                        M.Plan_Type,
                        '—'
                    ) AS Plan_Type,

                    M.Plan_Price,

                    ISNULL(
                        M.Plan_Description,
                        'No description available.'
                    ) AS Plan_Description,

                    P.Payment_Date,
                    P.Payment_Balance,
                    P.Plan_Duration,
                    P.Payment_Type

                FROM Payment AS P

                LEFT JOIN Membership AS M
                    ON M.Membership_ID =
                       P.Membership_ID

                WHERE P.Member_ID = @Member_ID

                ORDER BY
                    P.Payment_Date DESC,
                    P.Payment_ID DESC;";

            using (SqlCommand command =
                   new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            return table;
        }

        private void DisplayMembership(
            MemberInformation member,
            DataTable payments)
        {
            lblMemberID.Text =
                member.MemberId;

            lblMemberName.Text =
                string.IsNullOrWhiteSpace(
                    member.FullName
                )
                ? "Member"
                : member.FullName;

            lblStartDate.Text =
                member.JoinDate.HasValue
                ? member.JoinDate.Value.ToString(
                    "dd MMMM yyyy"
                )
                : "—";

            lblPaymentCount.Text =
                payments.Rows.Count.ToString();

            if (payments.Rows.Count == 0)
            {
                DisplayNoPaymentMembership(member);
                return;
            }

            DataRow latest =
                payments.Rows[0];

            DateTime? expiry =
                CalculateExpiry(payments);

            bool active =
                member.StoredStatus.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase
                )
                && expiry.HasValue
                && expiry.Value.Date >= DateTime.Today;

            lblMembershipID.Text =
                GetRowString(
                    latest,
                    "Membership_ID"
                );

            lblPlanType.Text =
                GetRowString(
                    latest,
                    "Plan_Type"
                );

            lblPlanPrice.Text =
                GetMoney(
                    latest,
                    "Plan_Price"
                );

            lblPlanDescription.Text =
                GetRowString(
                    latest,
                    "Plan_Description"
                );

            lblPlanDuration.Text =
                "1 month per payment";

            lblLastPaymentDate.Text =
                GetDate(
                    latest,
                    "Payment_Date"
                );

            lblExpiryDate.Text =
                expiry.HasValue
                ? expiry.Value.ToString(
                    "dd MMMM yyyy"
                )
                : "—";

            lblPaymentType.Text =
                GetRowString(
                    latest,
                    "Payment_Type"
                );

            lblPaymentBalance.Text =
                GetMoney(
                    latest,
                    "Payment_Balance"
                );

            if (active)
            {
                lblMemberStatus.Text = "ACTIVE";

                lblMemberStatus.CssClass =
                    "membership-status-active";

                lblNotice.Text =
                    "Your membership is active until "
                    + expiry.Value.ToString(
                        "dd MMMM yyyy"
                    )
                    + ". You may book training sessions "
                    + "scheduled on or before this date.";
            }
            else
            {
                lblMemberStatus.Text = "INACTIVE";

                lblMemberStatus.CssClass =
                    "membership-status-inactive";

                if (expiry.HasValue)
                {
                    lblNotice.Text =
                        "Your calculated membership expired on "
                        + expiry.Value.ToString(
                            "dd MMMM yyyy"
                        )
                        + ". Make another payment before "
                        + "booking training.";
                }
                else
                {
                    lblNotice.Text =
                        "Your membership is inactive. "
                        + "Make a payment to activate it.";
                }
            }

            MembershipMessage.Text =
                payments.Rows.Count
                + (payments.Rows.Count == 1
                    ? " payment record was found."
                    : " payment records were found.");
        }

        private void DisplayNoPaymentMembership(
            MemberInformation member)
        {
            lblMemberStatus.Text = "INACTIVE";

            lblMemberStatus.CssClass =
                "membership-status-inactive";

            lblPlanType.Text =
                member.MemberType;

            lblMembershipID.Text = "—";
            lblPlanPrice.Text = "—";

            lblPlanDuration.Text =
                "1 month per payment";

            lblLastPaymentDate.Text = "—";
            lblExpiryDate.Text = "—";
            lblPaymentType.Text = "—";
            lblPaymentBalance.Text = "—";

            lblPlanDescription.Text =
                "No membership payment has been recorded.";

            lblNotice.Text =
                "Your membership is inactive. Make a payment "
                + "before booking a training session.";

            MembershipMessage.Text =
                "No payment records were found.";
        }

        private static DateTime? CalculateExpiry(
            DataTable payments)
        {
            DataView view =
                new DataView(payments);

            view.Sort =
                "Payment_Date ASC, Payment_ID ASC";

            DateTime? expiry = null;

            foreach (DataRowView rowView in view)
            {
                DateTime paymentDate =
                    Convert.ToDateTime(
                        rowView["Payment_Date"]
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

            return expiry;
        }

        private void DisplayUnavailableMembership()
        {
            lblMemberStatus.Text =
                "UNAVAILABLE";

            lblMemberStatus.CssClass =
                "membership-status-inactive";

            lblPlanType.Text = "—";
            lblMembershipID.Text = "—";
            lblPlanPrice.Text = "—";
            lblPlanDuration.Text = "—";
            lblLastPaymentDate.Text = "—";
            lblExpiryDate.Text = "—";
            lblPaymentType.Text = "—";
            lblPaymentBalance.Text = "—";
            lblPaymentCount.Text = "0";

            lblPlanDescription.Text =
                "Membership information could not be loaded.";

            lblNotice.Text =
                "Please refresh the page and try again.";

            MembershipMessage.Text =
                "Membership information is unavailable.";
        }

        private static string GetReaderString(
            SqlDataReader reader,
            string column,
            string emptyValue = "—")
        {
            if (reader[column] == DBNull.Value)
            {
                return emptyValue;
            }

            string value =
                reader[column].ToString().Trim();

            return string.IsNullOrWhiteSpace(value)
                ? emptyValue
                : value;
        }

        private static string GetRowString(
            DataRow row,
            string column)
        {
            if (row[column] == DBNull.Value)
            {
                return "—";
            }

            string value =
                row[column].ToString().Trim();

            return string.IsNullOrWhiteSpace(value)
                ? "—"
                : value;
        }

        private static string GetDate(
            DataRow row,
            string column)
        {
            if (row[column] == DBNull.Value)
            {
                return "—";
            }

            return Convert.ToDateTime(
                row[column]
            ).ToString("dd MMMM yyyy");
        }

        private static string GetMoney(
            DataRow row,
            string column)
        {
            if (row[column] == DBNull.Value)
            {
                return "—";
            }

            return "R "
                + Convert.ToDecimal(
                    row[column]
                ).ToString("N2");
        }

        private sealed class MemberInformation
        {
            public string MemberId { get; set; }

            public string FullName { get; set; }

            public string MemberType { get; set; }

            public string StoredStatus { get; set; }

            public DateTime? JoinDate { get; set; }
        }
    }
}