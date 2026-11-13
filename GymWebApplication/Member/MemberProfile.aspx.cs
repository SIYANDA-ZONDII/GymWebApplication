using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication.Member
{
    public partial class MemberProfile : Page
    {
        // The old designer file contains controls from the old profile.
        // These declarations provide the new controls until Visual Studio
        // regenerates the designer file.
        protected Panel PaymentNotice, ViewPanel, EditPanel;

        protected Label ProfileMessage, ViewMemberID,
            ViewFirstName, ViewLastName, ViewEmail, ViewDOB,
            ViewMemberType, ViewJoinDate, ViewStatus,
            ViewPhone, ViewTelephone, ViewGender,
            ViewStreetNumber, ViewStreetName, ViewCity,
            ViewProvince, ViewPostalCode, ViewEmergencyName,
            ViewEmergencyNumber, ViewEmergencyRelationship;

        protected TextBox DOBInput, PhoneInput, TelephoneInput,
            StreetNumberInput, StreetNameInput, CityInput,
            PostalCodeInput, EmergencyNameInput,
            EmergencyNumberInput, EmergencyRelationshipInput;

        protected DropDownList GenderInput, ProvinceInput;

        protected Button EditButton, SaveButton, CancelButton;

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
            if (!Request.IsAuthenticated ||
                Session["MemberID"] == null ||
                Session["UserRole"] == null ||
                !Session["UserRole"].ToString().Equals(
                    "Member",
                    StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            PaymentNotice.Visible =
                Request.QueryString["payment"] == "1";

            DOBInput.Attributes["max"] =
                DateTime.Today.AddYears(-18)
                    .ToString("yyyy-MM-dd");

            DOBInput.Attributes["min"] =
                DateTime.Today.AddYears(-120)
                    .ToString("yyyy-MM-dd");

            if (!IsPostBack)
            {
                ViewState["Editing"] = false;
                ShowEditing(false);
                LoadProfile();
            }
            else
            {
                bool editing =
                    ViewState["Editing"] != null &&
                    Convert.ToBoolean(ViewState["Editing"]);

                ShowEditing(editing);
            }
        }

        protected void EditButton_Click(
            object sender,
            EventArgs e)
        {
            ViewState["Editing"] = true;
            ShowEditing(true);
            ProfileMessage.Visible = false;
        }

        protected void CancelButton_Click(
            object sender,
            EventArgs e)
        {
            ViewState["Editing"] = false;
            ShowEditing(false);
            ProfileMessage.Visible = false;

            // Discard edits that were not saved.
            LoadProfile();
        }

        private void ShowEditing(bool editing)
        {
            ViewPanel.Visible = !editing;
            EditPanel.Visible = editing;
        }

        private void LoadProfile()
        {
            const string sql = @"
                SELECT
                    Member_ID,
                    Member_FName,
                    Member_LName,
                    Member_Email,
                    Member_DOB,
                    Member_Type,
                    Member_Join_Date,
                    Member_Status,
                    Member_Phone_Number,
                    Member_Telephone,
                    Member_Gender,
                    Member_Street_Number,
                    Member_Street_Name,
                    Member_City,
                    Member_Province,
                    Member_Postal_Code,
                    Emergency_Contact_Name,
                    Emergency_Contact_Number,
                    Emergency_Contact_Relationship
                FROM Member
                WHERE Member_ID = @MemberID;";

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@MemberID",
                        SqlDbType.VarChar,
                        9).Value =
                        Session["MemberID"].ToString();

                    connection.Open();

                    using (SqlDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            ShowMessage(
                                "Member profile was not found.",
                                false);

                            EditButton.Enabled = false;
                            SaveButton.Enabled = false;
                            return;
                        }

                        ViewMemberID.Text =
                            Display(reader, "Member_ID");

                        ViewFirstName.Text =
                            Display(reader, "Member_FName");

                        ViewLastName.Text =
                            Display(reader, "Member_LName");

                        ViewEmail.Text =
                            Display(reader, "Member_Email");

                        ViewMemberType.Text =
                            Display(reader, "Member_Type");

                        ViewStatus.Text =
                            Display(reader, "Member_Status");

                        ViewPhone.Text =
                            Display(reader, "Member_Phone_Number");

                        ViewTelephone.Text =
                            Display(reader, "Member_Telephone");

                        ViewGender.Text =
                            Display(reader, "Member_Gender");

                        ViewStreetNumber.Text =
                            Display(reader, "Member_Street_Number");

                        ViewStreetName.Text =
                            Display(reader, "Member_Street_Name");

                        ViewCity.Text =
                            Display(reader, "Member_City");

                        ViewProvince.Text =
                            Display(reader, "Member_Province");

                        ViewPostalCode.Text =
                            Display(reader, "Member_Postal_Code");

                        ViewEmergencyName.Text =
                            Display(reader, "Emergency_Contact_Name");

                        ViewEmergencyNumber.Text =
                            Display(reader, "Emergency_Contact_Number");

                        ViewEmergencyRelationship.Text =
                            Display(
                                reader,
                                "Emergency_Contact_Relationship");

                        if (reader["Member_DOB"] == DBNull.Value)
                        {
                            ViewDOB.Text = "Not provided";
                            DOBInput.Text = "";
                        }
                        else
                        {
                            DateTime dob =
                                Convert.ToDateTime(
                                    reader["Member_DOB"]);

                            ViewDOB.Text =
                                dob.ToString("dd MMM yyyy");

                            DOBInput.Text =
                                dob.ToString("yyyy-MM-dd");
                        }

                        ViewJoinDate.Text =
                            reader["Member_Join_Date"] ==
                            DBNull.Value
                                ? "Not provided"
                                : Convert.ToDateTime(
                                    reader["Member_Join_Date"])
                                    .ToString("dd MMM yyyy");

                        PhoneInput.Text =
                            Value(reader, "Member_Phone_Number");

                        TelephoneInput.Text =
                            Value(reader, "Member_Telephone");

                        StreetNumberInput.Text =
                            Value(reader, "Member_Street_Number");

                        StreetNameInput.Text =
                            Value(reader, "Member_Street_Name");

                        CityInput.Text =
                            Value(reader, "Member_City");

                        PostalCodeInput.Text =
                            Value(reader, "Member_Postal_Code");

                        EmergencyNameInput.Text =
                            Value(
                                reader,
                                "Emergency_Contact_Name");

                        EmergencyNumberInput.Text =
                            Value(
                                reader,
                                "Emergency_Contact_Number");

                        EmergencyRelationshipInput.Text =
                            Value(
                                reader,
                                "Emergency_Contact_Relationship");

                        SelectValue(
                            GenderInput,
                            Value(reader, "Member_Gender"));

                        SelectValue(
                            ProvinceInput,
                            Value(reader, "Member_Province"));
                    }
                }
            }
            catch (SqlException)
            {
                ShowMessage(
                    "Your profile could not be loaded. Please try again.",
                    false);
            }
        }

        protected void SaveButton_Click(
            object sender,
            EventArgs e)
        {
            DateTime dateOfBirth;

            // Date of birth is compulsory.
            if (!DateTime.TryParseExact(
                    DOBInput.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateOfBirth) ||
                dateOfBirth.Date >
                    DateTime.Today.AddYears(-18) ||
                dateOfBirth.Date <
                    DateTime.Today.AddYears(-120))
            {
                ShowMessage(
                    "Enter a valid date of birth. You must be at least 18.",
                    false);
                return;
            }

            // Other fields may still be empty while members complete
            // their profiles gradually.
            if (!ValidOptionalPhone(PhoneInput.Text) ||
                !ValidOptionalPhone(TelephoneInput.Text) ||
                !ValidOptionalPhone(EmergencyNumberInput.Text))
            {
                ShowMessage(
                    "Any phone number you enter must have 10 digits and start with 0.",
                    false);
                return;
            }

            if (!ValidOptional(
                    StreetNumberInput.Text,
                    @"^[0-9A-Za-z/-]{1,10}$") ||
                !ValidOptional(
                    StreetNameInput.Text,
                    @"^[A-Za-z0-9À-ÿ][A-Za-z0-9À-ÿ\s.'-]{1,99}$") ||
                !ValidOptional(
                    CityInput.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,49}$") ||
                !ValidOptional(
                    PostalCodeInput.Text,
                    @"^\d{4}$"))
            {
                ShowMessage(
                    "Check the address you entered. Postal codes must contain four digits.",
                    false);
                return;
            }

            if (!ValidGender(GenderInput.SelectedValue) ||
                !ValidProvince(ProvinceInput.SelectedValue) ||
                !ValidOptional(
                    EmergencyNameInput.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,99}$") ||
                !ValidOptional(
                    EmergencyRelationshipInput.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s'-]{1,49}$"))
            {
                ShowMessage(
                    "Check the gender, province and emergency contact details.",
                    false);
                return;
            }

            const string sql = @"
                UPDATE Member
                SET
                    Member_DOB = @DOB,
                    Member_Phone_Number = @Phone,
                    Member_Telephone = @Telephone,
                    Member_Gender = @Gender,
                    Member_Street_Number = @StreetNumber,
                    Member_Street_Name = @StreetName,
                    Member_City = @City,
                    Member_Province = @Province,
                    Member_Postal_Code = @PostalCode,
                    Emergency_Contact_Name = @EmergencyName,
                    Emergency_Contact_Number = @EmergencyNumber,
                    Emergency_Contact_Relationship = @Relationship
                WHERE Member_ID = @MemberID;";

            try
            {
                using (SqlConnection connection =
                    new SqlConnection(ConnectionString))
                using (SqlCommand command =
                    new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@DOB",
                        SqlDbType.Date).Value =
                        dateOfBirth.Date;

                    AddOptionalText(
                        command,
                        "@Phone",
                        10,
                        PhoneInput.Text);

                    AddOptionalText(
                        command,
                        "@Telephone",
                        10,
                        TelephoneInput.Text);

                    AddOptionalText(
                        command,
                        "@Gender",
                        10,
                        GenderInput.SelectedValue);

                    AddOptionalText(
                        command,
                        "@StreetNumber",
                        10,
                        StreetNumberInput.Text);

                    AddOptionalText(
                        command,
                        "@StreetName",
                        100,
                        StreetNameInput.Text);

                    AddOptionalText(
                        command,
                        "@City",
                        50,
                        CityInput.Text);

                    AddOptionalText(
                        command,
                        "@Province",
                        50,
                        ProvinceInput.SelectedValue);

                    AddOptionalText(
                        command,
                        "@PostalCode",
                        10,
                        PostalCodeInput.Text);

                    AddOptionalText(
                        command,
                        "@EmergencyName",
                        100,
                        EmergencyNameInput.Text);

                    AddOptionalText(
                        command,
                        "@EmergencyNumber",
                        10,
                        EmergencyNumberInput.Text);

                    AddOptionalText(
                        command,
                        "@Relationship",
                        50,
                        EmergencyRelationshipInput.Text);

                    command.Parameters.Add(
                        "@MemberID",
                        SqlDbType.VarChar,
                        9).Value =
                        Session["MemberID"].ToString();

                    connection.Open();

                    if (command.ExecuteNonQuery() != 1)
                    {
                        ShowMessage(
                            "The member profile was not found.",
                            false);
                        return;
                    }
                }

                // If payment brought the member here and the profile
                // is complete, Global.asax.cs permits payment.
                // If details are still missing, it redirects back.
                if (Request.QueryString["payment"] == "1" &&
                    ProfileIsComplete())
                {
                    Response.Redirect(
                        "~/Member/MakePayment.aspx");
                    return;
                }

                ViewState["Editing"] = false;
                ShowEditing(false);
                LoadProfile();

                ShowMessage(
                    "Your details were saved. You can edit them again whenever needed.",
                    true);
            }
            catch (SqlException)
            {
                ShowMessage(
                    "Your details could not be saved. Please try again.",
                    false);
            }
        }

        private bool ProfileIsComplete()
        {
            const string sql = @"
                SELECT Member_Phone_Number,
                       Member_Street_Number,
                       Member_Street_Name,
                       Member_City,
                       Member_Province,
                       Member_Postal_Code,
                       Member_Gender,
                       Emergency_Contact_Name,
                       Emergency_Contact_Number,
                       Emergency_Contact_Relationship
                FROM Member
                WHERE Member_ID = @MemberID;";

            using (SqlConnection connection =
                new SqlConnection(ConnectionString))
            using (SqlCommand command =
                new SqlCommand(sql, connection))
            {
                command.Parameters.Add(
                    "@MemberID",
                    SqlDbType.VarChar,
                    9).Value =
                    Session["MemberID"].ToString();

                connection.Open();

                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    if (!reader.Read())
                        return false;

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (reader.IsDBNull(i) ||
                            string.IsNullOrWhiteSpace(
                                Convert.ToString(
                                    reader.GetValue(i))))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void AddOptionalText(
            SqlCommand command,
            string name,
            int length,
            string value)
        {
            command.Parameters.Add(
                name,
                SqlDbType.VarChar,
                length).Value =
                string.IsNullOrWhiteSpace(value)
                    ? (object)DBNull.Value
                    : value.Trim();
        }

        private static bool ValidOptionalPhone(
            string value)
        {
            return string.IsNullOrWhiteSpace(value) ||
                   Regex.IsMatch(
                       value.Trim(),
                       @"^0\d{9}$");
        }

        private static bool ValidOptional(
            string value,
            string pattern)
        {
            return string.IsNullOrWhiteSpace(value) ||
                   Regex.IsMatch(
                       value.Trim(),
                       pattern);
        }

        private static bool ValidGender(
            string value)
        {
            return value == "" ||
                   value == "Male" ||
                   value == "Female" ||
                   value == "Other" ||
                   value == "Prefer not to say";
        }

        private static bool ValidProvince(
            string value)
        {
            return value == "" ||
                   value == "Eastern Cape" ||
                   value == "Free State" ||
                   value == "Gauteng" ||
                   value == "KwaZulu-Natal" ||
                   value == "Limpopo" ||
                   value == "Mpumalanga" ||
                   value == "Northern Cape" ||
                   value == "North West" ||
                   value == "Western Cape";
        }

        private static string Value(
            SqlDataReader reader,
            string column)
        {
            return reader[column] == DBNull.Value
                ? ""
                : Convert.ToString(
                    reader[column]).Trim();
        }

        private static string Display(
            SqlDataReader reader,
            string column)
        {
            string value = Value(reader, column);

            return string.IsNullOrWhiteSpace(value)
                ? "Not provided"
                : HttpUtility.HtmlEncode(value);
        }

        private static void SelectValue(
            DropDownList list,
            string value)
        {
            list.ClearSelection();

            ListItem item =
                list.Items.FindByValue(value);

            if (item != null)
                item.Selected = true;
        }

        private void ShowMessage(
            string message,
            bool success)
        {
            ProfileMessage.Visible = true;

            ProfileMessage.CssClass =
                "member-profile-message " +
                (success
                    ? "member-profile-success"
                    : "member-profile-error");

            ProfileMessage.Text =
                HttpUtility.HtmlEncode(message);
        }
    }
}