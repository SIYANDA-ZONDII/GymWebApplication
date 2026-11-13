<%@ Page Title="Manage Trainers"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Trainers.aspx.cs"
    Inherits="GymWebApplication.Manager.Trainers" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link rel="stylesheet"
          href="<%= ResolveUrl("~/Content/ManagerTrainers.css") %>" />

    <div class="manager-page">
        <section class="manager-trainers-section">

            <div class="manager-section-heading">

                <span class="manager-section-label">
                    TRAINER MANAGEMENT
                </span>

                <h2>
                    Registered <span>Trainers.</span>
                </h2>

                <p>
                    Search by staff ID, name, email, phone number
                    or specialisation.
                </p>

            </div>

            <section class="manager-trainer-statistics">

                <div class="manager-trainer-stat-card">

                    <span>DISPLAYED</span>

                    <strong>

                        <asp:Label
                            ID="lblTrainerCount"
                            runat="server"
                            Text="0" />

                    </strong>

                    <p>Trainers matching filters</p>

                </div>

                <div class="manager-trainer-stat-card">

                    <span>ACTIVE</span>

                    <strong>

                        <asp:Label
                            ID="lblActiveTrainerCount"
                            runat="server"
                            Text="0" />

                    </strong>

                    <p>Active displayed trainers</p>

                </div>

                <div class="manager-trainer-stat-card">

                    <span>AVAILABLE</span>

                    <strong>

                        <asp:Label
                            ID="lblAvailableTrainerCount"
                            runat="server"
                            Text="0" />

                    </strong>

                    <p>Trainers with future slots</p>

                </div>

            </section>

            <asp:UpdatePanel
                ID="TrainersUpdatePanel"
                runat="server"
                UpdateMode="Conditional">

                <ContentTemplate>

                    <div class="manager-trainer-filters">

                        <div class="manager-trainer-filter-field">

                            <label>
                                SEARCH
                            </label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="manager-trainer-filter-input"
                                placeholder="ID, name, email or phone"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="manager-trainer-filter-field">

                            <label>
                                STATUS
                            </label>

                            <asp:DropDownList
                                ID="ddlStatus"
                                runat="server"
                                CssClass="manager-trainer-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="All statuses"
                                    Value="All" />

                                <asp:ListItem
                                    Text="Active"
                                    Value="Active" />

                                <asp:ListItem
                                    Text="Inactive"
                                    Value="Inactive" />

                            </asp:DropDownList>

                        </div>

                        <div class="manager-trainer-filter-field">

                            <label>
                                SPECIALISATION
                            </label>

                            <asp:DropDownList
                                ID="ddlSpecialisation"
                                runat="server"
                                CssClass="manager-trainer-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSpecialisation_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>

                        <asp:Button
                            ID="btnClearFilters"
                            runat="server"
                            Text="CLEAR FILTERS"
                            CssClass="manager-trainer-clear-button"
                            CausesValidation="false"
                            OnClick="btnClearFilters_Click" />

                    </div>

                    <asp:Label
                        ID="lblStatusMessage"
                        runat="server"
                        CssClass="manager-status-message"
                        Visible="false" />

                    <div class="manager-table-wrapper">

                        <asp:GridView
                            ID="gvTrainers"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="manager-table"
                            GridLines="None"
                            OnRowCommand="gvTrainers_RowCommand"
                            EmptyDataText="No trainers match the selected filters.">

                            <Columns>

                                <asp:BoundField
                                    DataField="Staff_ID"
                                    HeaderText="STAFF ID" />

                                <asp:BoundField
                                    DataField="Name"
                                    HeaderText="TRAINER" />

                                <asp:BoundField
                                    DataField="Email"
                                    HeaderText="EMAIL" />

                                <asp:BoundField
                                    DataField="Phone"
                                    HeaderText="PHONE" />

                                <asp:BoundField
                                    DataField="Specialisation"
                                    HeaderText="SPECIALISATION" />

                                <asp:BoundField
                                    DataField="Available_Slots"
                                    HeaderText="FUTURE SLOTS" />

                                <asp:TemplateField HeaderText="STATUS">

                                    <ItemTemplate>

                                        <span class='<%# GetStatusClass(Eval("Status")) %>'>

                                            <%# Eval("Status") %>

                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ACTION">

                                    <ItemTemplate>

                                        <asp:LinkButton
                                            ID="btnToggleStatus"
                                            runat="server"
                                            CssClass='<%# GetActionClass(Eval("Status")) %>'
                                            Text='<%# GetActionText(Eval("Status")) %>'
                                            CommandName="ToggleStatus"
                                            CommandArgument='<%# Eval("Staff_ID") %>'
                                            CausesValidation="false"
                                            OnClientClick='<%# GetConfirmation(Eval("Status")) %>' />

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                    <asp:Label
                        ID="lblNoTrainers"
                        runat="server"
                        CssClass="manager-empty-message"
                        Text="No trainers match the selected filters."
                        Visible="false">
                    </asp:Label>

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

    <style type="text/css">

        .manager-trainer-statistics {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin: 28px 0;
        }

        .manager-trainer-stat-card {
            padding: 22px;
            border-radius: 10px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .manager-trainer-stat-card span {
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .manager-trainer-stat-card strong {
            display: block;
            margin: 7px 0;
            font-size: 30px;
        }

        .manager-trainer-stat-card p {
            margin: 0;
            color: #cbd3dc;
        }

        .manager-trainer-filters {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr auto;
            gap: 16px;
            align-items: end;
            margin: 24px 0;
            padding: 20px;
            border-radius: 10px;
            background: #eef2f6;
        }

        .manager-trainer-filter-field label {
            display: block;
            margin-bottom: 8px;
            color: #142235;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .manager-trainer-filter-input {
            width: 100%;
            min-height: 45px;
            box-sizing: border-box;
            padding: 10px 12px;
            border: 1px solid #cbd3dc;
            border-radius: 6px;
            background: #ffffff;
            color: #142235;
        }

        .manager-trainer-clear-button {
            min-height: 45px;
            padding: 10px 16px;
            border: none;
            border-radius: 6px;
            background: #142235;
            color: #ffffff;
            font-weight: 900;
            cursor: pointer;
        }

        .manager-trainer-clear-button:hover {
            background: #20354e;
        }

        .manager-trainer-status {
            display: inline-block;
            padding: 7px 11px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 900;
            text-transform: uppercase;
        }

        .manager-trainer-active {
            background: #e8f7ed;
            color: #146c2e;
        }

        .manager-trainer-inactive {
            background: #fdecec;
            color: #a51d1d;
        }

        .manager-status-action {
            display: inline-block;
            min-width: 92px;
            padding: 8px 12px;
            border-radius: 6px;
            color: #ffffff !important;
            font-size: 11px;
            font-weight: 900;
            text-align: center;
            text-decoration: none;
            transition:
                transform 0.15s ease,
                opacity 0.15s ease;
        }

        .manager-status-action:hover {
            color: #ffffff !important;
            opacity: 0.88;
            transform: translateY(-1px);
        }

        .manager-action-deactivate {
            background: #a51d1d;
        }

        .manager-action-activate {
            background: #146c2e;
        }

        .manager-status-message {
            display: block;
            margin: 0 0 16px;
            padding: 12px 16px;
            border-left: 4px solid #e49400;
            border-radius: 5px;
            background: #fff8e8;
            color: #142235;
            font-weight: 700;
        }

        @media screen and (max-width: 950px) {

            .manager-trainer-filters {
                grid-template-columns: 1fr;
            }
        }

        @media screen and (max-width: 700px) {

            .manager-trainer-statistics {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>