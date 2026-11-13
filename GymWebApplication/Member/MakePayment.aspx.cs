using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Member
{
    public partial class MakePayment : Page
    {
        // =========================================================
        // CONNECTION STRING
        // =========================================================

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;
            }
        }


        // =========================================================
        // VALIDATION REGULAR EXPRESSIONS
        // =========================================================

        private static readonly Regex CardholderNameRegex =
            new Regex(
                @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,99}$"
            );

        private static readonly Regex CardNumberRegex =
            new Regex(@"^\d{16}$");

        private static readonly Regex CVVRegex =
            new Regex(@"^\d{3}$");


        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsMemberLoggedIn())
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                txtPaymentDate.Text =
                    DateTime.Today.ToString("yyyy-MM-dd");

                LoadExpiryYears();

                LoadMemberPaymentInformation();
            }
        }


        // =========================================================
        // CHECK MEMBER LOGIN
        // =========================================================

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


        // =========================================================
        // LOAD EXPIRY YEARS
        // =========================================================

        private void LoadExpiryYears()
        {
            ddlExpiryYear.Items.Clear();

            ddlExpiryYear.Items.Add(
                new ListItem(
                    "Select year",
                    ""
                )
            );

            int currentYear =
                DateTime.Today.Year;

            for (int year = currentYear;
                 year <= currentYear + 15;
                 year++)
            {
                ddlExpiryYear.Items.Add(
                    new ListItem(
                        year.ToString(),
                        year.ToString()
                    )
                );
            }
        }


        // =========================================================
        // LOAD MEMBER AND MEMBERSHIP INFORMATION
        // =========================================================

        private void LoadMemberPaymentInformation()
        {
            string memberId =
                Session["MemberID"]
                    .ToString()
                    .Trim();

            const string sql = @"
                SELECT
                    M.Member_ID,
                    M.Member_FName,
                    M.Member_LName,
                    M.Member_Type,
                    MS.Membership_ID,
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

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command =
                           new SqlCommand(
                               sql,
                               connection
                           ))
                    {
                        command.Parameters.Add(
                            "@Member_ID",
                            SqlDbType.VarChar,
                            20
                        ).Value = memberId;

                        connection.Open();

                        using (SqlDataReader reader =
                               command.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                ShowError(
                                    "Member information could not be found."
                                );

                                btnSubmitPayment.Enabled = false;

                                return;
                            }


                            // =====================================
                            // MEMBER NAME
                            // =====================================

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


                            // =====================================
                            // MEMBER ID
                            // =====================================

                            lblMemberID.Text =
                                reader["Member_ID"]
                                .ToString();


                            // =====================================
                            // MEMBER TYPE
                            // =====================================

                            lblMemberType.Text =
                                reader["Member_Type"] ==
                                DBNull.Value
                                ? "—"
                                : reader["Member_Type"]
                                    .ToString();


                            // =====================================
                            // CHECK MEMBERSHIP PLAN
                            // =====================================

                            if (reader["Membership_ID"] ==
                                DBNull.Value)
                            {
                                ViewState["MembershipID"] =
                                    null;

                                lblMembershipPlan.Text =
                                    "No matching membership plan";

                                lblPlanDescription.Text =
                                    "Ask the manager to verify "
                                    + "your member type.";

                                lblPlanDuration.Text =
                                    "—";

                                lblPaymentAmount.Text =
                                    "0.00";

                                btnSubmitPayment.Enabled =
                                    false;

                                ShowError(
                                    "No membership plan matches "
                                    + "your member type."
                                );

                                return;
                            }


                            // =====================================
                            // MEMBERSHIP ID
                            // =====================================

                            ViewState["MembershipID"] =
                                reader["Membership_ID"]
                                .ToString();


                            // =====================================
                            // PLAN TYPE
                            // =====================================

                            lblMembershipPlan.Text =
                                Convert.ToString(
                                    reader["Plan_Type"]
                                );


                            // =====================================
                            // PLAN DESCRIPTION
                            // =====================================

                            lblPlanDescription.Text =
                                reader["Plan_Description"] ==
                                DBNull.Value
                                ? "No description available."
                                : reader["Plan_Description"]
                                    .ToString();


                            // =====================================
                            // PLAN DURATION
                            // =====================================

                            lblPlanDuration.Text =
                                Convert.ToString(
                                    reader["Plan_Type"]
                                );


                            // =====================================
                            // PLAN PRICE
                            // =====================================

                            decimal price =
                                reader["Plan_Price"] ==
                                DBNull.Value
                                ? 0
                                : Convert.ToDecimal(
                                    reader["Plan_Price"]
                                );

                            lblPaymentAmount.Text =
                                price.ToString("0.00");


                            // =====================================
                            // ENABLE PAYMENT
                            // =====================================

                            btnSubmitPayment.Enabled =
                                price > 0;
                        }
                    }
                }
            }
            catch
            {
                btnSubmitPayment.Enabled =
                    false;

                ShowError(
                    "Payment information could not be loaded."
                );
            }
        }


        // =========================================================
        // SUBMIT PAYMENT
        // =========================================================

        protected void btnSubmitPayment_Click(
            object sender,
            EventArgs e)
        {
            ClearMessage();

            Page.Validate("PaymentValidation");

            if (!Page.IsValid)
            {
                ShowError(
                    "Please correct the card details before submitting payment."
                );

                return;
            }


            // =====================================================
            // GET MEMBER ID
            // =====================================================

            string memberId =
                Session["MemberID"]
                    .ToString()
                    .Trim();


            // =====================================================
            // GET MEMBERSHIP ID
            // =====================================================

            string membershipId =
                ViewState["MembershipID"] == null
                ? ""
                : ViewState["MembershipID"]
                    .ToString()
                    .Trim();

            if (string.IsNullOrWhiteSpace(
                    membershipId))
            {
                ShowError(
                    "Your membership plan could not be identified."
                );

                return;
            }


            // =====================================================
            // PAYMENT TYPE
            // ONLY MASTERCARD IS ACCEPTED
            // =====================================================

            const string paymentType =
                "Mastercard";


            // =====================================================
            // CARDHOLDER NAME VALIDATION
            // =====================================================

            string cardholderName =
                txtCardholderName.Text.Trim();

            cardholderName =
                Regex.Replace(
                    cardholderName,
                    @"\s+",
                    " "
                );

            if (string.IsNullOrWhiteSpace(
                    cardholderName))
            {
                ShowError(
                    "Cardholder name is required."
                );

                return;
            }

            if (cardholderName.Length < 2 ||
                cardholderName.Length > 100 ||
                !CardholderNameRegex.IsMatch(
                    cardholderName))
            {
                ShowError(
                    "Please enter a valid cardholder name."
                );

                return;
            }


            // =====================================================
            // CARD NUMBER VALIDATION
            // =====================================================

            string cardNumber =
                txtCardNumber.Text.Trim();

            cardNumber =
                cardNumber.Replace(" ", "");

            cardNumber =
                cardNumber.Replace("-", "");

            if (string.IsNullOrWhiteSpace(
                    cardNumber))
            {
                ShowError(
                    "Card number is required."
                );

                return;
            }

            if (!CardNumberRegex.IsMatch(
                    cardNumber))
            {
                ShowError(
                    "Card number must contain exactly 16 digits."
                );

                return;
            }


            // =====================================================
            // MASTERCARD NUMBER VALIDATION
            //
            // Traditional Mastercard:
            // 51 - 55
            //
            // New Mastercard range:
            // 2221 - 2720
            // =====================================================

            if (!IsMastercardNumber(
                    cardNumber))
            {
                ShowError(
                    "Please enter a valid Mastercard number."
                );

                return;
            }


            // =====================================================
            // LUHN CHECK
            //
            // Checks whether the card number has a
            // mathematically valid payment-card structure.
            // =====================================================

            if (!PassesLuhnCheck(
                    cardNumber))
            {
                ShowError(
                    "The Mastercard number entered is invalid."
                );

                return;
            }


            // =====================================================
            // CVV VALIDATION
            // =====================================================

            string cvv =
                txtCVV.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    cvv))
            {
                ShowError(
                    "CVV is required."
                );

                return;
            }

            if (!CVVRegex.IsMatch(cvv))
            {
                ShowError(
                    "CVV must contain exactly 3 digits."
                );

                return;
            }


            // =====================================================
            // EXPIRY MONTH VALIDATION
            // =====================================================

            int expiryMonth;

            if (!int.TryParse(
                    ddlExpiryMonth.SelectedValue,
                    out expiryMonth))
            {
                ShowError(
                    "Please select a valid expiry month."
                );

                return;
            }

            if (expiryMonth < 1 ||
                expiryMonth > 12)
            {
                ShowError(
                    "Please select a valid expiry month."
                );

                return;
            }


            // =====================================================
            // EXPIRY YEAR VALIDATION
            // =====================================================

            int expiryYear;

            if (!int.TryParse(
                    ddlExpiryYear.SelectedValue,
                    out expiryYear))
            {
                ShowError(
                    "Please select a valid expiry year."
                );

                return;
            }

            if (expiryYear <
                DateTime.Today.Year)
            {
                ShowError(
                    "The card has expired."
                );

                return;
            }


            // =====================================================
            // CHECK CARD EXPIRY DATE
            // =====================================================

            DateTime cardExpiry;

            try
            {
                cardExpiry =
                    new DateTime(
                        expiryYear,
                        expiryMonth,
                        DateTime.DaysInMonth(
                            expiryYear,
                            expiryMonth
                        )
                    );
            }
            catch
            {
                ShowError(
                    "Please enter a valid card expiry date."
                );

                return;
            }

            if (cardExpiry.Date <
                DateTime.Today)
            {
                ShowError(
                    "The Mastercard has expired. "
                    + "Please use a card that has not expired."
                );

                return;
            }


            // =====================================================
            // PAYMENT DATE VALIDATION
            // =====================================================

            DateTime paymentDate;

            if (!DateTime.TryParse(
                    txtPaymentDate.Text,
                    out paymentDate))
            {
                ShowError(
                    "The payment date is invalid."
                );

                return;
            }

            paymentDate =
                paymentDate.Date;

            if (paymentDate !=
                DateTime.Today)
            {
                ShowError(
                    "The payment date must be today."
                );

                txtPaymentDate.Text =
                    DateTime.Today
                        .ToString("yyyy-MM-dd");

                return;
            }


            // =====================================================
            // SIMULATED PAYMENT
            //
            // This academic system assumes payment succeeds
            // after all Mastercard details pass validation.
            //
            // Card number and CVV are deliberately NOT saved
            // to the database.
            // =====================================================

            try
            {
                using (SqlConnection connection =
                       new SqlConnection(
                           ConnectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction =
                           connection.BeginTransaction(
                               IsolationLevel.Serializable))
                    {
                        try
                        {
                            // =====================================
                            // GET MEMBERSHIP PLAN
                            // =====================================

                            MembershipPlan plan =
                                GetMembershipPlan(
                                    connection,
                                    transaction,
                                    membershipId
                                );

                            if (plan == null)
                            {
                                throw new
                                    InvalidOperationException(
                                        "The membership plan could not be found."
                                    );
                            }

                            if (plan.Price <= 0)
                            {
                                throw new
                                    InvalidOperationException(
                                        "The membership plan does not have a valid payment amount."
                                    );
                            }


                            // =====================================
                            // INSERT PAYMENT
                            // =====================================

                            InsertPayment(
                                connection,
                                transaction,
                                membershipId,
                                memberId,
                                paymentDate,
                                plan.Price,
                                plan.PlanType,
                                paymentType
                            );


                            // =====================================
                            // ACTIVATE MEMBER
                            // =====================================

                            ActivateMember(
                                connection,
                                transaction,
                                memberId
                            );


                            // =====================================
                            // COMMIT TRANSACTION
                            // =====================================

                            transaction.Commit();


                            // =====================================
                            // UPDATE SESSION
                            // =====================================

                            Session["MemberStatus"] =
                                "Active";

                            Session["PaymentSuccessMessage"] =
                                "Mastercard payment completed successfully.";


                            // =====================================
                            // CLEAR CARD DATA BEFORE REDIRECT
                            // =====================================

                            ClearCardFields();


                            // =====================================
                            // REDIRECT
                            // =====================================

                            Response.Redirect(
                                "~/Member/Payments.aspx",
                                false
                            );

                            Context
                                .ApplicationInstance
                                .CompleteRequest();
                        }
                        catch
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch
                            {
                                // Ignore rollback failure.
                            }

                            throw;
                        }
                    }
                }
            }
            catch
            {
                ShowError(
                    "Payment could not be completed. "
                    + "Please check your details and try again."
                );
            }
        }


        // =========================================================
        // CHECK MASTERCARD NUMBER RANGE
        // =========================================================

        private bool IsMastercardNumber(
            string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(
                    cardNumber))
            {
                return false;
            }

            if (cardNumber.Length != 16)
            {
                return false;
            }

            if (!Regex.IsMatch(
                    cardNumber,
                    @"^\d{16}$"))
            {
                return false;
            }


            // =====================================================
            // RANGE 51 - 55
            // =====================================================

            int firstTwoDigits;

            if (int.TryParse(
                    cardNumber.Substring(0, 2),
                    out firstTwoDigits))
            {
                if (firstTwoDigits >= 51 &&
                    firstTwoDigits <= 55)
                {
                    return true;
                }
            }


            // =====================================================
            // RANGE 2221 - 2720
            // =====================================================

            int firstFourDigits;

            if (int.TryParse(
                    cardNumber.Substring(0, 4),
                    out firstFourDigits))
            {
                if (firstFourDigits >= 2221 &&
                    firstFourDigits <= 2720)
                {
                    return true;
                }
            }

            return false;
        }


        // =========================================================
        // LUHN CARD NUMBER VALIDATION
        // =========================================================

        private bool PassesLuhnCheck(
            string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(
                    cardNumber))
            {
                return false;
            }

            int sum = 0;

            bool doubleDigit = false;

            for (int i = cardNumber.Length - 1;
                 i >= 0;
                 i--)
            {
                int digit =
                    cardNumber[i] - '0';

                if (digit < 0 ||
                    digit > 9)
                {
                    return false;
                }

                if (doubleDigit)
                {
                    digit *= 2;

                    if (digit > 9)
                    {
                        digit -= 9;
                    }
                }

                sum += digit;

                doubleDigit =
                    !doubleDigit;
            }

            return sum % 10 == 0;
        }


        // =========================================================
        // GET MEMBERSHIP PLAN
        // =========================================================

        private MembershipPlan GetMembershipPlan(
            SqlConnection connection,
            SqlTransaction transaction,
            string membershipId)
        {
            const string sql = @"
                SELECT
                    Plan_Price,
                    Plan_Type

                FROM Membership

                WHERE Membership_ID =
                      @Membership_ID;";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Membership_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = membershipId;

                using (SqlDataReader reader =
                       command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new MembershipPlan
                    {
                        Price =
                            Convert.ToDecimal(
                                reader["Plan_Price"]
                            ),

                        PlanType =
                            reader["Plan_Type"]
                                .ToString()
                    };
                }
            }
        }


        // =========================================================
        // INSERT PAYMENT
        // =========================================================

        private void InsertPayment(
            SqlConnection connection,
            SqlTransaction transaction,
            string membershipId,
            string memberId,
            DateTime paymentDate,
            decimal amount,
            string planType,
            string paymentType)
        {
            const string sql = @"
                INSERT INTO Payment
                (
                    Membership_ID,
                    Member_ID,
                    Payment_Date,
                    Payment_Balance,
                    Plan_Duration,
                    Payment_Type
                )
                VALUES
                (
                    @Membership_ID,
                    @Member_ID,
                    @Payment_Date,
                    @Payment_Balance,
                    @Plan_Duration,
                    @Payment_Type
                );";

            using (SqlCommand command =
                   new SqlCommand(
                       sql,
                       connection,
                       transaction))
            {
                command.Parameters.Add(
                    "@Membership_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = membershipId;

                command.Parameters.Add(
                    "@Member_ID",
                    SqlDbType.VarChar,
                    20
                ).Value = memberId;

                command.Parameters.Add(
                    "@Payment_Date",
                    SqlDbType.Date
                ).Value = paymentDate.Date;


                // =============================================
                // PAYMENT AMOUNT
                // =============================================

                SqlParameter balance =
                    command.Parameters.Add(
                        "@Payment_Balance",
                        SqlDbType.Decimal
                    );

                balance.Precision = 18;
                balance.Scale = 2;
                balance.Value = amount;


                // =============================================
                // PLAN
                // =============================================

                command.Parameters.Add(
                    "@Plan_Duration",
                    SqlDbType.VarChar,
                    50
                ).Value = planType;


                // =============================================
                // PAYMENT TYPE
                // ALWAYS MASTERCARD
                // =============================================

                command.Parameters.Add(
                    "@Payment_Type",
                    SqlDbType.VarChar,
                    20
                ).Value = paymentType;


                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // ACTIVATE MEMBER
        // =========================================================

        private void ActivateMember(
            SqlConnection connection,
            SqlTransaction transaction,
            string memberId)
        {
            const string sql = @"
                UPDATE Member

                SET Member_Status = 'Active'

                WHERE Member_ID =
                      @Member_ID;";

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

                command.ExecuteNonQuery();
            }
        }


        // =========================================================
        // CLEAR CARD FIELDS
        // =========================================================

        private void ClearCardFields()
        {
            txtCardholderName.Text =
                "";

            txtCardNumber.Text =
                "";

            txtCVV.Text =
                "";

            ddlExpiryMonth.SelectedIndex =
                0;

            ddlExpiryYear.SelectedIndex =
                0;
        }


        // =========================================================
        // CLEAR MESSAGE
        // =========================================================

        private void ClearMessage()
        {
            lblPaymentMessage.Text =
                "";
        }


        // =========================================================
        // SHOW ERROR
        // =========================================================

        private void ShowError(
            string message)
        {
            lblPaymentMessage.Text =
                message;
        }


        // =========================================================
        // MEMBERSHIP PLAN CLASS
        // =========================================================

        private sealed class MembershipPlan
        {
            public decimal Price
            {
                get;
                set;
            }

            public string PlanType
            {
                get;
                set;
            }
        }
    }
}