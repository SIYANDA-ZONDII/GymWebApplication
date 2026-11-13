
<%@ Page Title="Manage Booking"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Activities.aspx.cs"
    Inherits="GymWebApplication.Member.Activities" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Booking.css") %>" />
    <link rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Activities.css") %>" />

    <style>
        .manage-booking {
            --mb-navy: #062d38;
            --mb-teal: #084451;
            --mb-orange: #f5a000;
            --mb-ink: #17283a;
            --mb-muted: #687789;
            --mb-border: #e0e7ee;
            max-width: 1380px;
            margin: 0 auto;
            padding: 38px 24px 65px;
            color: var(--mb-ink);
        }

        .mb-eyebrow {
            display: inline-block;
            margin-bottom: 10px;
            color: var(--mb-orange);
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 2px;
            text-transform: uppercase;
        }

        .mb-section-heading h2 span {
            color: var(--mb-orange);
        }

        .mb-section {
            margin-top: 34px;
        }

        .mb-section-heading {
            margin-bottom: 20px;
        }

        .mb-section-heading h2 {
            margin: 0 0 8px;
            color: #f7fbfd;
            font-size: 27px;
            font-weight: 800;
        }

        .mb-section-heading p {
            margin: 0;
            color: #c8dce3;
            line-height: 1.7;
        }

        .mb-booking-layout {
            display: grid;
            grid-template-columns: minmax(0, 1fr);
            gap: 24px;
            align-items: start;
        }

        .mb-card {
            min-width: 0;
            padding: 30px;
            border: 1px solid var(--mb-border);
            border-radius: 15px;
            background: #fff;
            box-shadow: 0 8px 28px rgba(20, 43, 60, .055);
        }

        .mb-card-heading {
            margin-bottom: 25px;
        }

        .mb-card-heading h3 {
            margin: 0 0 7px;
            color: var(--mb-ink);
            font-size: 20px;
            font-weight: 800;
        }

        .mb-card-heading p {
            margin: 0;
            color: var(--mb-muted);
            font-size: 13px;
            line-height: 1.7;
        }

        .mb-booking-fields {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 21px;
        }

        .mb-field {
            min-width: 0;
        }

        .mb-field label {
            display: block;
            margin-bottom: 9px;
            color: #40546a;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1.2px;
        }

        .mb-input {
            display: block;
            box-sizing: border-box;
            width: 100%;
            min-height: 49px;
            padding: 12px 14px;
            border: 1px solid #d4dee8;
            border-radius: 8px;
            background: #fff;
            color: var(--mb-ink);
            font-family: inherit;
            font-size: 14px;
            transition: border-color .2s, box-shadow .2s;
        }

        .mb-input:focus {
            border-color: #0d7485;
            outline: none;
            box-shadow: 0 0 0 3px rgba(13,116,133,.12);
        }

        .mb-field-full {
            grid-column: 1 / -1;
        }

        .mb-booking-button {
            min-height: 49px;
            margin-top: 24px;
            padding: 13px 22px;
            border: 0;
            border-radius: 8px;
            background: var(--mb-orange);
            color: #17283a;
            font-size: 12px;
            font-weight: 900;
            letter-spacing: .7px;
            cursor: pointer;
            transition: transform .2s, background .2s;
        }

        .mb-booking-button:hover {
            transform: translateY(-1px);
            background: #ffb21b;
        }

        .mb-message {
            display: block;
            margin-top: 15px;
            line-height: 1.6;
            overflow-wrap: anywhere;
        }
