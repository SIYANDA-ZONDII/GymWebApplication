<%@ Page Title="Book Training"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Booking.aspx.cs"
    Inherits="GymWebApplication.Member.Booking" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Booking.css") %>" />

    <div class="booking-page">

       

        <section class="booking-section">

            <div class="booking-heading">

                <span class="booking-label">
                    TRAINING BOOKING
                </span>

                <h2>
                    Find Your <span>Session.</span>
                </h2>

                <p>
                    You must have an active membership to book.
                    Past, full and conflicting sessions are unavailable.
                </p>

            </div>

            <asp:UpdatePanel
                ID="BookingUpdatePanel"
                runat="server"
                UpdateMode="Conditional"
                ChildrenAsTriggers="true">

                <ContentTemplate>

                    <div class="booking-card">

                        <div class="booking-field">

                            <label>
                                TRAINING TYPE
                            </label>

                            <asp:DropDownList
                                ID="ddlSessionType"
                                runat="server"
                                CssClass="booking-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSessionType_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>

                        <div class="booking-field">

                            <label>
                                TRAINER
                            </label>

                            <asp:DropDownList
                                ID="ddlTrainer"
                                runat="server"
                                CssClass="booking-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlTrainer_SelectedIndexChanged">
                            </asp:DropDownList>

                        </div>

                        <div class="booking-field">

                            <label>
                                DATE
                            </label>

                            <asp:TextBox
                                ID="txtDate"
                                runat="server"
                                TextMode="Date"
                                CssClass="booking-input"
                                AutoPostBack="true"
                                OnTextChanged="txtDate_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="booking-field">

                            <label>
                                AVAILABLE TIME
                            </label>

                            <asp:DropDownList
                                ID="ddlTime"
                                runat="server"
                                CssClass="booking-input">
                            </asp:DropDownList>

                        </div>

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            CssClass="booking-message">
                        </asp:Label>

                        <asp:Button
                            ID="btnBook"
                            runat="server"
                            Text="BOOK TRAINING SESSION →"
                            CssClass="booking-button"
                            OnClientClick="return confirm('Confirm this training-session booking?');"
                            OnClick="btnBook_Click" />

                    </div>

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

        <section class="booking-info-section">

            <div class="booking-info-card">

                <div class="booking-info-number">
                    01
                </div>

                <div>

                    <span class="booking-label">
                        ACTIVE MEMBERSHIP
                    </span>

                    <h3>
                        Membership <span>Validation.</span>
                    </h3>

                    <p>
                        Your payment history is checked before the
                        booking is saved.
                    </p>

                </div>

            </div>

            <div class="booking-info-card">

                <div class="booking-info-number">
                    02
                </div>

                <div>

                    <span class="booking-label">
                        CONFLICT PREVENTION
                    </span>

                    <h3>
                        No <span>Overlaps.</span>
                    </h3>

                    <p>
                        You cannot book another session that overlaps
                        an existing active booking.
                    </p>

                </div>

            </div>

            <div class="booking-info-card">

                <div class="booking-info-number">
                    03
                </div>

                <div>

                    <span class="booking-label">
                        AUTOMATIC DURATION
                    </span>

                    <h3>
                        Accurate <span>Times.</span>
                    </h3>

                    <p>
                        Duration is calculated automatically from the
                        selected time slot.
                    </p>

                </div>

            </div>

        </section>

    </div>

</asp:Content>