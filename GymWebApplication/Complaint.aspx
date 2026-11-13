<%@ Page Title="Submit Complaint"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Complaint.aspx.cs"
    Inherits="GymWebApplication.Complaint" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .complaint-page {
            min-height: 100vh;
            padding: 55px 20px 80px;
            background: #f3f5f7;
            font-family: Arial, sans-serif;
            color: #142235;
        }

        .complaint-container {
            max-width: 850px;
            margin: 0 auto;
        }

        .complaint-heading {
            margin-bottom: 28px;
        }

        .complaint-heading h1 {
            font-size: 38px;
            font-weight: 800;
            margin: 0 0 10px;
        }

        .complaint-heading p {
            color: #667587;
            line-height: 1.6;
        }

        .complaint-card {
            background: #fff;
            border-radius: 16px;
            padding: 35px;
            box-shadow: 0 10px 35px rgba(20, 34, 53, .08);
        }

        .complaint-card h2 {
            font-size: 23px;
            margin: 0 0 25px;
        }

        .complaint-field {
            margin-bottom: 23px;
        }

        .complaint-field label {
            display: block;
            margin-bottom: 9px;
            font-size: 13px;
            font-weight: 700;
        }

        .required-mark {
            color: #cf6200;
        }

        .complaint-input {
            width: 100%;
            box-sizing: border-box;
            padding: 13px 14px;
            border: 1px solid #cdd5de;
            border-radius: 8px;
            background: #fff;
            font: inherit;
        }

        .complaint-input:focus {
            outline: none;
            border-color: #e49400;
        }

        .complaint-textarea {
            min-height: 155px;
            resize: vertical;
        }

        .anonymous-choice,
        .anonymous-notice {
            padding: 15px 18px;
            margin-bottom: 25px;
            border-radius: 8px;
            background: #fff7e9;
            line-height: 1.5;
        }

        .anonymous-choice p,
        .anonymous-notice p {
            margin: 8px 0 0;
            color: #667587;
            font-size: 13px;
        }

        .validation-error {
            display: block;
            margin-top: 6px;
            color: #b42318;
            font-size: 13px;
        }

        .validation-summary {
            display: block;
            padding: 13px;
            margin-bottom: 20px;
            border-radius: 7px;
            color: #b42318;
            background: #fff0ee;
        }

        .status-message {
            display: block;
            padding: 14px;
            margin-bottom: 20px;
            border-radius: 7px;
        }

        .success-message {
            color: #17643a;
            background: #e9f7ee;
        }

        .complaint-actions {
            display: flex;
            gap: 12px;
            flex-wrap: wrap;
        }

        .complaint-submit,
        .complaint-clear,
        .complaint-my-complaints-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 46px;
            padding: 13px 22px;
            border: 0;
            border-radius: 8px;
            font-size: 13px;
            font-weight: 700;
            cursor: pointer;
            text-decoration: none !important;
            box-sizing: border-box;
            transition: background-color .2s ease, color .2s ease;
        }

        .complaint-submit {
            color: #fff;
            background: #142235;
        }

        .complaint-submit:hover {
            color: #fff;
            background: #203b59;
        }

        .complaint-my-complaints-btn {
            color: #fff !important;
            background: #0d8490;
        }

        .complaint-my-complaints-btn:hover {
            color: #fff !important;
            background: #096b75;
        }

        .complaint-clear {
            color: #142235;
            background: #e9edf2;
        }

        .complaint-clear:hover {
            background: #d8dee7;
        }
    </style>

    <div class="complaint-page">
        <div class="complaint-container">

            <div class="complaint-heading">
                <h1>Submit a Complaint</h1>
                <p>Tell us what happened so gym management can review it.</p>
            </div>

            <div class="complaint-card">
                <h2>Complaint Details</h2>

                <asp:Label ID="lblMessage"
                    runat="server"
                    Visible="false"
                    CssClass="status-message" />

                <asp:ValidationSummary ID="ValidationSummary1"
                    runat="server"
                    ValidationGroup="ComplaintValidation"
                    CssClass="validation-summary"
                    HeaderText="Please correct the following:" />

                <asp:Panel ID="pnlLoggedInAnonymousOption"
                    runat="server"
                    Visible="false"
                    CssClass="anonymous-choice">

                    <asp:CheckBox ID="chkSubmitAnonymously"
                        runat="server"
                        Text="Submit this complaint anonymously" />

                    <p>
                        Leave this unchecked to link the complaint to your
                        account so you can track its progress.
                    </p>
                </asp:Panel>

                <asp:Panel ID="pnlAnonymousVisitor"
                    runat="server"
                    Visible="false"
                    CssClass="anonymous-notice">

                    <strong>Anonymous submission</strong>
                    <p>
                        Since you are not logged in, this complaint will be
                        submitted anonymously.
                    </p>
                </asp:Panel>

                <div class="complaint-field">
                    <label for="<%= ddlComplaintCategory.ClientID %>">
                        Complaint Category <span class="required-mark">*</span>
                    </label>

                    <asp:DropDownList ID="ddlComplaintCategory"
                        runat="server"
                        CssClass="complaint-input">
                        <asp:ListItem Text="Select a category" Value="" />
                        <asp:ListItem Text="Gym Equipment" Value="Equipment" />
                        <asp:ListItem Text="Gym Facility" Value="Facility" />
                        <asp:ListItem Text="Trainer Service" Value="Trainer" />
                        <asp:ListItem Text="Staff Service" Value="Staff" />
                        <asp:ListItem Text="Payment or Membership" Value="Payment" />
                        <asp:ListItem Text="Training Session Booking" Value="Booking" />
                        <asp:ListItem Text="Health or Safety" Value="Safety" />
                        <asp:ListItem Text="Other" Value="Other" />
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvComplaintCategory"
                        runat="server"
                        ControlToValidate="ddlComplaintCategory"
                        InitialValue=""
                        ErrorMessage="Please select a complaint category."
                        ValidationGroup="ComplaintValidation"
                        CssClass="validation-error" />
                </div>

                <div class="complaint-field">
                    <label for="<%= ddlIncidentLocation.ClientID %>">
                        Location <span class="required-mark">*</span>
                    </label>

                    <asp:DropDownList ID="ddlIncidentLocation"
                        runat="server"
                        CssClass="complaint-input">
                        <asp:ListItem Text="Select a location" Value="" />
                        <asp:ListItem Text="Reception Area" Value="Reception" />
                        <asp:ListItem Text="Main Gym Floor" Value="Main Gym" />
                        <asp:ListItem Text="Training Studio" Value="Studio" />
                        <asp:ListItem Text="Changing Room" Value="Changing Room" />
                        <asp:ListItem Text="Website or Online Service" Value="Online" />
                        <asp:ListItem Text="Other Area" Value="Other" />
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvIncidentLocation"
                        runat="server"
                        ControlToValidate="ddlIncidentLocation"
                        InitialValue=""
                        ErrorMessage="Please select a location."
                        ValidationGroup="ComplaintValidation"
                        CssClass="validation-error" />
                </div>

                <div class="complaint-field">
                    <label for="<%= txtComplaintDescription.ClientID %>">
                        Complaint Description <span class="required-mark">*</span>
                    </label>

                    <asp:TextBox ID="txtComplaintDescription"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="7"
                        MaxLength="2000"
                        CssClass="complaint-input complaint-textarea"
                        placeholder="Describe what happened." />

                    <asp:RequiredFieldValidator ID="rfvComplaintDescription"
                        runat="server"
                        ControlToValidate="txtComplaintDescription"
                        ErrorMessage="Please enter your complaint."
                        ValidationGroup="ComplaintValidation"
                        CssClass="validation-error" />
                </div>

                <div class="complaint-actions">
                    <asp:Button ID="btnSubmitComplaint"
                        runat="server"
                        Text="SUBMIT COMPLAINT"
                        CssClass="complaint-submit"
                        ValidationGroup="ComplaintValidation"
                        OnClick="btnSubmitComplaint_Click" />

                    <asp:HyperLink ID="lnkMyComplaints"
                        runat="server"
                        NavigateUrl="~/MyComplaints.aspx"
                        CssClass="complaint-my-complaints-btn">
                        MY COMPLAINTS
                    </asp:HyperLink>

                    <asp:Button ID="btnClear"
                        runat="server"
                        Text="CLEAR"
                        CssClass="complaint-clear"
                        CausesValidation="false"
                        OnClick="btnClear_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>