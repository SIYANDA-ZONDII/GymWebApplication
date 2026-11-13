
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace GymWebApplication.Account
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MessagePanel.Visible = false;

                DateOfBirth.Attributes["max"] =
                    DateTime.Today.AddYears(-18)
                        .ToString("yyyy-MM-dd");

                DateOfBirth.Attributes["min"] =
                    DateTime.Today.AddYears(-120)
                        .ToString("yyyy-MM-dd");
            }
        }

        protected void RegisterMember_Click(object sender, EventArgs e)
        {
            MessagePanel.Visible = false;

            string memberId = MemberID.Text.Trim();
            string firstName = FirstName.Text.Trim();
            string lastName = LastName.Text.Trim();
            string email = Email.Text.Trim().ToLowerInvariant();
            string memberType = MemberType.SelectedValue;
            string gender = Gender.SelectedValue;
            string password = Password.Text;

            DateTime dateOfBirth;

            if (!Regex.IsMatch(memberId, @"^(\d{6}|\d{9})$"))
            {
                ShowError("Member ID must be exactly 6 or 9 digits.");
                return;
            }

            if (!Regex.IsMatch(firstName,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s'-]{1,49}$") ||
                !Regex.IsMatch(lastName,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s'-]{1,49}$"))
            {
                ShowError(
                    "Enter valid first and last names of 2 to 50 characters.");
                return;
            }

            if (!DateTime.TryParseExact(
                    DateOfBirth.Text.Trim(),
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateOfBirth) ||
                dateOfBirth.Date > DateTime.Today.AddYears(-18) ||
                dateOfBirth.Date < DateTime.Today.AddYears(-120))
            {
                ShowError(
                    "Enter a valid date of birth. You must be at least 18.");
                return;
            }

            if (memberType != "Normal Student" &&
                memberType != "Sport Science Student" &&
                memberType != "Staff")
            {
                ShowError("Select a valid member type.");
                return;
            }

            // Gender is compulsory.
            if (string.IsNullOrWhiteSpace(gender))
            {
                ShowError("Please select your gender before registering.");
                return;
            }

            if (!ValidGender(gender))
            {
                ShowError("Please select a valid gender.");
                return;
            }

            try
            {
                MailAddress address = new MailAddress(email);

                if (address.Address != email || email.Length > 100)
                {
                    ShowError("Enter a valid email address.");
                    return;
                }
            }
            catch
            {
                ShowError("Enter a valid email address.");
                return;
            }

            if (!ValidOptionalPhone(PhoneNumber.Text) ||
                !ValidOptionalPhone(TelephoneNumber.Text) ||
                !ValidOptionalPhone(EmergencyContactNumber.Text))
            {
                ShowError(
                    "Any phone number you provide must have 10 digits and start with 0.");
                return;
            }

            if (!ValidOptional(
                    StreetNumber.Text,
                    @"^[0-9A-Za-z/-]{1,10}$") ||
                !ValidOptional(
                    StreetName.Text,
                    @"^[A-Za-z0-9À-ÿ][A-Za-z0-9À-ÿ\s.'-]{1,99}$") ||
                !ValidOptional(
                    City.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,49}$") ||
                !ValidOptional(
                    PostalCode.Text,
                    @"^\d{4}$"))
            {
                ShowError(
                    "Check the address details you entered. Postal codes must contain four digits.");
                return;
            }

            if (!ValidProvince(Province.SelectedValue))
            {
                ShowError("Select a valid province.");
                return;
            }

            if (!ValidOptional(
                    EmergencyContactName.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,99}$") ||
                !ValidOptional(
                    EmergencyRelationship.Text,
                    @"^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s'-]{1,49}$"))
            {
                ShowError(
                    "Check the emergency contact details you entered.");
                return;
            }

            if (!Regex.IsMatch(
                    password,
                    @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9\s])\S{8,100}$"))
            {
                ShowError(
                    "Password must have at least 8 characters, uppercase and lowercase letters, a number and a special character.");
                return;
            }

            if (password != ConfirmPassword.Text)
            {
                ShowError("Passwords do not match.");
                return;
            }

            try
            {
                string connectionString = ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlTransaction transaction =
                        connection.BeginTransaction())
                    {
                        try
                        {
                            const string duplicateSql = @"
                                SELECT
                                    (SELECT COUNT(*)
                                     FROM Member
                                     WHERE Member_ID = @MemberID
                                        OR LOWER(Member_Email) = @Email)
                                  + (SELECT COUNT(*)
                                     FROM Login
                                     WHERE LOWER(Username) = @Email);";

                            using (SqlCommand check =
                                new SqlCommand(
                                    duplicateSql,
                                    connection,
                                    transaction))
                            {
                                check.Parameters.Add(
                                    "@MemberID",
                                    SqlDbType.VarChar,
                                    9).Value = memberId;

                                check.Parameters.Add(
                                    "@Email",
                                    SqlDbType.VarChar,
                                    100).Value = email;

                                if (Convert.ToInt32(
                                    check.ExecuteScalar()) > 0)
                                {
                                    transaction.Rollback();

                                    ShowError(
                                        "That member ID or email address is already registered.");
                                    return;
                                }
                            }

                            const string insertMemberSql = @"
                                INSERT INTO Member
                                (
                                    Member_ID,
                                    Member_FName,
                                    Member_LName,
                                    Member_Telephone,
                                    Member_Phone_Number,
                                    Member_Email,
                                    Member_Street_Name,
                                    Member_Street_Number,
                                    Member_City,
                                    Member_Province,
                                    Member_Postal_Code,
                                    Member_Gender,
                                    Member_DOB,
                                    Member_Join_Date,
                                    Member_Status,
                                    Emergency_Contact_Name,
                                    Emergency_Contact_Number,
                                    Emergency_Contact_Relationship,
                                    Member_Type
                                )
                                VALUES
                                (
                                    @MemberID,
                                    @FirstName,
                                    @LastName,
                                    @Telephone,
                                    @Phone,
                                    @Email,
                                    @StreetName,
                                    @StreetNumber,
                                    @City,
                                    @Province,
                                    @PostalCode,
                                    @Gender,
                                    @DOB,
                                    @JoinDate,
                                    @Status,
                                    @EmergencyName,
                                    @EmergencyNumber,
                                    @Relationship,
                                    @MemberType
                                );";

                            using (SqlCommand insert =
                                new SqlCommand(
                                    insertMemberSql,
                                    connection,
                                    transaction))
                            {
                                AddText(insert, "@MemberID", 9, memberId);
                                AddText(insert, "@FirstName", 50, firstName);
                                AddText(insert, "@LastName", 50, lastName);
                                AddText(insert, "@Email", 100, email);
                                AddText(insert, "@MemberType", 50, memberType);
                                AddText(insert, "@Status", 20, "Inactive");

                                AddOptionalText(
                                    insert, "@Telephone", 10,
                                    TelephoneNumber.Text);
                                AddOptionalText(
                                    insert, "@Phone", 10,
                                    PhoneNumber.Text);
                                AddOptionalText(
                                    insert, "@StreetName", 100,
                                    StreetName.Text);
                                AddOptionalText(
                                    insert, "@StreetNumber", 10,
                                    StreetNumber.Text);
                                AddOptionalText(
                                    insert, "@City", 50,
                                    City.Text);
                                AddOptionalText(
                                    insert, "@Province", 50,
                                    Province.SelectedValue);
                                AddOptionalText(
                                    insert, "@PostalCode", 10,
                                    PostalCode.Text);

                                // Save the selected compulsory gender.
                                AddText(
                                    insert,
                                    "@Gender",
                                    50,
                                    gender);

                                AddOptionalText(
                                    insert, "@EmergencyName", 100,
                                    EmergencyContactName.Text);
                                AddOptionalText(
                                    insert, "@EmergencyNumber", 10,
                                    EmergencyContactNumber.Text);
                                AddOptionalText(
                                    insert, "@Relationship", 50,
                                    EmergencyRelationship.Text);

                                insert.Parameters.Add(
                                    "@DOB",
                                    SqlDbType.Date).Value =
                                    dateOfBirth.Date;

                                insert.Parameters.Add(
                                    "@JoinDate",
                                    SqlDbType.Date).Value =
                                    DateTime.Today;

                                insert.ExecuteNonQuery();
                            }

                            // Matches the existing Login.aspx.cs logic.
                            const string insertLoginSql = @"
                                INSERT INTO Login
                                    (Username, Password, UserRole)
                                VALUES
                                    (@Email, @Password, 'Member');";

                            using (SqlCommand insert =
                                new SqlCommand(
                                    insertLoginSql,
                                    connection,
                                    transaction))
                            {
                                AddText(
                                    insert,
                                    "@Email",
                                    100,
                                    email);

                                AddText(
                                    insert,
                                    "@Password",
                                    100,
                                    password);

                                insert.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            try
                            {
                                transaction.Rollback();
                            }
                            catch
                            {
                            }

                            throw;
                        }
                    }
                }

                ShowSuccess(
                    "Registration successful. Log in to view and update your profile.");

                ClearForm();
            }
            catch (SqlException)
            {
                ShowError(
                    "Registration could not be saved. Check whether the member ID or email is already registered.");
            }
            catch (Exception)
            {
                ShowError(
                    "Registration could not be completed. Please try again.");
            }
        }

        private static void AddText(
            SqlCommand command,
            string name,
            int length,
            string value)
        {
            command.Parameters.Add(
                name,
                SqlDbType.VarChar,
                length).Value = value.Trim();
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

        private static bool ValidOptionalPhone(string value)
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

        private static bool ValidGender(string value)
        {
            return value == "Male" ||
                   value == "Female" ||
                   value == "Other" ||
                   value == "Prefer not to say";
        }

        private static bool ValidProvince(string value)
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

        private void ClearForm()
        {
            MemberID.Text = "";
            FirstName.Text = "";
            LastName.Text = "";
            DateOfBirth.Text = "";
            MemberType.SelectedIndex = 0;
            Email.Text = "";

            PhoneNumber.Text = "";
            TelephoneNumber.Text = "";
            Gender.SelectedIndex = 0;

            StreetName.Text = "";
            StreetNumber.Text = "";
            City.Text = "";
            Province.SelectedIndex = 0;
            PostalCode.Text = "";

            EmergencyContactName.Text = "";
            EmergencyContactNumber.Text = "";
            EmergencyRelationship.Text = "";

            Password.Text = "";
            ConfirmPassword.Text = "";
        }

        private void ShowError(string message)
        {
            MessagePanel.Visible = true;
            MessageBox.Attributes["class"] =
                "member-form-message member-form-error";
            MessageText.Text =
                HttpUtility.HtmlEncode(message);
        }

        private void ShowSuccess(string message)
        {
            MessagePanel.Visible = true;
            MessageBox.Attributes["class"] =
                "member-form-message member-form-success";
            MessageText.Text =
                HttpUtility.HtmlEncode(message);
        }
    }
}
