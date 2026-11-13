<%@ Page Title="Manage Complaints"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Complaint.aspx.cs"
    Inherits="GymWebApplication.Manager.Complaint" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .complaints-page {
            min-height: 100vh;
            padding: 45px 20px 80px;
            background: #f3f5f7;
            font-family: Arial, sans-serif;
            color: #142235;
        }

        .complaints-container {
            max-width: 1250px;
            margin: 0 auto;
        }

        .complaints-heading {
            margin-bottom: 28px;
        }

        .complaints-heading h1 {
            margin: 0 0 8px;
            font-size: 36px;
        }

        .complaints-heading p {
            color: #667587;
        }

        .complaints-card {
            padding: 25px;
            margin-bottom: 24px;
            background: #fff;
            border-radius: 14px;
            box-shadow: 0 8px 28px rgba(20, 34, 53, .07);
        }

        .complaints-card h2 {
            margin: 0 0 18px;
            font-size: 22px;
        }

        .complaints-filters {
            display: flex;
            flex-wrap: wrap;
            gap: 12px;
            margin-bottom: 20px;
        }

        .complaints-input {
            box-sizing: border-box;
            padding: 11px 12px;
            border: 1px solid #cdd5de;
            border-radius: 7px;
            font: inherit;
        }

        .complaints-search {
            min-width: 260px;
            flex: 1;
        }

        .complaints-button {
            padding: 11px 18px;
            border: 0;
            border-radius: 7px;
            background: #142235;
            color: #fff;
            font-weight: 700;
            cursor: pointer;
        }

        .complaints-table-wrap {
            overflow-x: auto;
        }

        .complaints-table {
            width: 100%;
            border-collapse: collapse;
        }

        .complaints-table th,
        .complaints-table td {
            padding: 12px;
            border-bottom: 1px solid #e5e9ee;
            text-align: left;
            vertical-align: top;
        }

        .complaints-table th {
            background: #f5f7fa;
            font-size: 12px;
        }

        .complaints-table td {
            font-size: 13px;
        }

        .complaints-table a {
            color: #a46200;
            font-weight: 700;
        }

        .detail-row {
            margin-bottom: 15px;
            line-height: 1.5;
        }

        .detail-row strong {
            display: block;
            margin-bottom: 5px;
        }

        .detail-text {
            padding: 14px;
            border-radius: 7px;
            background: #f5f7fa;
            overflow-wrap: anywhere;
        }

        .response-box {
            width: 100%;
            min-height: 110px;
            box-sizing: border-box;
            resize: vertical;
        }

        .status-message {
            display: block;
            padding: 13px;
            margin-bottom: 18px;
            border-radius: 7px;
        }

        .success-message {
            color: #17643a;
            background: #e9f7ee;
        }

        .error-message {
            color: #b42318;
            background: #fff0ee;
        }
    </style>

    <div class="complaints-page">
        <div class="complaints-container">

            <div class="complaints-heading">
                <h1>Manage Complaints</h1>
                <p>Review complaints and update their progress.</p>
            </div>

            <asp:Label ID="lblMessage"
                runat="server"
                Visible="false"
                CssClass="status-message" />

            <section class="complaints-card">
                <h2>Submitted Complaints</h2>

                <div class="complaints-filters">
                    <asp:TextBox ID="txtSearch"
                        runat="server"
                        CssClass="complaints-input complaints-search"
                        placeholder="Search reference, category or description" />

                    <asp:DropDownList ID="ddlFilterStatus"
                        runat="server"
                        CssClass="complaints-input">
                        <asp:ListItem Text="All statuses" Value="" />
                        <asp:ListItem Text="Submitted" Value="Submitted" />
                        <asp:ListItem Text="In Progress" Value="In Progress" />
                        <asp:ListItem Text="Resolved" Value="Resolved" />
                        <asp:ListItem Text="Closed" Value="Closed" />
                    </asp:DropDownList>

                    <asp:Button ID="btnFilter"
                        runat="server"
                        Text="FILTER"
                        CssClass="complaints-button"
                        OnClick="btnFilter_Click" />
                </div>

                <div class="complaints-table-wrap">
                    <asp:GridView ID="gvComplaints"
                        runat="server"
                        AutoGenerateColumns="false"
                        DataKeyNames="Complaint_ID"
                        CssClass="complaints-table"
                        GridLines="None"
                        EmptyDataText="No complaints found."
                        OnSelectedIndexChanged="gvComplaints_SelectedIndexChanged">

                        <Columns>
                            <asp:BoundField DataField="Complaint_ID"
                                HeaderText="REFERENCE" />

                            <asp:BoundField DataField="Submitted_By"
                                HeaderText="SUBMITTED BY" />

                            <asp:BoundField DataField="Category"
                                HeaderText="CATEGORY" />

                            <asp:BoundField DataField="Location"
                                HeaderText="LOCATION" />

                            <asp:BoundField DataField="Submitted_At"
                                HeaderText="SUBMITTED"
                                DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                            <asp:BoundField DataField="Status"
                                HeaderText="STATUS" />

                            <asp:CommandField ShowSelectButton="true"
                                SelectText="VIEW / UPDATE"
                                HeaderText="ACTION" />
                        </Columns>
                    </asp:GridView>
                </div>
            </section>

            <asp:Panel ID="pnlDetails"
                runat="server"
                Visible="false"
                CssClass="complaints-card">

                <h2>Complaint #<asp:Label ID="lblComplaintId" runat="server" /></h2>

                <div class="detail-row">
                    <strong>Submitted by</strong>
                    <asp:Label ID="lblSubmittedBy" runat="server" />
                </div>

                <div class="detail-row">
                    <strong>Category and location</strong>
                    <asp:Label ID="lblCategoryLocation" runat="server" />
                </div>

                <div class="detail-row">
                    <strong>Description</strong>
                    <div class="detail-text">
                        <asp:Label ID="lblDescription" runat="server" />
                    </div>
                </div>

                <div class="detail-row">
                    <strong>Update status</strong>
                    <asp:DropDownList ID="ddlUpdateStatus"
                        runat="server"
                        CssClass="complaints-input">
                        <asp:ListItem Text="Submitted" Value="Submitted" />
                        <asp:ListItem Text="In Progress" Value="In Progress" />
                        <asp:ListItem Text="Resolved" Value="Resolved" />
                        <asp:ListItem Text="Closed" Value="Closed" />
                    </asp:DropDownList>
                </div>

                <div class="detail-row">
                    <strong>Manager response / progress update</strong>
                    <asp:TextBox ID="txtManagerResponse"
                        runat="server"
                        TextMode="MultiLine"
                        Rows="5"
                        MaxLength="2000"
                        CssClass="complaints-input response-box" />
                </div>

                <asp:Button ID="btnSave"
                    runat="server"
                    Text="SAVE UPDATE"
                    CssClass="complaints-button"
                    OnClick="btnSave_Click" />
            </asp:Panel>
        </div>
    </div>
</asp:Content>