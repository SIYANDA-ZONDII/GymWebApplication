using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace GymWebApplication.Member
{
    public partial class Payments : Page
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
                ShowPaymentResult();
                LoadPageInformation();
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

        private void ShowPaymentResult()
        {
            if (Session["PaymentSuccessMessage"] == null)
            {
                return;
            }

            lblPaymentMessage.Text =
                Session["PaymentSuccessMessage"]
                .ToString();

            lblPaymentMessage.CssClass =
                "payments-message payments-success";

            Session.Remove(
                "PaymentSuccessMessage"
            );
        }

        private void LoadPageInformation()
        {
            string memberId =
                Session["MemberID"].ToString().Trim();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    LoadMemberPlan(
                        connection,
                        memberId
                    );

                    LoadPayments(
                        connection,
                        memberId
                    );
                }
            }
            catch
            {
                ShowError(
                    "Payment information could not be loaded."
                );

                BindEmptyPayments();
            }
        }

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            ReloadPayments();
        }

        protected void ddlPaymentTypeFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            ReloadPayments();
        }

        protected void btnApplyFilters_Click(
            object sender,
            EventArgs e)
        {
            DateTime fromDate;
            DateTime toDate;

            if (!string.IsNullOrWhiteSpace(
                    txtFromDate.Text)
                && !DateTime.TryParse(
                    txtFromDate.Text,
                    out fromDate))
            {
                ShowError(
                    "Please enter a valid from date."
                );

                return;
            }

            if (!string.IsNullOrWhiteSpace(
                    txtToDate.Text)
                && !DateTime.TryParse(
                    txtToDate.Text,
                    out toDate))
            {
                ShowError(
                    "Please enter a valid to date."
                );

                return;
            }

            if (DateTime.TryParse(
                    txtFromDate.Text,
                    out fromDate)
                && DateTime.TryParse(
                    txtToDate.Text,
                    out toDate)
                && fromDate.Date > toDate.Date)
            {
                ShowError(
                    "The from date cannot be later "
                    + "than the to date."
                );

                return;
            }

            ReloadPayments();
        }

        protected void btnClearFilters_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text = "";
            ddlPaymentTypeFilter.SelectedValue = "All";
            txtFromDate.Text = "";
            txtToDate.Text = "";

            ClearMessage();
            ReloadPayments();
        }

        private void ReloadPayments()
        {
            string memberId =
                Session["MemberID"].ToString().Trim();

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    LoadPayments(
                        connection,
                        memberId
                    );
                }
            }
            catch
            {
                ShowError(
                    "Payment records could not be loaded."
                );

                BindEmptyPayments();
            }
        }

        private void LoadMemberPlan(
            SqlConnection connection,
            string memberId)
        {
            const string sql = @"
                SELECT
                    M.Member_ID,
                    M.Member_FName,
                    M.Member_LName,
                    M.Member_Type,
                    MS.Plan_Type,
                    MS.Plan_Price,
                    MS.Plan_Description

                FROM Member AS M

                LEFT JOIN Membership AS MS
                    ON LOWER(
                        LTRIM(RTRIM(M.Member_Type))
                    )
                    =
                    LOWER(
                        LTRIM(RTRIM(MS.Plan_Type))
                    )

                WHERE M.Member_ID = @Member_ID;";

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
                        lblMemberName.Text = "—";
                        lblMemberID.Text = memberId;
                        lblMembershipType.Text = "—";
                        lblMembershipDescription.Text = "—";
                        lblPlanPrice.Text = "—";
                        return;
                    }

                    string firstName =
                        Convert.ToString(
                            reader["Member_FName"]
                        ).Trim();

                    string lastName =
                        Convert.ToString(
                            reader["Member_LName"]
                        ).Trim();

                    lblMemberName.Text =
                        (firstName + " " + lastName)
                        .Trim();

                    lblMemberID.Text =
                        reader["Member_ID"].ToString();

                    lblMembershipType.Text =
                        reader["Plan_Type"] ==
                        DBNull.Value
                        ? Convert.ToString(
                            reader["Member_Type"]
                        )
                        : reader["Plan_Type"]
                            .ToString();

                    lblMembershipDescription.Text =
                        reader["Plan_Description"] ==
                        DBNull.Value
                        ? "No matching plan description."
                        : reader["Plan_Description"]
                            .ToString();

                    lblPlanPrice.Text =
                        reader["Plan_Price"] ==
                        DBNull.Value
                        ? "—"
                        : "R "
                          + Convert.ToDecimal(
                              reader["Plan_Price"]
                          ).ToString("N2");
                }
            }
        }

        private void LoadPayments(
            SqlConnection connection,
            string memberId)
        {
            DateTime fromDate;
            DateTime toDate;

            bool hasFromDate =
                DateTime.TryParse(
                    txtFromDate.Text,
                    out fromDate
                );

            bool hasToDate =
                DateTime.TryParse(
                    txtToDate.Text,
                    out toDate
                );

            DataTable table = new DataTable();

            const string sql = @"
                SELECT
                    P.Payment_ID,
                    P.Membership_ID,

                    ISNULL(
                        M.Plan_Type,
                        '—'
                    ) AS Plan_Type,

                    P.Payment_Date,
                    P.Payment_Balance,
                    P.Payment_Type

                FROM Payment AS P

                LEFT JOIN Membership AS M
                    ON M.Membership_ID =
                       P.Membership_ID

                WHERE P.Member_ID = @Member_ID

                  AND
                  (
                      @Payment_Type = 'All'
                      OR P.Payment_Type =
                         @Payment_Type
                  )

                  AND
                  (
                      @From_Date IS NULL
                      OR P.Payment_Date >=
                         @From_Date
                  )

                  AND
                  (
                      @To_Date IS NULL
                      OR P.Payment_Date <=
                         @To_Date
                  )

                  AND
                  (
                      @Search = ''
                      OR CAST(
                          P.Payment_ID AS VARCHAR(30)
                      ) LIKE '%' + @Search + '%'
                      OR P.Membership_ID LIKE
                         '%' + @Search + '%'
                      OR P.Payment_Type LIKE
                         '%' + @Search + '%'
                      OR M.Plan_Type LIKE
                         '%' + @Search + '%'
                  )

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

                command.Parameters.Add(
                    "@Payment_Type",
                    SqlDbType.VarChar,
                    20
                ).Value =
                    ddlPaymentTypeFilter.SelectedValue;

                command.Parameters.Add(
                    "@Search",
                    SqlDbType.VarChar,
                    100
                ).Value =
                    txtSearch.Text.Trim();

                SqlParameter fromParameter =
                    command.Parameters.Add(
                        "@From_Date",
                        SqlDbType.Date
                    );

                fromParameter.Value =
                    hasFromDate
                    ? (object)fromDate.Date
                    : DBNull.Value;

                SqlParameter toParameter =
                    command.Parameters.Add(
                        "@To_Date",
                        SqlDbType.Date
                    );

                toParameter.Value =
                    hasToDate
                    ? (object)toDate.Date
                    : DBNull.Value;

                using (SqlDataAdapter adapter =
                       new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            PaymentsGrid.DataSource = table;
            PaymentsGrid.DataBind();

            UpdateStatistics(table);
        }

        private void UpdateStatistics(DataTable table)
        {
            decimal total = 0;

            foreach (DataRow row in table.Rows)
            {
                if (row["Payment_Balance"] != DBNull.Value)
                {
                    total +=
                        Convert.ToDecimal(
                            row["Payment_Balance"]
                        );
                }
            }

            lblPaymentCount.Text =
                table.Rows.Count.ToString();

            lblTotalPaid.Text =
                total.ToString("N2");

            if (table.Rows.Count > 0 &&
                table.Rows[0]["Payment_Date"] != DBNull.Value)
            {
                lblLastPaymentDate.Text =
                    Convert.ToDateTime(
                        table.Rows[0]["Payment_Date"]
                    ).ToString("dd MMM yyyy");
            }
            else
            {
                lblLastPaymentDate.Text = "—";
            }
        }

        private void BindEmptyPayments()
        {
            DataTable table = new DataTable();

            PaymentsGrid.DataSource = table;
            PaymentsGrid.DataBind();

            lblPaymentCount.Text = "0";
            lblTotalPaid.Text = "0.00";
            lblLastPaymentDate.Text = "—";
        }

        private void ClearMessage()
        {
            lblPaymentMessage.Text = "";
            lblPaymentMessage.CssClass =
                "payments-message";
        }

        private void ShowError(string message)
        {
            lblPaymentMessage.Text = message;
            lblPaymentMessage.CssClass =
                "payments-message payments-error";
        }
    }
}