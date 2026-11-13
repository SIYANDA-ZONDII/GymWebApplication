<%@ Page Title="My Members"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Members.aspx.cs"
    Inherits="GymWebApplication.Trainer.Members" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/TrainerMembers.css") %>" />

    <div class="trainer-page members-page">

       

        <section class="members-section">

            <div class="members-section-heading">

                <div>

                    <span class="trainer-section-label">
                        ASSIGNED MEMBERS
                    </span>

                    <h2>
                        Your <span>Members.</span>
                    </h2>

                    <p>
                        Each member is displayed once with a summary
                        of their sessions with you.
                    </p>

                </div>

            </div>

            <section class="trainer-member-statistics">

                <div class="trainer-member-stat-card">

                    <span>DISPLAYED</span>

                    <strong>
                        <asp:Label
                            ID="lblMemberCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Members matching filters</p>

                </div>

                <div class="trainer-member-stat-card">

                    <span>ACTIVE MEMBERS</span>

                    <strong>
                        <asp:Label
                            ID="lblActiveMemberCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Active displayed members</p>

                </div>

                <div class="trainer-member-stat-card">

                    <span>WITH UPCOMING</span>

                    <strong>
                        <asp:Label
                            ID="lblUpcomingMemberCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Members with pending sessions</p>

                </div>

            </section>

            <asp:UpdatePanel
                ID="MembersUpdatePanel"
                runat="server"
                UpdateMode="Conditional">

                <ContentTemplate>

                    <div class="trainer-member-filters">

                        <div class="trainer-member-filter-field">

                            <label>SEARCH</label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="trainer-member-filter-input"
                                placeholder="ID, name, email or phone"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="trainer-member-filter-field">

                            <label>MEMBER STATUS</label>

                            <asp:DropDownList
                                ID="ddlMemberStatus"
                                runat="server"
                                CssClass="trainer-member-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlMemberStatus_SelectedIndexChanged">

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

                        <div class="trainer-member-filter-field">

                            <label>SESSION HISTORY</label>

                            <asp:DropDownList
                                ID="ddlSessionFilter"
                                runat="server"
                                CssClass="trainer-member-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSessionFilter_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="All assigned members"
                                    Value="All" />

                                <asp:ListItem
                                    Text="Has upcoming sessions"
                                    Value="Pending" />

                                <asp:ListItem
                                    Text="Has completed sessions"
                                    Value="Completed" />

                                <asp:ListItem
                                    Text="Has cancelled sessions"
                                    Value="Cancelled" />

                            </asp:DropDownList>

                        </div>

                        <asp:Button
                            ID="btnClearFilters"
                            runat="server"
                            Text="CLEAR FILTERS"
                            CssClass="trainer-member-clear-button"
                            CausesValidation="false"
                            OnClick="btnClearFilters_Click" />

                    </div>

                    <div class="trainer-members-table-wrapper">

                        <asp:GridView
                            ID="gvMembers"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="trainer-table"
                            GridLines="None"
                            EmptyDataText="No assigned members match the selected filters.">

                            <Columns>

                                <asp:BoundField
                                    DataField="Member_ID"
                                    HeaderText="MEMBER ID" />

                                <asp:BoundField
                                    DataField="Name"
                                    HeaderText="MEMBER" />

                                <asp:BoundField
                                    DataField="Email"
                                    HeaderText="EMAIL" />

                                <asp:BoundField
                                    DataField="Phone"
                                    HeaderText="PHONE" />

                                <asp:BoundField
                                    DataField="Member_Type"
                                    HeaderText="TYPE" />

                                <asp:BoundField
                                    DataField="Total_Sessions"
                                    HeaderText="TOTAL" />

                                <asp:BoundField
                                    DataField="Upcoming_Sessions"
                                    HeaderText="UPCOMING" />

                                <asp:BoundField
                                    DataField="Completed_Sessions"
                                    HeaderText="COMPLETED" />

                                <asp:TemplateField HeaderText="STATUS">

                                    <ItemTemplate>

                                        <span class='<%# GetStatusClass(
                                            Eval("Status")
                                        ) %>'>
                                            <%# Eval("Status") %>
                                        </span>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                    <asp:Label
                        ID="lblNoMembers"
                        runat="server"
                        CssClass="trainer-empty-message"
                        Text="No assigned members match the selected filters."
                        Visible="false">
                    </asp:Label>

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

    <style type="text/css">

        .trainer-member-statistics {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin: 28px 0;
        }

        .trainer-member-stat-card {
            padding: 22px;
            border-radius: 10px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .trainer-member-stat-card span {
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .trainer-member-stat-card strong {
            display: block;
            margin: 7px 0;
            font-size: 30px;
        }

        .trainer-member-stat-card p {
            margin: 0;
            color: #cbd3dc;
        }

        .trainer-member-filters {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr auto;
            gap: 16px;
            align-items: end;
            margin: 24px 0;
            padding: 20px;
            border-radius: 10px;
            background: #eef2f6;
        }

        .trainer-member-filter-field label {
            display: block;
            margin-bottom: 8px;
            color: #142235;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .trainer-member-filter-input {
            width: 100%;
            min-height: 45px;
            padding: 10px 12px;
            border: 1px solid #cbd3dc;
            border-radius: 6px;
            background: #ffffff;
            color: #142235;
        }

        .trainer-member-clear-button {
            min-height: 45px;
            padding: 10px 16px;
            border: none;
            border-radius: 6px;
            background: #142235;
            color: #ffffff;
            font-weight: 900;
            cursor: pointer;
        }

        .trainer-member-status {
            display: inline-block;
            padding: 7px 11px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 900;
            text-transform: uppercase;
        }

        .trainer-member-active {
            background: #e8f7ed;
            color: #146c2e;
        }

        .trainer-member-inactive {
            background: #fdecec;
            color: #a51d1d;
        }

        @media screen and (max-width: 1000px) {

            .trainer-member-filters {
                grid-template-columns: 1fr;
            }
        }

        @media screen and (max-width: 700px) {

            .trainer-member-statistics {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>