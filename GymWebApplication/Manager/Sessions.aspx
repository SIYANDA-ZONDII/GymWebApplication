<%@ Page Title="Manage Training Sessions"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Sessions.aspx.cs"
    Inherits="GymWebApplication.Manager.Sessions" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/ManagerSessions.css") %>" />

    <div class="manager-sessions-page">

        <section class="manager-session-summary">

            <div class="manager-summary-card">

                <span>TOTAL</span>

                <strong>
                    <asp:Label
                        ID="lblTotalSessions"
                        runat="server"
                        Text="0" />
                </strong>

                <p>Displayed session groups</p>

            </div>

            <div class="manager-summary-card">

                <span>UPCOMING</span>

                <strong>
                    <asp:Label
                        ID="lblPendingSessions"
                        runat="server"
                        Text="0" />
                </strong>

                <p>Pending sessions</p>

            </div>

            <div class="manager-summary-card">

                <span>COMPLETED</span>

                <strong>
                    <asp:Label
                        ID="lblCompletedSessions"
                        runat="server"
                        Text="0" />
                </strong>

                <p>Finished sessions</p>

            </div>

            <div class="manager-summary-card">

                <span>CANCELLED</span>

                <strong>
                    <asp:Label
                        ID="lblCancelledSessions"
                        runat="server"
                        Text="0" />
                </strong>

                <p>Cancelled sessions</p>

            </div>

        </section>

        <asp:UpdatePanel
            ID="SessionsUpdatePanel"
            runat="server"
            UpdateMode="Conditional">

            <ContentTemplate>

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="manager-session-message">
                </asp:Label>

                <section class="manager-sessions-section">

                    <div class="manager-session-filters">

                        <div class="manager-filter-field">

                            <label>SEARCH</label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="manager-filter-input"
                                placeholder="Member, trainer or session"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="manager-filter-field">

                            <label>STATUS</label>

                            <asp:DropDownList
                                ID="ddlFilterStatus"
                                runat="server"
                                CssClass="manager-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="All sessions"
                                    Value="All" />

                                <asp:ListItem
                                    Text="Upcoming"
                                    Value="Pending" />

                                <asp:ListItem
                                    Text="Completed"
                                    Value="Completed" />

                                <asp:ListItem
                                    Text="Cancelled"
                                    Value="Cancelled" />

                            </asp:DropDownList>

                        </div>

                        <asp:Button
                            ID="btnClearFilters"
                            runat="server"
                            Text="CLEAR FILTERS"
                            CssClass="manager-clear-button"
                            CausesValidation="false"
                            OnClick="btnClearFilters_Click" />

                    </div>

                    <div class="manager-table-wrapper">

                        <asp:GridView
                            ID="SessionsGrid"
                            runat="server"
                            AutoGenerateColumns="False"
                            GridLines="None"
                            CssClass="manager-sessions-table"
                            EmptyDataText="No sessions match the selected filters."
                            OnRowCommand="SessionsGrid_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="Session_Type"
                                    HeaderText="SESSION" />

                                <asp:BoundField
                                    DataField="Trainer_Name"
                                    HeaderText="TRAINER" />

                                <asp:BoundField
                                    DataField="Session_Date"
                                    HeaderText="DATE"
                                    DataFormatString="{0:dd/MM/yyyy}" />

                                <asp:BoundField
                                    DataField="Start_Time"
                                    HeaderText="START" />

                                <asp:BoundField
                                    DataField="End_Time"
                                    HeaderText="END" />

                                <asp:BoundField
                                    DataField="Session_Duration"
                                    HeaderText="DURATION" />

                                <asp:BoundField
                                    DataField="Member_Count"
                                    HeaderText="MEMBERS" />

                                <asp:BoundField
                                    DataField="Session_Capacity"
                                    HeaderText="CAPACITY" />

                                <asp:TemplateField HeaderText="STATUS">

                                    <ItemTemplate>

                                        <span class='<%# GetStatusClass(
                                            Eval("Session_Status")
                                        ) %>'>
                                            <%# Eval("Session_Status") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ACTIONS">

                                    <ItemTemplate>

                                        <div class="manager-action-buttons">

                                            <asp:Button
                                                ID="btnEditSession"
                                                runat="server"
                                                Text="EDIT"
                                                CssClass="manager-edit-button"
                                                CommandName="EditSession"
                                                CommandArgument='<%# Eval("Session_Key") %>'
                                                Visible='<%# CanManageSession(
                                                    Eval("Session_Status"),
                                                    Eval("Session_Date"),
                                                    Eval("Start_Time")
                                                ) %>' />

                                            <asp:Button
                                                ID="btnCancelSession"
                                                runat="server"
                                                Text="CANCEL"
                                                CssClass="manager-cancel-button"
                                                CommandName="CancelSession"
                                                CommandArgument='<%# Eval("Session_Key") %>'
                                                Visible='<%# CanManageSession(
                                                    Eval("Session_Status"),
                                                    Eval("Session_Date"),
                                                    Eval("Start_Time")
                                                ) %>'
                                                OnClientClick="return confirm('Cancel this entire session for every booked member?');" />

                                            <asp:Label
                                                ID="lblNoAction"
                                                runat="server"
                                                Text="No action"
                                                CssClass="manager-no-action"
                                                Visible='<%# !CanManageSession(
                                                    Eval("Session_Status"),
                                                    Eval("Session_Date"),
                                                    Eval("Start_Time")
                                                ) %>'>
                                            </asp:Label>

                                        </div>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </section>

                <asp:Panel
                    ID="EditPanel"
                    runat="server"
                    CssClass="manager-edit-panel"
                    Visible="false">

                    <div class="manager-edit-card">

                        <div class="manager-edit-heading">

                            <span class="manager-section-label">
                                EDIT SESSION
                            </span>

                            <h2>
                                Update <span>Session.</span>
                            </h2>

                        </div>

                        <div class="manager-edit-grid">

                            <div class="manager-edit-field">

                                <label>SESSION TYPE</label>

                                <asp:Label
                                    ID="lblEditSessionType"
                                    runat="server"
                                    CssClass="manager-readonly-value" />

                            </div>

                            <div class="manager-edit-field">

                                <label>BOOKED MEMBERS</label>

                                <asp:Label
                                    ID="lblEditMemberCount"
                                    runat="server"
                                    CssClass="manager-readonly-value" />

                            </div>

                            <div class="manager-edit-field">

                                <label>TRAINER</label>

                                <asp:DropDownList
                                    ID="ddlEditTrainer"
                                    runat="server"
                                    CssClass="manager-edit-input">
                                </asp:DropDownList>

                            </div>

                            <div class="manager-edit-field">

                                <label>DATE</label>

                                <asp:TextBox
                                    ID="txtSessionDate"
                                    runat="server"
                                    TextMode="Date"
                                    CssClass="manager-edit-input">
                                </asp:TextBox>

                            </div>

                            <div class="manager-edit-field">

                                <label>TIME</label>

                                <asp:DropDownList
                                    ID="ddlTime"
                                    runat="server"
                                    CssClass="manager-edit-input"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlTime_SelectedIndexChanged">
                                </asp:DropDownList>

                            </div>

                            <div class="manager-edit-field">

                                <label>DURATION</label>

                                <asp:Label
                                    ID="lblEditDuration"
                                    runat="server"
                                    CssClass="manager-readonly-value" />

                            </div>

                            <div class="manager-edit-field">

                                <label>STATUS</label>

                                <asp:DropDownList
                                    ID="ddlEditStatus"
                                    runat="server"
                                    CssClass="manager-edit-input">

                                    <asp:ListItem
                                        Text="Pending"
                                        Value="Pending" />

                                    <asp:ListItem
                                        Text="Completed"
                                        Value="Completed" />

                                </asp:DropDownList>

                            </div>

                        </div>

                        <div class="manager-edit-actions">

                            <asp:Button
                                ID="btnUpdateSession"
                                runat="server"
                                Text="SAVE CHANGES"
                                CssClass="manager-save-button"
                                OnClientClick="return confirm('Save these changes for every member in this session?');"
                                OnClick="btnUpdateSession_Click" />

                            <asp:Button
                                ID="btnCloseEdit"
                                runat="server"
                                Text="CLOSE"
                                CssClass="manager-close-button"
                                CausesValidation="false"
                                OnClick="btnCloseEdit_Click" />

                        </div>

                    </div>

                </asp:Panel>

            </ContentTemplate>

        </asp:UpdatePanel>

    </div>

    <style type="text/css">

        .manager-session-summary {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 18px;
            margin: 32px 0;
        }

        .manager-summary-card {
            padding: 22px;
            border-radius: 10px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .manager-summary-card span {
            color: #e49400;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 1px;
        }

        .manager-summary-card strong {
            display: block;
            margin: 7px 0;
            font-size: 32px;
        }

        .manager-summary-card p {
            margin: 0;
            color: #cbd3dc;
        }

        .manager-session-message {
            display: block;
            margin: 18px 0;
            padding: 14px 16px;
            border-radius: 7px;
            background: #eef2f6;
            color: #142235;
            font-weight: 600;
        }

        .manager-session-message:empty {
            display: none;
        }

        .manager-message-success {
            border: 1px solid #8dd4a3;
            background: #e8f7ed;
            color: #146c2e;
        }

        .manager-message-error {
            border: 1px solid #efaaaa;
            background: #fdecec;
            color: #a51d1d;
        }

        .manager-session-filters {
            display: grid;
            grid-template-columns: 2fr 1fr auto;
            gap: 16px;
            align-items: end;
            margin: 25px 0;
            padding: 20px;
            border-radius: 10px;
            background: #eef2f6;
        }

        .manager-filter-field label,
        .manager-edit-field label {
            display: block;
            margin-bottom: 8px;
            color: #142235;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 1px;
        }

        .manager-filter-input,
        .manager-edit-input {
            width: 100%;
            min-height: 45px;
            padding: 10px 12px;
            border: 1px solid #cbd3dc;
            border-radius: 6px;
            background: #ffffff;
            color: #142235;
        }

        .manager-clear-button,
        .manager-save-button,
        .manager-close-button {
            min-height: 45px;
            padding: 10px 17px;
            border: none;
            border-radius: 6px;
            font-weight: 800;
            cursor: pointer;
        }

        .manager-clear-button,
        .manager-close-button {
            background: #142235;
            color: #ffffff;
        }

        .manager-save-button {
            background: #e49400;
            color: #111d2b;
        }

        .manager-table-wrapper {
            width: 100%;
            overflow-x: auto;
            border-radius: 10px;
            box-shadow: 0 8px 25px rgba(0, 0, 0, .18);
        }

        .manager-sessions-table {
            width: 100% !important;
            min-width: 1350px;
            border-collapse: collapse !important;
        }

        .manager-sessions-table th {
            padding: 16px 14px !important;
            border: none !important;
            border-bottom: 4px solid #e49400 !important;
            background: #111d2b !important;
            color: #ffffff !important;
            font-size: 12px;
            text-align: left !important;
            white-space: nowrap;
        }

        .manager-sessions-table td {
            padding: 16px 14px !important;
            border: none !important;
            border-bottom: 1px solid #dfe4ea !important;
            background: #ffffff !important;
            color: #142235 !important;
            white-space: nowrap;
        }

        .manager-sessions-table tr:nth-child(even) td {
            background: #f4f6f9 !important;
        }

        .manager-session-status {
            display: inline-block;
            padding: 7px 11px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 800;
            text-transform: uppercase;
        }

        .manager-status-pending {
            background: #fff2d8;
            color: #9a5a00;
        }

        .manager-status-completed {
            background: #e8f7ed;
            color: #146c2e;
        }

        .manager-status-cancelled {
            background: #fdecec;
            color: #a51d1d;
        }

        .manager-action-buttons {
            display: flex;
            gap: 7px;
        }

        .manager-edit-button,
        .manager-cancel-button {
            padding: 8px 11px;
            border: none;
            border-radius: 5px;
            color: #ffffff;
            font-size: 11px;
            font-weight: 800;
            cursor: pointer;
        }

        .manager-edit-button {
            background: #146c2e;
        }

        .manager-cancel-button {
            background: #c62828;
        }

        .manager-no-action {
            color: #7b8794;
            font-size: 12px;
        }

        .manager-edit-panel {
            margin: 35px 0;
        }

        .manager-edit-card {
            padding: 28px;
            border-radius: 12px;
            background: #ffffff;
            box-shadow: 0 8px 25px rgba(0, 0, 0, .18);
        }

        .manager-edit-grid {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 20px;
            margin-top: 22px;
        }

        .manager-readonly-value {
            display: block;
            min-height: 45px;
            padding: 12px;
            border-radius: 6px;
            background: #eef2f6;
            color: #142235;
            font-weight: 700;
        }

        .manager-edit-actions {
            display: flex;
            gap: 12px;
            margin-top: 24px;
        }

        @media screen and (max-width: 900px) {

            .manager-session-summary,
            .manager-session-filters,
            .manager-edit-grid {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>