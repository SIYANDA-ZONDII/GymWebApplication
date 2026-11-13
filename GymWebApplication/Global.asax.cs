using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GymWebApplication
{
    public class Global : HttpApplication
    {
        protected void Application_Start(
            object sender,
            EventArgs e)
        {
            RouteConfig.RegisterRoutes(
                System.Web.Routing.RouteTable.Routes);

            BundleConfig.RegisterBundles(
                System.Web.Optimization.BundleTable.Bundles);
        }

        protected void Application_PostAcquireRequestState(
            object sender,
            EventArgs e)
        {
            HttpContext context =
                HttpContext.Current;

            if (!IsMember(context))
                return;

            string path =
                context.Request
                    .AppRelativeCurrentExecutionFilePath;

            if (!path.Equals(
                    "~/Member/MakePayment.aspx",
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                List<string> missing =
                    GetMissingDetails(
                        context.Session["MemberID"]
                            .ToString());

                if (missing.Count > 0)
                {
                    context.Response.Redirect(
                        VirtualPathUtility.ToAbsolute(
                            "~/Member/MemberProfile.aspx?payment=1"),
                        false);

                    context.ApplicationInstance
                        .CompleteRequest();
                }
            }
            catch (SqlException)
            {
                context.Response.StatusCode = 503;

                context.Response.Write(
                    "We could not verify your member profile. Please try again.");

                context.ApplicationInstance
                    .CompleteRequest();
            }
        }

        protected void Application_PreRequestHandlerExecute(
            object sender,
            EventArgs e)
        {
            Page page =
                Context.CurrentHandler as Page;

            if (page == null ||
                !IsMember(Context))
            {
                return;
            }

            page.PreRender += ShowMemberReminder;
        }

        private void ShowMemberReminder(
            object sender,
            EventArgs e)
        {
            Page page = (Page)sender;

            if (page.Master == null ||
                !IsMember(Context))
            {
                return;
            }

            Literal notice =
                page.Master.FindControl(
                    "ProfileCompletionNotice")
                    as Literal;

            if (notice == null)
                return;

            notice.Text = "";

            try
            {
                List<string> missing =
                    GetMissingDetails(
                        Context.Session["MemberID"]
                            .ToString());

                if (missing.Count == 0)
                    return;

                string profileUrl =
                    VirtualPathUtility.ToAbsolute(
                        "~/Member/MemberProfile.aspx");

                notice.Text =
                    "<div role=\"alert\" style=\"" +
                    "padding:14px 18px;" +
                    "margin:12px 0;" +
                    "background:#fff3cd;" +
                    "color:#594300;" +
                    "border:1px solid #e9ce70;" +
                    "border-radius:7px;\">" +
                    "<strong>Complete your member profile.</strong> " +
                    "Still needed: " +
                    HttpUtility.HtmlEncode(
                        string.Join(", ", missing)) +
                    ". <a href=\"" +
                    HttpUtility.HtmlAttributeEncode(
                        profileUrl) +
                    "\">Update my profile</a>. " +
                    "These details are required before payment. " +
                    "Telephone is optional.</div>";
            }
            catch (SqlException)
            {
                // Payment remains blocked if the database
                // cannot verify the profile.
            }
        }

        private static bool IsMember(
            HttpContext context)
        {
            return context != null &&
                   context.Request.IsAuthenticated &&
                   context.Session != null &&
                   context.Session["MemberID"] != null &&
                   context.Session["UserRole"] != null &&
                   context.Session["UserRole"]
                       .ToString()
                       .Equals(
                           "Member",
                           StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> GetMissingDetails(
            string memberId)
        {
            List<string> missing =
                new List<string>();

            const string sql = @"
                SELECT
                    Member_DOB,
                    Member_Phone_Number,
                    Member_Street_Name,
                    Member_Street_Number,
                    Member_City,
                    Member_Province,
                    Member_Postal_Code,
                    Member_Gender,
                    Emergency_Contact_Name,
                    Emergency_Contact_Number,
                    Emergency_Contact_Relationship
                FROM Member
                WHERE Member_ID = @MemberID;";

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings[
                        "DefaultConnection"]
                    .ConnectionString;

            using (SqlConnection connection =
                new SqlConnection(
                    connectionString))
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@MemberID",
                    SqlDbType.VarChar,
                    9).Value = memberId;

                connection.Open();

                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        missing.Add("member record");
                        return missing;
                    }

                    Check(
                        reader,
                        "Member_DOB",
                        "date of birth",
                        missing);

                    Check(
                        reader,
                        "Member_Phone_Number",
                        "phone number",
                        missing);

                    Check(
                        reader,
                        "Member_Street_Name",
                        "street name",
                        missing);

                    Check(
                        reader,
                        "Member_Street_Number",
                        "street number",
                        missing);

                    Check(
                        reader,
                        "Member_City",
                        "city",
                        missing);

                    Check(
                        reader,
                        "Member_Province",
                        "province",
                        missing);

                    Check(
                        reader,
                        "Member_Postal_Code",
                        "postal code",
                        missing);

                    Check(
                        reader,
                        "Member_Gender",
                        "gender",
                        missing);

                    Check(
                        reader,
                        "Emergency_Contact_Name",
                        "emergency contact name",
                        missing);

                    Check(
                        reader,
                        "Emergency_Contact_Number",
                        "emergency contact number",
                        missing);

                    Check(
                        reader,
                        "Emergency_Contact_Relationship",
                        "emergency contact relationship",
                        missing);
                }
            }

            return missing;
        }

        private static void Check(
            SqlDataReader reader,
            string column,
            string label,
            List<string> missing)
        {
            if (reader[column] == DBNull.Value ||
                string.IsNullOrWhiteSpace(
                    Convert.ToString(
                        reader[column])))
            {
                missing.Add(label);
            }
        }
    }
}