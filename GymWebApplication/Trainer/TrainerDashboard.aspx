
<%@ Page Title="Trainer Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="TrainerDashboard.aspx.cs"
    Inherits="GymWebApplication.Trainer.TrainerDashboard" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link rel="stylesheet"
          href="<%= ResolveUrl("~/Content/TrainerDashboard.css") %>" />

    <div class="trainer-dashboard simplified-trainer-dashboard">

        <!-- WELCOME BANNER -->

        <section class="trainer-dashboard-welcome compact-trainer-heading">
            <div>
                <span class="trainer-section-label">TRAINER OVERVIEW</span>
                <h1>
                    Welcome Back,
                    <span>
                        <asp:Label ID="lblTrainerName" runat="server" Text="Trainer" />
                    </span>
                </h1>
                <p>Review your sessions, assigned members and availability.</p>
            </div>

            <div class="trainer-campus-card">
                <span>UKZN SPORT</span>
                <strong>WESTVILLE CAMPUS</strong>
            </div>
        </section>

        <!-- TRAINER NOTIFICATIONS -->

        <section class="trainer-dashboard-section trainer-notifications">

            <div class="trainer-heading dashboard-heading-row">
                <div>
                    <span class="trainer-section-label">IMPORTANT UPDATES</span>
                    <h2>Gym <span>Announcements.</span></h2>
                </div>

                <asp:Label
                    ID="lblNotificationCount"
                    runat="server"
                    CssClass="trainer-notification-count"
                    Text="0 unread" />
            </div>

            <asp:Label
                ID="lblNotificationEmpty"
                runat="server"
                CssClass="trainer-notification-empty"
                Text="You have no announcements at the moment." />

            <asp:Repeater
                ID="rptNotifications"
                runat="server"
                OnItemCommand="rptNotifications_ItemCommand">

                <ItemTemplate>
                    <article class='<%# Convert.ToBoolean(Eval("Is_Read")) ? "trainer-notification-card" : "trainer-notification-card unread" %>'>

                        <div class="trainer-notification-top">
                            <h3><%#: Eval("Title") %></h3>

                            <span class="trainer-notification-status">
                                <%# Convert.ToBoolean(Eval("Is_Read")) ? "READ" : "UNREAD" %>
                            </span>
                        </div>

                        <p><%#: Eval("Message") %></p>

                        <div class="trainer-notification-footer">
                            <span><%#: Eval("Created_At") %></span>

                            <asp:LinkButton
                                ID="btnMarkNotificationRead"
                                runat="server"
                                CssClass="trainer-notification-read"
                                CommandName="MarkRead"
                                CommandArgument='<%# Eval("Notification_ID") %>'
                                CausesValidation="false"
                                Visible='<%# !Convert.ToBoolean(Eval("Is_Read")) %>'>
                                MARK AS READ
                            </asp:LinkButton>
                        </div>

                    </article>
                </ItemTemplate>

            </asp:Repeater>

        </section>

        <!-- TRAINER STATISTICS -->

        <section class="trainer-dashboard-section trainer-overview-section">

            <div class="trainer-heading dashboard-heading-row">
                <div>
                    <span class="trainer-section-label">CURRENT ACTIVITY</span>
                    <h2>Training <span>Overview.</span></h2>
                </div>

                <p>Your latest training statistics.</p>
            </div>

            <div class="trainer-stat-grid trainer-expanded-stats">

                <div class="trainer-stat-card">
                    <span class="trainer-stat-number">
                        <asp:Label ID="lblTodaySessions" runat="server" Text="0" />
                    </span>
                    <span class="trainer-stat-label">TODAY'S SESSIONS</span>
                    <a href="<%= ResolveUrl("~/Trainer/Sessions.aspx") %>"
                       class="trainer-stat-link">VIEW SESSIONS →</a>
                </div>

                <div class="trainer-stat-card">
                    <span class="trainer-stat-number">
                        <asp:Label ID="lblUpcomingSessions" runat="server" Text="0" />
                    </span>
                    <span class="trainer-stat-label">UPCOMING SESSIONS</span>
                    <a href="<%= ResolveUrl("~/Trainer/Sessions.aspx") %>"
                       class="trainer-stat-link">VIEW SCHEDULE →</a>
                </div>

                <div class="trainer-stat-card">
                    <span class="trainer-stat-number">
                        <asp:Label ID="lblCompletedSessions" runat="server" Text="0" />
                    </span>
                    <span class="trainer-stat-label">COMPLETED SESSIONS</span>
                    <a href="<%= ResolveUrl("~/Trainer/Sessions.aspx") %>"
                       class="trainer-stat-link">VIEW HISTORY →</a>
                </div>

                <div class="trainer-stat-card">
                    <span class="trainer-stat-number">
                        <asp:Label ID="lblMemberCount" runat="server" Text="0" />
                    </span>
                    <span class="trainer-stat-label">UNIQUE MEMBERS</span>
                    <a href="<%= ResolveUrl("~/Trainer/Members.aspx") %>"
                       class="trainer-stat-link">VIEW MEMBERS →</a>
                </div>

                <div class="trainer-stat-card trainer-availability-card">
                    <span class="trainer-stat-number trainer-availability-value">
                        <asp:Label ID="lblScheduleStatus" runat="server" Text="—" />
                    </span>
                    <span class="trainer-stat-label">FUTURE AVAILABILITY</span>
                    <a href="<%= ResolveUrl("~/Trainer/Schedule.aspx") %>"
                       class="trainer-stat-link">MANAGE AVAILABILITY →</a>
                </div>

            </div>
        </section>

        <!-- NEXT SESSION -->

        <asp:Panel
            ID="NextSessionPanel"
            runat="server"
            CssClass="trainer-next-session"
            Visible="false">

            <div class="trainer-next-number">NEXT</div>

            <div>
                <span class="trainer-section-label">YOUR NEXT TRAINING SESSION</span>

                <h2>
                    <asp:Label ID="lblNextSessionTitle" runat="server" />
                </h2>

                <p>
                    <asp:Label ID="lblNextSessionDetails" runat="server" />
                </p>
            </div>

            <a href="<%= ResolveUrl("~/Trainer/Sessions.aspx") %>"
               class="trainer-next-link">MANAGE SESSION →</a>

        </asp:Panel>

        <!-- TRAINER ACCOUNT INFORMATION -->

        <section class="trainer-dashboard-account">

            <div class="trainer-heading dashboard-heading-row">
                <div>
                    <span class="trainer-section-label">ACCOUNT INFORMATION</span>
                    <h2>Trainer <span>Account.</span></h2>
                </div>
                <p>Details linked to your current login.</p>
            </div>

            <div class="trainer-account-grid">

                <div class="trainer-account-box">
                    <span>EMAIL</span>
                    <strong>
                        <asp:Label ID="lblTrainerEmail" runat="server" Text="—" />
                    </strong>
                </div>

                <div class="trainer-account-box">
                    <span>ROLE</span>
                    <strong>TRAINER</strong>
                </div>

                <div class="trainer-account-box">
                    <span>STATUS</span>
                    <strong>
                        <asp:Label ID="lblTrainerStatus" runat="server" Text="—" />
                    </strong>
                </div>

            </div>
        </section>

    </div>

    <style type="text/css">
        /* Match the Member Dashboard layout and colour palette */
        .simplified-trainer-dashboard,
        .simplified-trainer-dashboard * { box-sizing: border-box; }

        .simplified-trainer-dashboard {
            width: 100%;
            min-height: calc(100vh - 50px);
            margin: 0;
            padding: 45px 0 75px;
            background: #0d1b2a;
            color: #fff;
            font-family: Arial, Helvetica, sans-serif;
        }

        .simplified-trainer-dashboard > section,
        .simplified-trainer-dashboard > .trainer-next-session {
            width: calc(100% - 60px);
            max-width: 1250px;
            margin-left: auto;
            margin-right: auto;
        }

        .trainer-section-label {
            display: block;
            margin-bottom: 10px;
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 2px;
        }

        .compact-trainer-heading {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 30px;
            margin-bottom: 38px !important;
            padding: 0 0 28px;
            border-bottom: 1px solid rgba(255,255,255,.14);
        }

        .compact-trainer-heading h1 {
            margin: 0 0 10px;
            color: #fff;
            font-size: 39px;
            font-weight: 900;
        }

        .compact-trainer-heading h1 span { color: #e49400; }
        .compact-trainer-heading p {
            margin: 0;
            color: #9ba8b7;
            font-size: 14px;
            line-height: 1.7;
        }

        .trainer-campus-card {
            min-width: 230px;
            padding: 20px 23px;
            border-left: 5px solid #e49400;
            background: #111f2f;
            color: #fff;
            text-align: right;
        }

        .trainer-campus-card span {
            display: block;
            margin-bottom: 7px;
            color: #e49400;
            font-size: 10px;
            font-weight: 900;
            letter-spacing: 1.5px;
        }
        .trainer-campus-card strong { color: #fff; font-size: 13px; }

        .trainer-dashboard-section { margin-top: 0; margin-bottom: 24px; }
        .trainer-notifications {
            padding: 16px;
            border: 1px solid rgba(255,255,255,.10);
            border-radius: 8px;
            background: #142235;
        }

        .trainer-heading { margin-bottom: 24px; }
        .dashboard-heading-row {
            display: flex;
            align-items: flex-end;
            justify-content: space-between;
            gap: 25px;
            margin-bottom: 14px;
        }
        .dashboard-heading-row h2,
        .trainer-heading h2 {
            margin: 5px 0 0;
            color: #fff;
            font-size: 30px;
            font-weight: 900;
        }
        .dashboard-heading-row h2 span,
        .trainer-heading h2 span { color: #e49400; }
        .dashboard-heading-row > p {
            margin: 0;
            color: #8795a6;
            font-size: 13px;
        }

        .trainer-notification-count {
            display: inline-block;
            padding: 5px 9px;
            border-radius: 20px;
            background: rgba(228,148,0,.15);
            color: #f2bd54;
            font-size: 11px;
            font-weight: 700;
            white-space: nowrap;
        }
        .trainer-notification-empty {
            display: block;
            padding: 12px;
            border: 1px dashed #35465b;
            border-radius: 6px;
            background: #0f1c2b;
            color: #9ba8b7;
            font-size: 12px;
            line-height: 1.5;
        }
        .trainer-notification-card {
            margin-top: 9px;
            padding: 12px 15px;
            border: 1px solid #304056;
            border-radius: 7px;
            background: #0f1c2b;
        }
        .trainer-notification-card.unread { border-left: 3px solid #e49400; }
        .trainer-notification-top {
            display: flex;
            align-items: baseline;
            flex-wrap: wrap;
            gap: 4px 12px;
        }
        .trainer-notification-top h3 {
            margin: 0;
            color: #fff;
            font-size: 14px;
            font-weight: 800;
        }
        .trainer-notification-status {
            color: #8795a6;
            font-size: 10px;
            font-weight: 800;
            letter-spacing: .7px;
        }
        .trainer-notification-card p {
            margin: 8px 0;
            color: #c4ccd5;
            font-size: 12px;
            line-height: 1.5;
            overflow-wrap: anywhere;
        }
        .trainer-notification-footer {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            color: #8795a6;
            font-size: 10px;
        }
        .trainer-notification-read {
            display: inline-block;
            margin-top: 4px;
            padding: 6px 9px;
            border: 1px solid #e49400;
            border-radius: 4px;
            background: transparent;
            color: #f2bd54;
            font-size: 9px;
            font-weight: 900;
            text-decoration: none;
        }
        .trainer-notification-read:hover { background: #e49400; color: #111d2b; text-decoration: none; }

        .trainer-overview-section { margin-top: 45px; }
        .trainer-stat-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 18px;
            width: 100%;
            margin: 0;
        }
        .trainer-stat-card {
            min-width: 0;
            min-height: 200px;
            padding: 24px;
            border: 1px solid rgba(255,255,255,.10);
            border-bottom: 5px solid #e49400;
            border-radius: 8px;
            background: #142235;
            color: #fff;
            box-shadow: 0 8px 22px rgba(0,0,0,.18);
        }
        .trainer-stat-number {
            display: block;
            margin: 0 0 12px;
            color: #fff;
            font-size: 35px;
            font-weight: 900;
            line-height: 1.1;
            overflow-wrap: anywhere;
        }
        .trainer-stat-label {
            display: block;
            margin-bottom: 12px;
            color: #e49400;
            font-size: 10px;
            font-weight: 900;
            letter-spacing: 1.3px;
        }
        .trainer-stat-link {
            display: inline-block;
            margin-top: 13px;
            color: #e49400;
            font-size: 9px;
            font-weight: 900;
            text-decoration: none;
        }
        .trainer-stat-link:hover { color: #fff; }
        .trainer-availability-value { color: #e49400; font-size: 22px; }
        .trainer-availability-card { background: #14283a; }

        .trainer-next-session {
            display: grid;
            grid-template-columns: auto 1fr auto;
            gap: 20px;
            align-items: center;
            margin-top: 30px;
            margin-bottom: 30px;
            padding: 23px;
            border: 1px solid #e6bb64;
            border-left: 6px solid #e49400;
            border-radius: 7px;
            background: #fff8e8;
            color: #142235;
            box-shadow: 0 7px 20px rgba(0,0,0,.14);
        }
        .trainer-next-number {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 48px;
            height: 48px;
            border-radius: 50%;
            background: #e49400;
            color: #111d2b;
            font-size: 12px;
            font-weight: 900;
        }
        .trainer-next-session h2 { margin: 5px 0; color: #142235; font-size: 18px; }
        .trainer-next-session p { margin: 0; color: #596778; font-size: 13px; line-height: 1.6; }
        .trainer-next-link { color: #9a5a00; font-size: 10px; font-weight: 900; text-decoration: none; white-space: nowrap; }

        .trainer-dashboard-account { margin-top: 45px !important; }
        .trainer-account-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0,1fr));
            gap: 18px;
        }
        .trainer-account-box {
            min-width: 0;
            padding: 24px;
            border: 1px solid rgba(255,255,255,.10);
            border-radius: 8px;
            background: rgba(255,255,255,.05);
        }
        .trainer-account-box span {
            display: block;
            margin-bottom: 10px;
            color: #8795a6;
            font-size: 10px;
            font-weight: 900;
            letter-spacing: 1.2px;
        }
        .trainer-account-box strong { color: #fff; font-size: 14px; overflow-wrap: anywhere; }
        .trainer-active { color: #e49400 !important; }

        @media screen and (max-width: 1000px) {
            .trainer-stat-grid { grid-template-columns: repeat(2, minmax(0,1fr)); }
        }
        @media screen and (max-width: 700px) {
            .compact-trainer-heading,
            .dashboard-heading-row { display: block; }
            .trainer-campus-card { margin-top: 22px; text-align: left; }
            .dashboard-heading-row > p { margin-top: 10px; }
            .trainer-next-session { grid-template-columns: auto 1fr; }
            .trainer-next-link { grid-column: 2; }
            .trainer-account-grid { grid-template-columns: 1fr; }
            .trainer-notification-top,
            .trainer-notification-footer { align-items: flex-start; flex-direction: column; }
        }
        @media screen and (max-width: 550px) {
            .simplified-trainer-dashboard > section,
            .simplified-trainer-dashboard > .trainer-next-session { width: calc(100% - 30px); }
            .trainer-stat-grid { grid-template-columns: 1fr; }
            .compact-trainer-heading h1 { font-size: 32px; }
            .trainer-notifications { padding: 13px; }
            .trainer-notification-count { margin-top: 12px; }
        }
    </style>

</asp:Content>