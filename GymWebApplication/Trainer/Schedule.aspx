<%@ Page Title="Trainer Availability"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Schedule.aspx.cs"
    Inherits="GymWebApplication.Trainer.Schedule" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Schedule.css") %>" />

    <div class="trainer-page">

       
        <section class="availability-section">

            <div class="availability-heading">

                <span class="trainer-section-label">
                    MANAGE AVAILABILITY
                </span>

                <h2>
                    Set Your <span>Availability.</span>
                </h2>

                <p>
                    Slots containing pending member bookings are
                    protected and cannot be made unavailable.
                </p>

            </div>

            <asp:UpdatePanel
                ID="ScheduleUpdatePanel"
                runat="server"
                UpdateMode="Conditional">

                <ContentTemplate>

                    <div class="availability-date-card">

                        <div class="availability-date-content">

                            <div class="availability-date-information">

                                <span class="availability-label">
                                    SELECT DATE
                                </span>

                                <h3>
                                    Choose a training day
                                </h3>

                                <p>
                                    Past dates cannot be changed.
                                </p>

                            </div>

                            <div class="availability-date-control">

                                <asp:TextBox
                                    ID="txtDate"
                                    runat="server"
                                    TextMode="Date"
                                    CssClass="trainer-input">
                                </asp:TextBox>

                                <asp:Button
                                    ID="btnLoadDate"
                                    runat="server"
                                    Text="VIEW TIME SLOTS →"
                                    CssClass="trainer-action-button"
                                    CausesValidation="false"
                                    OnClick="btnLoadDate_Click" />

                            </div>

                        </div>

                    </div>

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        CssClass="trainer-message">
                    </asp:Label>

                    <asp:Panel
                        ID="pnlSlots"
                        runat="server"
                        CssClass="availability-slots-panel"
                        Visible="false">

                        <div class="availability-slots-header">

                            <div>

                                <span class="availability-label">
                                    YOUR TIME SLOTS
                                </span>

                                <h3 class="selected-date-heading">

                                    <asp:Label
                                        ID="lblSelectedDate"
                                        runat="server" />

                                </h3>

                                <p class="slot-instruction">
                                    Tick each slot when you are
                                    available for training.
                                </p>

                            </div>

                        </div>

                        <section class="schedule-statistics">

                            <div class="schedule-stat-card">

                                <span>AVAILABLE</span>

                                <strong>
                                    <asp:Label
                                        ID="lblAvailableCount"
                                        runat="server"
                                        Text="0" />
                                </strong>

                            </div>

                            <div class="schedule-stat-card">

                                <span>BOOKED</span>

                                <strong>
                                    <asp:Label
                                        ID="lblBookedCount"
                                        runat="server"
                                        Text="0" />
                                </strong>

                            </div>

                            <div class="schedule-slot-filter">

                                <label>SHOW</label>

                                <asp:DropDownList
                                    ID="ddlSlotFilter"
                                    runat="server"
                                    CssClass="trainer-input"
                                    AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlSlotFilter_SelectedIndexChanged">

                                    <asp:ListItem
                                        Text="All slots"
                                        Value="All" />

                                    <asp:ListItem
                                        Text="Available"
                                        Value="Available" />

                                    <asp:ListItem
                                        Text="Not available"
                                        Value="Not Available" />

                                    <asp:ListItem
                                        Text="Booked"
                                        Value="Booked" />

                                </asp:DropDownList>

                            </div>

                        </section>

                        <div class="availability-table-wrapper">

                            <asp:GridView
                                ID="gvTimeSlots"
                                runat="server"
                                AutoGenerateColumns="False"
                                CssClass="availability-table"
                                GridLines="None"
                                DataKeyNames="SlotStart,SlotEnd">

                                <Columns>

                                    <asp:BoundField
                                        DataField="TimeSlot"
                                        HeaderText="TIME SLOT" />

                                    <asp:TemplateField
                                        HeaderText="AVAILABLE">

                                        <ItemTemplate>

                                            <asp:CheckBox
                                                ID="chkAvailable"
                                                runat="server"
                                                Checked='<%# Convert.ToBoolean(
                                                    Eval("IsAvailable")
                                                ) %>'
                                                Enabled='<%# Convert.ToBoolean(
                                                    Eval("CanEdit")
                                                ) %>'
                                                CssClass="availability-checkbox" />

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                    <asp:BoundField
                                        DataField="BookingCount"
                                        HeaderText="BOOKINGS" />

                                    <asp:TemplateField
                                        HeaderText="STATUS">

                                        <ItemTemplate>

                                            <span class='<%# Eval(
                                                "StatusCss"
                                            ) %>'>
                                                <%# Eval("Status") %>
                                            </span>

                                        </ItemTemplate>

                                    </asp:TemplateField>

                                </Columns>

                            </asp:GridView>

                        </div>

                        <div class="availability-save-area">

                            <asp:Button
                                ID="btnSaveAvailability"
                                runat="server"
                                Text="SAVE AVAILABILITY →"
                                CssClass="trainer-action-button availability-save-button"
                                OnClientClick="return confirm('Save these availability changes? Existing booked slots will remain protected.');"
                                OnClick="btnSaveAvailability_Click" />

                        </div>

                    </asp:Panel>

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

    <style type="text/css">

        .trainer-message {
            display: block;
            margin: 18px 0;
            padding: 14px 16px;
            border-radius: 7px;
            background: #eef2f6;
            color: #142235;
            font-weight: 600;
        }

        .trainer-message:empty {
            display: none;
        }

        .schedule-message-success {
            border: 1px solid #8dd4a3;
            background: #e8f7ed;
            color: #146c2e;
        }

        .schedule-message-error {
            border: 1px solid #efaaaa;
            background: #fdecec;
            color: #a51d1d;
        }

        .schedule-statistics {
            display: grid;
            grid-template-columns: 1fr 1fr 2fr;
            gap: 16px;
            align-items: end;
            margin: 22px 0;
        }

        .schedule-stat-card {
            padding: 18px;
            border-radius: 8px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .schedule-stat-card span {
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
        }

        .schedule-stat-card strong {
            display: block;
            margin-top: 5px;
            font-size: 28px;
        }

        .schedule-slot-filter label {
            display: block;
            margin-bottom: 7px;
            color: #142235;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .slot-status.booked {
            background: #e8efff;
            color: #174ea6;
        }

        .slot-status.past {
            background: #eceff2;
            color: #697582;
        }

        @media screen and (max-width: 760px) {

            .schedule-statistics {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>