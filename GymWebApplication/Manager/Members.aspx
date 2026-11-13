<%@ Page Title="Manage Members"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Members.aspx.cs"
    Inherits="GymWebApplication.Manager.Members" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/ManagerMembers.css") %>" />

    <div class="manager-page members-page">

        

        <section class="manager-members-section">

            <div class="manager-section-heading">

                <span class="manager-section-label">
                    MEMBER MANAGEMENT
                </span>

                <h2>
                    Registered <span>Members.</span>
                </h2>

                <p>
                    Search using a member’s ID, name, email,
                    telephone number, type or status.
                </p>

            </div>

            <section class="manager-member-statistics">

                <div class="manager-member-stat-card">

                    <span>DISPLAYED</span>

                    <strong>
                        <asp:Label
                            ID="lblMemberCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Members matching filters</p>

                </div>

                <div class="manager-member-stat-card">

                    <span>ACTIVE</span>

                    <strong>
                        <asp:Label
                            ID="lblActiveCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Active displayed members</p>

                </div>

                <div class="manager-member-stat-card">

                    <span>INACTIVE</span>

                    <strong>
                        <asp:Label
                            ID="lblInactiveCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <p>Inactive displayed members</p>

                </div>

            </section>

            <asp:UpdatePanel
                ID="MembersUpdatePanel"
                runat="server"
                UpdateMode="Conditional">

                <ContentTemplate>

                    <div class="manager-member-filters">

                        <div class="manager-filter-field">

                            <label>SEARCH</label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="manager-filter-input"
                                placeholder="ID, name, email or phone"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="manager-filter-field">

                            <label>STATUS</label>

                            <asp:DropDownList
                                ID="ddlStatus"
                                runat="server"
                                CssClass="manager-filter-input"
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

                        <div class="manager-filter-field">

                            <label>MEMBER TYPE</label>

                            <asp:DropDownList
                                ID="ddlMemberType"
                                runat="server"
                                CssClass="manager-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlMemberType_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>

                        <asp:Button
                            ID="btnClearFilters"
                            runat="server"
                            Text="CLEAR FILTERS"
                            CssClass="manager-clear-filter-button"
                            CausesValidation="false"
                            OnClick="btnClearFilters_Click" />

                    </div>

                    <div class="manager-table-wrapper">

                        <asp:GridView
                            ID="gvMembers"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="manager-table"
                            GridLines="None"
                            EmptyDataText="No members match the selected filters.">

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
                        CssClass="manager-empty-message"
                        Text="No members match the selected filters."
                        Visible="false">
                    </asp:Label>

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

    <style type="text/css">

        .manager-member-statistics {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin: 28px 0;
        }

        .manager-member-stat-card {
            padding: 22px;
            border-radius: 10px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .manager-member-stat-card span {
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .manager-member-stat-card strong {
            display: block;
            margin: 7px 0;
            font-size: 30px;
        }

        .manager-member-stat-card p {
            margin: 0;
            color: #cbd3dc;
        }

        .manager-member-filters {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr auto;
            gap: 16px;
            align-items: end;
            margin: 24px 0;
            padding: 20px;
            border-radius: 10px;
            background: #eef2f6;
        }

        .manager-filter-field label {
            display: block;
            margin-bottom: 8px;
            color: #142235;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .manager-filter-input {
            width: 100%;
            min-height: 45px;
            padding: 10px 12px;
            border: 1px solid #cbd3dc;
            border-radius: 6px;
            background: #ffffff;
            color: #142235;
        }

        .manager-clear-filter-button {
            min-height: 45px;
            padding: 10px 16px;
            border: none;
            border-radius: 6px;
            background: #142235;
            color: #ffffff;
            font-weight: 900;
            cursor: pointer;
        }

        .manager-member-status {
            display: inline-block;
            padding: 7px 11px;
            border-radius: 20px;
            font-size: 11px;
            font-weight: 900;
            text-transform: uppercase;
        }

        .manager-member-active {
            background: #e8f7ed;
            color: #146c2e;
        }

        .manager-member-inactive {
            background: #fdecec;
            color: #a51d1d;
        }

        @media screen and (max-width: 950px) {

            .manager-member-filters {
                grid-template-columns: 1fr;
            }
        }

        @media screen and (max-width: 700px) {

            .manager-member-statistics {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>