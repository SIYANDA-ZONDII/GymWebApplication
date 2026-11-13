
<%@ Page Title="Manager Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ManagerDashboard.aspx.cs"
    Inherits="GymWebApplication.Manager.ManagerDashboard" %>
<asp:Content ID="ManagerPageStyles"
    ContentPlaceHolderID="PageStyles"
    runat="server">

    <link href="<%: ResolveUrl("~/Content/ManagerDashboard.css") %>"
        rel="stylesheet" />

</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

  

    <div class="manager-page simplified-manager-dashboard">

        <!-- WELCOME -->
        <section class="manager-welcome-section compact-manager-heading">
            <div>
                <span class="manager-label">MANAGER OVERVIEW</span>
                <h1>Welcome Back,
                    <span>
                        <asp:Label ID="lblManagerName" runat="server" Text="Manager" />
                    </span>
                </h1>
                <p>Monitor membership, trainers, complaints, sessions and recorded revenue.</p>
            </div>

            <div class="manager-campus-card">
                <span>UKZN SPORT</span>
                <strong>WESTVILLE CAMPUS</strong>
            </div>
        </section>

        <!-- NOTIFICATIONS: ABOVE THE OVERVIEW -->
        <section class="manager-today-section manager-notifications-section">
            <div class="manager-section-heading manager-heading-row">
                <div>
                    <span class="manager-label">ANNOUNCEMENTS</span>
                    <h2>Gym <span>Notifications.</span></h2>
                </div>

                <asp:Label ID="lblNotificationCount" runat="server"
                    Text="0 unread" CssClass="manager-label" />
            </div>

            <asp:Label ID="lblNotificationEmpty" runat="server"
                Text="You have no announcements at the moment."
                Visible="false" />

            <asp:Repeater ID="rptNotifications" runat="server"
                OnItemCommand="rptNotifications_ItemCommand">
                <ItemTemplate>
                    <div class="manager-notification-item">
                        <div class="manager-notification-content">
                            <h3><%#: Eval("Title") %></h3>
                            <p><%#: Eval("Message") %></p>
                            <small><%# Eval("Created_At", "{0:dd MMM yyyy, HH:mm}") %></small>
                        </div>

                        <asp:Label ID="lblReadStatus" runat="server"
                            Text='<%# Convert.ToBoolean(Eval("Is_Read")) ? "Read" : "Unread" %>'
                            CssClass="manager-notification-status" />

                        <asp:LinkButton ID="btnMarkRead" runat="server"
                            Text="Mark as Read"
                            CommandName="MarkRead"
                            CommandArgument='<%# Eval("Notification_ID") %>'
                            Visible='<%# !Convert.ToBoolean(Eval("Is_Read")) %>'
                            CausesValidation="false" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </section>

        <!-- GYM OVERVIEW -->
        <section class="manager-overview">
            <div class="manager-section-heading manager-heading-row">
                <div>
                    <span class="manager-label">LIVE STATISTICS</span>
                    <h2>Gym <span>Overview.</span></h2>
                </div>
                <p>Current membership, staff and session information.</p>
            </div>

            <div class="manager-stat-grid manager-expanded-grid">

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblMemberCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">TOTAL MEMBERS</div>
                    <p>All registered gym members</p>
                    <a href="<%= ResolveUrl("~/Manager/Members.aspx") %>" class="manager-stat-link">VIEW MEMBERS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblActiveMemberCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">ACTIVE MEMBERS</div>
                    <p>Members currently marked active</p>
                    <a href="<%= ResolveUrl("~/Manager/Members.aspx") %>" class="manager-stat-link">VIEW MEMBERS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblTrainerCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">TOTAL TRAINERS</div>
                    <p>All registered trainers</p>
                    <a href="<%= ResolveUrl("~/Manager/Trainers.aspx") %>" class="manager-stat-link">VIEW TRAINERS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblAvailableTrainerCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">AVAILABLE TRAINERS</div>
                    <p>Trainers with future availability</p>
                    <a href="<%= ResolveUrl("~/Manager/Trainers.aspx") %>" class="manager-stat-link">VIEW TRAINERS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblTodaySessionCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">TODAY'S SESSIONS</div>
                    <p>Session groups scheduled today</p>
                    <a href="<%= ResolveUrl("~/Manager/Sessions.aspx") %>" class="manager-stat-link">VIEW SESSIONS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblUpcomingSessionCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">UPCOMING SESSIONS</div>
                    <p>Pending future session groups</p>
                    <a href="<%= ResolveUrl("~/Manager/Sessions.aspx") %>" class="manager-stat-link">MANAGE SESSIONS →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblCompletedSessionCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">COMPLETED SESSIONS</div>
                    <p>Completed training groups</p>
                    <a href="<%= ResolveUrl("~/Manager/Sessions.aspx") %>" class="manager-stat-link">VIEW HISTORY →</a>
                </div>

                <div class="manager-stat-card">
                    <div class="manager-stat-number">
                        <asp:Label ID="lblCancelledSessionCount" runat="server" Text="0" />
                    </div>
                    <div class="manager-stat-title">CANCELLED SESSIONS</div>
                    <p>Cancelled training groups</p>
                    <a href="<%= ResolveUrl("~/Manager/Sessions.aspx") %>" class="manager-stat-link">VIEW HISTORY →</a>
                </div>

                <div class="manager-stat-card manager-revenue-card">
                    <div class="manager-stat-number manager-revenue-value">
                        R<asp:Label ID="lblRevenue" runat="server" Text="0.00" />
                    </div>
                    <div class="manager-stat-title">RECORDED REVENUE</div>
                    <p>Total payment balance recorded</p>
                    <a href="<%= ResolveUrl("~/Manager/Reports.aspx") %>" class="manager-stat-link">VIEW REPORTS →</a>
                </div>

            </div>
        </section>

        <!-- TODAY'S SESSIONS -->
        <section class="manager-today-section">
            <div class="manager-section-heading manager-heading-row">
                <div>
                    <span class="manager-label">TODAY'S ACTIVITY</span>
                    <h2>Today’s <span>Sessions.</span></h2>
                </div>
                <p>Active session groups scheduled for today.</p>
            </div>

            <div class="manager-today-table-wrapper">
                <asp:GridView ID="TodaySessionsGrid" runat="server"
                    AutoGenerateColumns="False"
                    GridLines="None"
                    CssClass="manager-today-table"
                    EmptyDataText="No active sessions are scheduled for today.">
                    <Columns>
                        <asp:BoundField DataField="Session_Type" HeaderText="SESSION" />
                        <asp:BoundField DataField="Trainer_Name" HeaderText="TRAINER" />
                        <asp:BoundField DataField="Start_Time" HeaderText="START" />
                        <asp:BoundField DataField="End_Time" HeaderText="END" />
                        <asp:BoundField DataField="Member_Count" HeaderText="MEMBERS" />
                        <asp:BoundField DataField="Session_Status" HeaderText="STATUS" />
                    </Columns>
                </asp:GridView>
            </div>
        </section>

    </div>

    <style type="text/css">
        .simplified-manager-dashboard {
            padding-top: 42px;
            padding-bottom: 70px;
        }

        .compact-manager-heading {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 24px;
            margin: 0 0 35px;
            padding: 0 0 25px;
            border-bottom: 1px solid #dce1e6;
        }

        .compact-manager-heading h1 {
            margin: 8px 0;
            color: #111d2b;
            font-size: 36px;
            font-weight: 900;
            letter-spacing: -1px;
        }

        .compact-manager-heading h1 span {
            color: #e49400;
        }

        .compact-manager-heading p {
            margin: 0;
            color: #687485;
            font-size: 14px;
        }

        .manager-campus-card {
            min-width: 210px;
            padding: 18px 22px;
            border-left: 5px solid #e49400;
            background-color: #111d2b;
            color: #ffffff;
            text-align: right;
        }

        .manager-campus-card span {
            display: block;
            margin-bottom: 5px;
            color: #e49400;
            font-size: 10px;
            font-weight: 900;
            letter-spacing: 1.5px;
        }

        .manager-campus-card strong {
            font-size: 13px;
        }

        .manager-heading-row {
            display: flex;
            align-items: flex-end;
            justify-content: space-between;
            gap: 18px;
            margin-bottom: 23px;
        }

        .manager-heading-row h2 {
            margin: 7px 0 0;
        }

        .manager-heading-row > p {
            margin: 0;
            color: #718092;
            font-size: 13px;
        }

        /* Four cards per row, sized like the member dashboard */
        .manager-expanded-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 18px;
            width: 100%;
        }

        .manager-stat-card {
            min-width: 0;
            min-height: 195px;
            padding: 24px;
            box-sizing: border-box;
        }

       