.mb-stats {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 18px;
        }

        .mb-stat {
            padding: 23px;
            border: 1px solid var(--mb-border);
            border-radius: 13px;
            background: #fff;
            box-shadow: 0 5px 20px rgba(20,43,60,.035);
        }

        .mb-stat-label {
            color: var(--mb-muted);
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .mb-stat-number {
            display: block;
            margin: 11px 0 7px;
            color: var(--mb-ink);
            font-size: 35px;
            font-weight: 900;
        }

        .mb-stat-description {
            color: var(--mb-muted);
            font-size: 12px;
        }

        .mb-stat-upcoming {
            border-top: 4px solid #e99a00;
        }

        .mb-stat-completed {
            border-top: 4px solid #23945b;
        }

        .mb-stat-cancelled {
            border-top: 4px solid #d34c4c;
        }

        .mb-filters {
            display: grid;
            grid-template-columns: minmax(0, 2fr) minmax(180px, 1fr) auto;
            gap: 15px;
            align-items: end;
            margin: 22px 0;
            padding: 21px;
            border: 1px solid var(--mb-border);
            border-radius: 12px;
            background: #f6f8fb;
        }

        .mb-clear-button {
            min-height: 47px;
            padding: 12px 17px;
            border: 1px solid #cbd6e0;
            border-radius: 8px;
            background: #fff;
            color: var(--mb-ink);
            font-size: 11px;
            font-weight: 900;
            cursor: pointer;
        }

        .mb-clear-button:hover {
            border-color: var(--mb-orange);
            background: #fff9ed;
        }

        .mb-table-shell {
            width: 100%;
            overflow-x: auto;
            border: 1px solid var(--mb-border);
            border-radius: 12px;
            background: #fff;
        }

        .mb-table {
            width: 100% !important;
            min-width: 1080px;
            border-collapse: collapse !important;
            color: var(--mb-ink) !important;
            font-size: 12px;
        }

        .mb-table th {
            padding: 17px 14px !important;
            border: 0 !important;
            border-bottom: 3px solid var(--mb-orange) !important;
            background: var(--mb-navy) !important;
            color: #fff !important;
            font-size: 10px;
            font-weight: 900;
            letter-spacing: .7px;
            text-align: left !important;
            white-space: nowrap;
        }

        .mb-table td {
            padding: 16px 14px !important;
            border: 0 !important;
            border-bottom: 1px solid #e9edf2 !important;
            background: #fff !important;
            color: var(--mb-ink) !important;
            vertical-align: middle;
            white-space: nowrap;
        }

        .mb-table tr:nth-child(even) td {
            background: #f8fafc !important;
        }

        .mb-table tr:hover td {
            background: #fffaf0 !important;
        }

        .mb-status {
            display: inline-block;
            padding: 6px 10px;
            border-radius: 20px;
            font-size: 10px;
            font-weight: 900;
            text-transform: uppercase;
        }

        .mb-status-upcoming {
            background: #fff1d4;
            color: #925900;
        }

        .mb-status-completed {
            background: #e3f6eb;
            color: #17683b;
        }

        .mb-status-cancelled {
            background: #fdeaea;
            color: #a32c2c;
        }

        .mb-cancel-button {
            padding: 9px 12px;
            border: 0;
            border-radius: 6px;
            background: #c73737;
            color: #fff;
            font-size: 10px;
            font-weight: 900;
            cursor: pointer;
        }

        .mb-cancel-button:hover {
            background: #a21e1e;
        }

        .mb-unavailable {
            color: #8994a1;
            font-size: 11px;
        }

        .mb-message:empty,
        .mb-results-message:empty {
            display: none;
        }

        .mb-results-message {
            display: block;
            margin: 15px 0;
            color: var(--mb-muted);
            font-size: 12px;
        }
@media (max-width: 900px) {
            .mb-booking-layout {
                grid-template-columns: 1fr;
            }

            .mb-filters {
                grid-template-columns: 1fr 1fr;
            }

            .mb-filter-action {
                grid-column: 1 / -1;
            }
        }

        @media (max-width: 600px) {
            .manage-booking {
                padding: 22px 12px 40px;
            }

        </style>

    <div class="manage-booking">

<!-- BOOK A SESSION -->
        <section class="mb-section" id="book-session">

            <div class="mb-section-heading">
                <span class="mb-eyebrow">RESERVE YOUR TIME</span>
                <h2>Book a <span>Session.</span></h2>
                <p>
                    Choose a training type, trainer and date to find
                    an available time slot.
                </p>
            </div>

            <div class="mb-booking-layout">

                <div class="mb-card">

                    <div class="mb-card-heading">
                        <h3>Training details</h3>
                        <p>
                            You must have an active membership to book.
                            Past, full and conflicting sessions are unavailable.
                        </p>
                    </div>

                    <asp:UpdatePanel ID="BookingUpdatePanel"
                        runat="server"
                        UpdateMode="Conditional"
                        ChildrenAsTriggers="true">

                        <ContentTemplate>

                            <div class="mb-booking-fields">

                                <div class="mb-field mb-field-full">
                                    <label for="<%= ddlSessionType.ClientID %>">
                                        TRAINING TYPE
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlSessionType"
                                        runat="server"
                                        CssClass="mb-input"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlSessionType_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-field mb-field-full">
                                    <label for="<%= ddlTrainer.ClientID %>">
                                        TRAINER
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlTrainer"
                                        runat="server"
                                        CssClass="mb-input"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="ddlTrainer_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-field">
                                    <label for="<%= txtDate.ClientID %>">
                                        DATE
                                    </label>

                                    <asp:TextBox
                                        ID="txtDate"
                                        runat="server"
                                        TextMode="Date"
                                        CssClass="mb-input"
                                        AutoPostBack="true"
                                        OnTextChanged="txtDate_TextChanged">
                                    </asp:TextBox>
                                </div>

                                <div class="mb-field">
                                    <label for="<%= ddlTime.ClientID %>">
                                        AVAILABLE TIME
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlTime"
                                        runat="server"
                                        CssClass="mb-input">
                                    </asp:DropDownList>
                                </div>

                            </div>

                            <asp:Label
                                ID="lblMessage"
                                runat="server"
                                CssClass="mb-message">
                            </asp:Label>

                            <asp:Button
                                ID="btnBook"
                                runat="server"
                                Text="CONFIRM BOOKING  →"
                                CssClass="mb-booking-button"
                                OnClientClick="return confirm('Confirm this training-session booking?');"
                                OnClick="btnBook_Click" />

                        </ContentTemplate>

                    </asp:UpdatePanel>

                </div>
</div>
        </section>

        <!-- BOOKING STATISTICS -->
        <section class="mb-section">

            <div class="mb-section-heading">
                <span class="mb-eyebrow">YOUR ACTIVITY</span>
                <h2>Booking <span>Overview.</span></h2>
                <p>A quick look at your training-session history.</p>
            </div>

            <div class="mb-stats">

                <div class="mb-stat mb-stat-upcoming">
                    <span class="mb-stat-label">UPCOMING</span>
                    <asp:Label ID="lblUpcomingCount"
                        runat="server"
                        CssClass="mb-stat-number"
                        Text="0" />
                    <span class="mb-stat-description">
                        Pending future sessions
                    </span>
                </div>

                <div class="mb-stat mb-stat-completed">
                    <span class="mb-stat-label">COMPLETED</span>
                    <asp:Label ID="lblCompletedCount"
                        runat="server"
                        CssClass="mb-stat-number"
                        Text="0" />
                    <span class="mb-stat-description">
                        Finished sessions
                    </span>
                </div>

                <div class="mb-stat mb-stat-cancelled">
                    <span class="mb-stat-label">CANCELLED</span>
                    <asp:Label ID="lblCancelledCount"
                        runat="server"
                        CssClass="mb-stat-number"
                        Text="0" />
                    <span class="mb-stat-description">
                        Cancelled bookings
                    </span>
                </div>

            </div>
        </section>

        <!-- EXISTING BOOKINGS -->
        <section class="mb-section" id="my-bookings">

            <div class="mb-section-heading">
                <span class="mb-eyebrow">BOOKING HISTORY</span>
                <h2>My <span>Sessions.</span></h2>
                <p>
                    Search your sessions, filter by status and cancel
                    an eligible pending booking before it starts.
                </p>
            </div>

            <asp:UpdatePanel ID="ActivitiesUpdatePanel"
                runat="server"
                UpdateMode="Conditional"
                ChildrenAsTriggers="true">

                <ContentTemplate>

                    <!-- FILTERS -->
                    <div class="mb-filters">

                        <div class="mb-field">
                            <label for="<%= txtSearch.ClientID %>">
                                SEARCH SESSIONS
                            </label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="mb-input"
                                placeholder="Search by session or trainer"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>
                        </div>

                        <div class="mb-field">
                            <label for="<%= ddlStatus.ClientID %>">
                                BOOKING STATUS
                            </label>

                            <asp:DropDownList
                                ID="ddlStatus"
                                runat="server"
                                CssClass="mb-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">

                                <asp:ListItem Text="All sessions" Value="All" />
                                <asp:ListItem Text="Upcoming" Value="Pending" />
                                <asp:ListItem Text="Completed" Value="Completed" />
                                <asp:ListItem Text="Cancelled" Value="Cancelled" />

                            </asp:DropDownList>
                        </div>

                        <div class="mb-filter-action">
                            <asp:Button
                                ID="btnClearFilters"
                                runat="server"
                                Text="CLEAR FILTERS"
                                CssClass="mb-clear-button"
                                CausesValidation="false"
                                OnClick="btnClearFilters_Click" />
                        </div>

                    </div>

                    <asp:Label
                        ID="ActivitiesMessage"
                        runat="server"
                        CssClass="mb-message">
                    </asp:Label>

                    <!-- SESSION TABLE -->
                    <div class="mb-table-shell">

                        <asp:GridView
                            ID="ActivitiesGrid"
                            runat="server"
                            AutoGenerateColumns="False"
                            GridLines="None"
                            CssClass="mb-table"
                            EmptyDataText="No training sessions match your search."
                            AllowPaging="false"
                            OnRowCommand="ActivitiesGrid_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="Session_Type"
                                    HeaderText="SESSION" />

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
                                    DataField="Trainer_Name"
                                    HeaderText="TRAINER" />

                                <asp:BoundField
                                    DataField="Member_Attendance"
                                    HeaderText="ATTENDANCE" />

                                <asp:TemplateField HeaderText="STATUS">
                                    <ItemTemplate>
                                        <span class='<%# GetStatusClass(Eval("Session_Status")) %>'>
                                            <%# Eval("Session_Status") %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ACTION">
                                    <ItemTemplate>

                                        <asp:Button
                                            ID="btnCancelBooking"
                                            runat="server"
                                            Text="CANCEL"
                                            CssClass="mb-cancel-button"
                                            CommandName="CancelBooking"
                                            CommandArgument='<%# Eval("Training_ID") %>'
                                            Visible='<%# CanCancelBooking(
                                                Eval("Session_Date"),
                                                Eval("Start_Time"),
                                                Eval("Session_Status")) %>'
                                            OnClientClick="return confirm('Are you sure you want to cancel this booking?');" />

                                        <asp:Label
                                            ID="lblCancellationUnavailable"
                                            runat="server"
                                            Text="Not available"
                                            CssClass="mb-unavailable"
                                            Visible='<%# !CanCancelBooking(
                                                Eval("Session_Date"),
                                                Eval("Start_Time"),
                                                Eval("Session_Status")) %>' />

                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                    <asp:Label
                        ID="lblActivityNotice"
                        runat="server"
                        CssClass="mb-results-message">
                    </asp:Label>
</ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

</asp:Content>
