<%@ Page Title="Manager Reports"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Reports.aspx.cs"
    Inherits="GymWebApplication.Manager.Reports" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link rel="stylesheet"
          href="<%= ResolveUrl("~/Content/ManagerReports.css") %>" />

    <div class="manager-reports-page">
        <asp:Label
            ID="lblReportMessage"
            runat="server"
            CssClass="manager-report-message">
        </asp:Label>

        <section class="manager-report-stats">

            <div class="manager-report-stat-card">

                <span class="manager-report-stat-label">
                    TOTAL MEMBERS
                </span>

                <strong>

                    <asp:Label
                        ID="lblTotalMembers"
                        runat="server"
                        Text="0" />

                </strong>

                <p>
                    Registered members
                </p>

            </div>

            <div class="manager-report-stat-card">

                <span class="manager-report-stat-label">
                    ACTIVE MEMBERS
                </span>

                <strong>

                    <asp:Label
                        ID="lblActiveMembers"
                        runat="server"
                        Text="0" />

                </strong>

                <p>
                    Currently active
                </p>

            </div>

            <div class="manager-report-stat-card">

                <span class="manager-report-stat-label">
                    TOTAL TRAINERS
                </span>

                <strong>

                    <asp:Label
                        ID="lblTotalTrainers"
                        runat="server"
                        Text="0" />

                </strong>

                <p>
                    Registered trainers
                </p>

            </div>

            <div class="manager-report-stat-card">

                <span class="manager-report-stat-label">
                    TOTAL SESSIONS
                </span>

                <strong>

                    <asp:Label
                        ID="lblTotalSessions"
                        runat="server"
                        Text="0" />

                </strong>

                <p>
                    Recorded sessions
                </p>

            </div>

        </section>

        <section class="manager-report-section">

            <div class="manager-section-heading">

                <span class="manager-section-label">
                    TRAINING ACTIVITY
                </span>

                <h2>
                    Session <span>Overview.</span>
                </h2>

                <p>
                    Current status of training sessions across
                    the gym.
                </p>

            </div>

            <div class="manager-session-stat-grid">

                <div class="manager-session-stat">

                    <div class="manager-session-stat-number">

                        <asp:Label
                            ID="lblPendingSessions"
                            runat="server"
                            Text="0" />

                    </div>

                    <div>

                        <span>
                            PENDING
                        </span>

                        <p>
                            Sessions awaiting completion
                        </p>

                    </div>

                </div>

                <div class="manager-session-stat">

                    <div class="manager-session-stat-number">

                        <asp:Label
                            ID="lblCompletedSessions"
                            runat="server"
                            Text="0" />

                    </div>

                    <div>

                        <span>
                            COMPLETED
                        </span>

                        <p>
                            Successfully completed sessions
                        </p>

                    </div>

                </div>

                <div class="manager-session-stat">

                    <div class="manager-session-stat-number">

                        <asp:Label
                            ID="lblCancelledSessions"
                            runat="server"
                            Text="0" />

                    </div>

                    <div>

                        <span>
                            CANCELLED
                        </span>

                        <p>
                            Cancelled training sessions
                        </p>

                    </div>

                </div>

            </div>

        </section>

        <section class="manager-report-section">

            <div class="manager-section-heading">

                <span class="manager-section-label">
                    MEMBERSHIP
                </span>

                <h2>
                    Member <span>Overview.</span>
                </h2>

                <p>
                    Current membership status across the system.
                </p>

            </div>

            <div class="manager-membership-card">

                <div class="manager-membership-item">

                    <span>
                        ACTIVE MEMBERS
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblMembershipActive"
                            runat="server"
                            Text="0" />

                    </strong>

                </div>

                <div class="manager-membership-divider"></div>

                <div class="manager-membership-item">

                    <span>
                        INACTIVE MEMBERS
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblInactiveMembers"
                            runat="server"
                            Text="0" />

                    </strong>

                </div>

                <div class="manager-membership-divider"></div>

                <div class="manager-membership-item">

                    <span>
                        TOTAL MEMBERS
                    </span>

                    <strong>

                        <asp:Label
                            ID="lblMembershipTotal"
                            runat="server"
                            Text="0" />

                    </strong>

                </div>

            </div>

        </section>

        <section class="manager-report-section">

            <div class="manager-section-heading">

                <span class="manager-section-label">
                    TRAINER ACTIVITY
                </span>

                <h2>
                    Trainer <span>Performance.</span>
                </h2>

                <p>
                    Number of training sessions handled by each
                    trainer.
                </p>

            </div>

            <div class="manager-table-wrapper">

                <asp:GridView
                    ID="gvTrainerReport"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="manager-report-table"
                    GridLines="None"
                    EmptyDataText="No trainer activity is currently available.">

                    <Columns>

                        <asp:BoundField
                            DataField="Trainer_Name"
                            HeaderText="TRAINER" />

                        <asp:BoundField
                            DataField="MemberCount"
                            HeaderText="MEMBERS" />

                        <asp:BoundField
                            DataField="TotalSessions"
                            HeaderText="SESSIONS" />

                        <asp:BoundField
                            DataField="CompletedSessions"
                            HeaderText="COMPLETED" />

                        <asp:BoundField
                            DataField="PendingSessions"
                            HeaderText="PENDING" />

                        <asp:BoundField
                            DataField="CancelledSessions"
                            HeaderText="CANCELLED" />

                    </Columns>

                </asp:GridView>

            </div>

        </section>

    </div>

</asp:Content>