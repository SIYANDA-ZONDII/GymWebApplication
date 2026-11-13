<%@ Page Title="Member Dashboard"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="MemberDashboard.aspx.cs"
    Inherits="GymWebApplication.Member.MemberDashboard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<style type="text/css">
    .md2-page, .md2-page * { box-sizing: border-box; }

    .md2-page {
        width: 100%;
        min-height: calc(100vh - 50px);
        padding: 45px 0 75px;
        background: #0d1b2a;
        color: #fff;
        font-family: Arial, Helvetica, sans-serif;
    }

    .md2-container {
        width: calc(100% - 60px);
        max-width: 1250px;
        margin: 0 auto;
    }

    .md2-label {
        display: block;
        margin-bottom: 10px;
        color: #e49400;
        font-size: 11px;
        font-weight: 900;
        letter-spacing: 2px;
    }

    .md2-welcome {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 30px;
        margin-bottom: 38px;
        padding-bottom: 28px;
        border-bottom: 1px solid rgba(255,255,255,.14);
    }

    .md2-welcome h1 {
        margin: 0 0 10px;
        color: #fff;
        font-size: 39px;
        font-weight: 900;
    }

    .md2-welcome h1 span,
    .md2-section-heading h2 span,
    .md2-notifications-heading h2 span {
        color: #e49400;
    }

    .md2-welcome p {
        margin: 0;
        color: #9ba8b7;
        font-size: 14px;
        line-height: 1.7;
    }

    .md2-campus {
        min-width: 230px;
        padding: 20px 23px;
        border-left: 5px solid #e49400;
        background: #111f2f;
        text-align: right;
    }

    .md2-campus span {
        display: block;
        margin-bottom: 7px;
        color: #e49400;
        font-size: 10px;
        font-weight: 900;
        letter-spacing: 1.5px;
    }

    .md2-campus strong {
        color: #fff;
        font-size: 13px;
    }

    /* SESSION REMINDER */

    .md2-reminder {
        display: grid;
        grid-template-columns: auto 1fr auto;
        gap: 20px;
        align-items: center;
        margin-bottom: 30px;
        padding: 23px;
        border: 1px solid #e6bb64;
        border-left: 6px solid #e49400;
        border-radius: 7px;
        background: #fff8e8;
        color: #142235;
    }

    .md2-reminder-icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 48px;
        height: 48px;
        border-radius: 50%;
        background: #e49400;
        color: #111d2b;
        font-size: 22px;
        font-weight: 900;
    }

    .md2-reminder-label {
        color: #9a5a00;
        font-size: 10px;
        font-weight: 900;
        letter-spacing: 1px;
    }

    .md2-reminder h3 {
        margin: 5px 0;
        color: #142235;
        font-size: 18px;
    }

    .md2-reminder p {
        margin: 0 0 7px;
        color: #596778;
        font-size: 13px;
    }

    .md2-reminder-link {
        color: #9a5a00;
        font-size: 10px;
        font-weight: 900;
        text-decoration: none;
    }

    .md2-reminder-dismiss {
        padding: 10px 14px;
        border: 0;
        border-radius: 4px;
        background: #142235;
        color: #fff;
        font-size: 10px;
        font-weight: 900;
        cursor: pointer;
    }

    /* NOTIFICATIONS */

    .md2-notifications {
        margin-bottom: 24px;
        padding: 16px;
        border: 1px solid rgba(255,255,255,.10);
        border-radius: 8px;
        background: #142235;
    }

    .md2-notifications-heading {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 15px;
        margin-bottom: 14px;
    }

    .md2-notifications-heading h2 {
        margin: 5px 0 0;
        color: #fff;
        font-size: 22px;
        font-weight: 900;
    }

    .md2-notification-count {
        display: inline-block;
        padding: 5px 9px;
        border-radius: 20px;
        background: rgba(228,148,0,.15);
        color: #f2bd54;
        font-size: 11px;
        font-weight: bold;
        white-space: nowrap;
    }

    .md2-notification-empty {
        display: block;
        padding: 12px;
        border: 1px dashed #35465b;
        border-radius: 6px;
        color: #9ba8b7;
        font-size: 12px;
        line-height: 1.5;
    }

    .md2-notification-item {
        margin-bottom: 9px;
        padding: 12px 15px;
        border: 1px solid #304056;
        border-radius: 7px;
        background: #0f1c2b;
    }

    .md2-notification-item:last-child {
        margin-bottom: 0;
    }

    .md2-notification-item.unread {
        border-left: 3px solid #e49400;
    }

    .md2-notification-content {
        display: flex;
        align-items: baseline;
        flex-wrap: wrap;
        column-gap: 12px;
        row-gap: 4px;
        margin-bottom: 8px;
    }

    .md2-notification-item h3 {
        flex: 0 0 auto;
        margin: 0;
        color: #fff;
        font-size: 14px;
        font-weight: 800;
    }

    .md2-notification-message {
        flex: 1 1 250px;
        min-width: 0;
        margin: 0;
        color: #c4ccd5;
        font-size: 12px;
        line-height: 1.5;
        white-space: pre-wrap;
        overflow-wrap: anywhere;
    }

    .md2-notification-meta {
        margin-top: 4px;
        color: #8795a6;
        font-size: 10px;
    }

    .md2-notification-read {
        display: inline-block;
        margin-top: 8px;
        padding: 6px 9px;
        border: 1px solid #e49400;
        border-radius: 4px;
        background: transparent;
        color: #f2bd54;
        font-size: 9px;
        font-weight: 900;
        text-decoration: none;
    }

    .md2-notification-read:hover {
        background: #e49400;
        color: #111d2b;
        text-decoration: none;
    }

    /* SECTION HEADINGS */

    .md2-section {
        margin: 45px 0;
    }

    .md2-section-heading {
        display: flex;
        align-items: flex-end;
        justify-content: space-between;
        gap: 25px;
        margin-bottom: 24px;
    }

    .md2-section-heading h2 {
        margin: 5px 0 0;
        color: #fff;
        font-size: 30px;
        font-weight: 900;
    }

    .md2-section-heading > p {
        margin: 0;
        color: #8795a6;
        font-size: 13px;
    }

    /* STATISTICS */

    .md2-stat-grid {
        display: grid;
        grid-template-columns: repeat(4, minmax(0,1fr));
        gap: 18px;
        width: 100%;
    }

    .md2-stat-card {
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

    .md2-stat-card > span {
        display: block;
        margin-bottom: 12px;
        color: #e49400;
        font-size: 10px;
        font-weight: 900;
        letter-spacing: 1.3px;
    }

    .md2-stat-card strong {
        display: block;
        margin: 0 0 12px;
        color: #fff;
        font-size: 35px;
        font-weight: 900;
        line-height: 1.1;
    }

    .md2-stat-card strong.md2-membership-status {
        color: #e49400;
        font-size: 22px;
    }

    .md2-stat-card p {
        min-height: 38px;
        margin: 0;
        color: #c4ccd5;
        font-size: 12px;
        line-height: 1.5;
    }

    .md2-stat-card a {
        display: inline-block;
        margin-top: 13px;
        color: #e49400;
        font-size: 9px;
        font-weight: 900;
        text-decoration: none;
    }

    /* ACCOUNT DETAILS */

    .md2-account-grid {
        display: grid;
        grid-template-columns: repeat(3, minmax(0,1fr));
        gap: 18px;
    }

    .md2-account-box {
        min-width: 0;
        padding: 24px;
        border: 1px solid rgba(255,255,255,.10);
        border-radius: 8px;
        background: rgba(255,255,255,.05);
    }

    .md2-account-box span {
        display: block;
        margin-bottom: 10px;
        color: #8795a6;
        font-size: 10px;
        font-weight: 900;
        letter-spacing: 1.2px;
    }

    .md2-account-box strong {
        color: #fff;
        font-size: 14px;
        overflow-wrap: anywhere;
    }

    /* RESPONSIVE LAYOUT */

    @media screen and (max-width: 1000px) {
        .md2-stat-grid {
            grid-template-columns: repeat(2, minmax(0,1fr));
        }
    }

    @media screen and (max-width: 700px) {
        .md2-welcome,
        .md2-section-heading {
            display: block;
        }

        .md2-campus {
            margin-top: 22px;
            text-align: left;
        }

        .md2-section-heading > p {
            margin-top: 10px;
        }

        .md2-reminder {
            grid-template-columns: auto 1fr;
        }

        .md2-reminder-dismiss {
            grid-column: 2;
            justify-self: start;
        }

        .md2-account-grid {
            grid-template-columns: 1fr;
        }
    }

    @media screen and (max-width: 550px) {
        .md2-container {
            width: calc(100% - 30px);
        }

        .md2-stat-grid {
            grid-template-columns: 1fr;
        }

        .md2-welcome h1 {
            font-size: 32px;
        }

        .md2-notifications-heading {
            align-items: flex-start;
            flex-direction: column;
        }
    }
</style>

<div class="md2-page">
    <div class="md2-container">

        <!-- WELCOME BANNER PRESERVED -->
        <section class="md2-welcome">
            <div>
                <span class="md2-label">MEMBER OVERVIEW</span>
                <h1>
                    Welcome Back,
                    <span>
                        <asp:Label ID="lblWelcomeName" runat="server" Text="Member" />
                    </span>
                </h1>
                <p>Here is the latest overview of your membership and training activity.</p>
            </div>

            <div class="md2-campus">
                <span>UKZN SPORT</span>
                <strong>WESTVILLE CAMPUS</strong>
            </div>
        </section>

        <!-- SESSION REMINDER -->
        <asp:UpdatePanel ID="ReminderUpdatePanel" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="SessionReminderPanel" runat="server"
                    CssClass="md2-reminder" Visible="false">

                    <div class="md2-reminder-icon">!</div>

                    <div>
                        <span class="md2-reminder-label">UPCOMING SESSION REMINDER</span>
                        <h3>
                            <asp:Label ID="lblReminderTitle" runat="server" />
                        </h3>
                        <p>
                            <asp:Label ID="lblReminderMessage" runat="server" />
                        </p>
                        <a href="<%= ResolveUrl("~/Member/Activities.aspx") %>"
                           class="md2-reminder-link">VIEW MY SESSION →</a>
                    </div>

                    <asp:Button ID="btnDismissReminder" runat="server"
                        Text="DISMISS"
                        CssClass="md2-reminder-dismiss"
                        CausesValidation="false"
                        OnClick="btnDismissReminder_Click" />

                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- NOTIFICATIONS -->
        <section class="md2-notifications">
            <div class="md2-notifications-heading">
                <div>
                    <span class="md2-label">IMPORTANT UPDATES</span>
                    <h2>Gym <span>Announcements.</span></h2>
                </div>

                <asp:Label ID="lblNotificationCount" runat="server"
                    CssClass="md2-notification-count" Text="0 unread" />
            </div>

            <asp:Label ID="lblNotificationEmpty" runat="server"
                CssClass="md2-notification-empty"
                Text="You have no announcements at the moment." />

            <asp:Repeater ID="rptNotifications" runat="server"
                OnItemCommand="rptNotifications_ItemCommand">
                <ItemTemplate>
                    <article class='<%# Convert.ToBoolean(Eval("Is_Read")) ? "md2-notification-item" : "md2-notification-item unread" %>'>

                        <div class="md2-notification-content">
                            <h3><%#: Eval("Title") %></h3>
                            <p class="md2-notification-message"><%#: Eval("Message") %></p>
                        </div>

                        <div class="md2-notification-meta">
                            <%# Eval("Created_At", "{0:dd MMM yyyy, HH:mm}") %>
                            &nbsp;|&nbsp;
                            <%# Convert.ToBoolean(Eval("Is_Read")) ? "Read" : "Unread" %>
                        </div>

                        <asp:LinkButton ID="btnMarkNotificationRead" runat="server"
                            CssClass="md2-notification-read"
                            CommandName="MarkRead"
                            CommandArgument='<%# Eval("Notification_ID") %>'
                            CausesValidation="false"
                            Visible='<%# !Convert.ToBoolean(Eval("Is_Read")) %>'>
                            MARK AS READ
                        </asp:LinkButton>

                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </section>

        <!-- STATISTICS -->
        <section class="md2-section">
            <div class="md2-section-heading">
                <div>
                    <span class="md2-label">CURRENT STATUS</span>
                    <h2>Your <span>Overview.</span></h2>
                </div>
                <p>A summary of your sessions and membership.</p>
            </div>

            <div class="md2-stat-grid">
                <div class="md2-stat-card">
                    <span>UPCOMING</span>
                    <strong>
                        <asp:Label ID="lblUpcomingSessions" runat="server" Text="0" />
                    </strong>
                    <p>Pending future sessions</p>
                    <a href="<%= ResolveUrl("~/Member/Activities.aspx") %>">VIEW SESSIONS →</a>
                </div>

                <div class="md2-stat-card">
                    <span>COMPLETED</span>
                    <strong>
                        <asp:Label ID="lblCompletedSessions" runat="server" Text="0" />
                    </strong>
                    <p>Finished training sessions</p>
                    <a href="<%= ResolveUrl("~/Member/Activities.aspx") %>">VIEW HISTORY →</a>
                </div>

                <div class="md2-stat-card">
                    <span>CANCELLED</span>
                    <strong>
                        <asp:Label ID="lblCancelledSessions" runat="server" Text="0" />
                    </strong>
                    <p>Cancelled bookings</p>
                    <a href="<%= ResolveUrl("~/Member/Activities.aspx") %>">VIEW HISTORY →</a>
                </div>

                <div class="md2-stat-card">
                    <span>MEMBERSHIP</span>
                    <strong class="md2-membership-status">
                        <asp:Label ID="lblMembershipStatus" runat="server" Text="INACTIVE" />
                    </strong>
                    <p>
                        <asp:Label ID="lblMembershipExpiry" runat="server" Text="No active payment" />
                    </p>
                    <a href="<%= ResolveUrl("~/Member/Membership.aspx") %>">VIEW MEMBERSHIP →</a>
                </div>
            </div>
        </section>

        <!-- QUICK ACTIONS CARDS REMOVED -->

        <!-- ACCOUNT DETAILS -->
        <section class="md2-section">
            <div class="md2-section-heading">
                <div>
                    <span class="md2-label">ACCOUNT DETAILS</span>
                    <h2>Your <span>Account.</span></h2>
                </div>
            </div>

            <div class="md2-account-grid">
                <div class="md2-account-box">
                    <span>EMAIL ADDRESS</span>
                    <strong>
                        <asp:Label ID="lblAccountEmail" runat="server" Text="Not available" />
                    </strong>
                </div>

                <div class="md2-account-box">
                    <span>ACCOUNT STATUS</span>
                    <strong>
                        <asp:Label ID="lblAccountStatus" runat="server" Text="Active" />
                    </strong>
                </div>
            </div>
        </section>

    </div>
</div>

</asp:Content>