.manager-expanded-grid {
    display: grid;
    grid-template-columns: repeat(4, minmax(0, 1fr));
    gap: 18px;
    width: 100%;
    align-items: stretch;
}

.manager-stat-card {
    min-width: 0;
    height: 200px;
    min-height: 200px;
    padding: 24px;
    box-sizing: border-box;
    overflow: hidden;
}

.manager-stat-number {
    display: block;
    margin: 0 0 12px;
    font-size: 35px;
    line-height: 1.1;
    overflow-wrap: anywhere;
}

.manager-stat-title {
    display: block;
    margin-bottom: 12px;
    font-size: 10px;
    letter-spacing: 1.3px;
}

.manager-stat-card p {
    min-height: 38px;
    margin: 0;
    font-size: 12px;
    line-height: 1.5;
}

.manager-stat-link {
    display: inline-block;
    margin-top: 13px;
    font-size: 9px;
}

.manager-revenue-value {
    font-size: 28px !important;
}


        .manager-revenue-card {
            background-color: #e49400;
        }

        .manager-revenue-card,
        .manager-revenue-card p,
        .manager-revenue-card .manager-stat-link {
            color: #111d2b !important;
        }

        .manager-revenue-value {
            font-size: 28px !important;
        }

        /* Notifications */
        .manager-notifications-section {
            margin: 30px 0 45px;
        }

        .manager-notification-item {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 18px;
            margin: 0 0 14px;
            padding: 20px 22px;
            border-left: 4px solid #e49400;
            border-radius: 8px;
            background: #ffffff;
            box-shadow: 0 6px 20px rgba(15, 28, 43, 0.08);
        }

        .manager-notification-content {
            min-width: 0;
            flex: 1;
        }

        .manager-notification-content h3 {
            margin: 0 0 8px;
            color: #142235;
            font-size: 17px;
        }

        .manager-notification-content p {
            margin: 0 0 8px;
            color: #526174;
            font-size: 13px;
            line-height: 1.6;
        }

        .manager-notification-content small {
            color: #718092;
            font-size: 11px;
        }

        .manager-notification-status {
            color: #9a5a00;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
        }

        .manager-notification-item a {
            color: #9a5a00;
            font-size: 11px;
            font-weight: 800;
            white-space: nowrap;
        }

        /* Today's session table */
        .manager-today-section {
            margin: 45px 0;
        }

        .manager-today-table-wrapper {
            width: 100%;
            margin-top: 24px;
            overflow-x: auto;
            border-radius: 8px;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.13);
        }

        .manager-today-table {
            width: 100% !important;
            min-width: 850px;
            border-collapse: collapse !important;
        }

        .manager-today-table th {
            padding: 16px 14px !important;
            border: none !important;
            border-bottom: 4px solid #e49400 !important;
            background-color: #111d2b !important;
            color: #ffffff !important;
            font-size: 12px;
            text-align: left !important;
        }

        .manager-today-table td {
            padding: 16px 14px !important;
            border: none !important;
            border-bottom: 1px solid #dfe4ea !important;
            background-color: #ffffff !important;
            color: #142235 !important;
        }

        .manager-today-table tr:nth-child(even) td {
            background-color: #f4f6f9 !important;
        }

        @media screen and (max-width: 1050px) {
            .manager-expanded-grid {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }
        }

        @media screen and (max-width: 800px) {
            .manager-expanded-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .manager-notification-item {
                display: block;
            }

            .manager-notification-status {
                display: inline-block;
                margin-top: 12px;
            }

            .compact-manager-heading,
            .manager-heading-row {
                display: block;
            }

            .manager-campus-card {
                margin-top: 22px;
                text-align: left;
            }

            .manager-heading-row > p {
                margin-top: 10px;
            }
        }

        @media screen and (max-width: 600px) {
            .manager-expanded-grid {
                grid-template-columns: 1fr;
            }

            .manager-stat-card {
                min-height: 170px;
            }
        }
    </style>

</asp:Content>
