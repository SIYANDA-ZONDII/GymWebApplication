<%@ Page Title="My Membership"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Membership.aspx.cs"
    Inherits="GymWebApplication.Member.Membership" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Membership.css") %>" />

    <div class="membership-page">

       

        <section class="membership-overview">

            <span class="section-label">
                MEMBERSHIP OVERVIEW
            </span>

            <h2>
                Your <span>Membership</span>
            </h2>

            <p class="section-description">
                Each recorded payment provides one month of
                membership. Consecutive payments extend the
                calculated expiry date.
            </p>

            <div class="membership-card">

                <div class="membership-card-top">

                    <div>

                        <span class="membership-card-label">
                            CURRENT MEMBERSHIP
                        </span>

                        <h3>
                            <asp:Label
                                ID="lblPlanType"
                                runat="server"
                                Text="—" />
                        </h3>

                    </div>

                    <div class="membership-status">

                        <asp:Label
                            ID="lblMemberStatus"
                            runat="server"
                            Text="INACTIVE" />

                    </div>

                </div>

                <div class="membership-card-line"></div>

                <div class="membership-details">

                    <div class="membership-detail">

                        <span>MEMBER ID</span>

                        <strong>
                            <asp:Label
                                ID="lblMemberID"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>MEMBER NAME</span>

                        <strong>
                            <asp:Label
                                ID="lblMemberName"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>JOIN DATE</span>

                        <strong>
                            <asp:Label
                                ID="lblStartDate"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>MEMBERSHIP ID</span>

                        <strong>
                            <asp:Label
                                ID="lblMembershipID"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>PLAN PRICE</span>

                        <strong>
                            <asp:Label
                                ID="lblPlanPrice"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>VALIDITY PER PAYMENT</span>

                        <strong>
                            <asp:Label
                                ID="lblPlanDuration"
                                runat="server"
                                Text="1 month" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>LAST PAYMENT</span>

                        <strong>
                            <asp:Label
                                ID="lblLastPaymentDate"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>CALCULATED EXPIRY</span>

                        <strong>
                            <asp:Label
                                ID="lblExpiryDate"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>PAYMENT TYPE</span>

                        <strong>
                            <asp:Label
                                ID="lblPaymentType"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>LAST PAYMENT AMOUNT</span>

                        <strong>
                            <asp:Label
                                ID="lblPaymentBalance"
                                runat="server"
                                Text="—" />
                        </strong>

                    </div>

                    <div class="membership-detail">

                        <span>TOTAL PAYMENTS</span>

                        <strong>
                            <asp:Label
                                ID="lblPaymentCount"
                                runat="server"
                                Text="0" />
                        </strong>

                    </div>

                </div>

                <div class="membership-plan-description">

                    <span class="membership-card-label">
                        PLAN DESCRIPTION
                    </span>

                    <p>
                        <asp:Label
                            ID="lblPlanDescription"
                            runat="server"
                            Text="—" />
                    </p>

                </div>

            </div>

        </section>

        <section class="membership-features">

            <span class="section-label">
                MEMBERSHIP HISTORY
            </span>

            <h2>
                Your <span>Payment Records</span>
            </h2>

            <p class="section-description">
                Payments used to calculate your membership validity.
            </p>

            <div class="payment-table-wrapper">

                <asp:GridView
                    ID="MembershipGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="payment-table"
                    GridLines="None"
                    EmptyDataText="No membership payments were found."
                    AllowPaging="false">

                    <Columns>

                        <asp:BoundField
                            DataField="Payment_ID"
                            HeaderText="PAYMENT ID" />

                        <asp:BoundField
                            DataField="Membership_ID"
                            HeaderText="MEMBERSHIP" />

                        <asp:BoundField
                            DataField="Plan_Type"
                            HeaderText="PLAN" />

                        <asp:BoundField
                            DataField="Payment_Date"
                            HeaderText="DATE"
                            DataFormatString="{0:dd/MM/yyyy}" />

                        <asp:BoundField
                            DataField="Payment_Balance"
                            HeaderText="AMOUNT"
                            DataFormatString="R {0:N2}" />

                        <asp:BoundField
                            DataField="Payment_Type"
                            HeaderText="TYPE" />

                    </Columns>

                </asp:GridView>

            </div>

            <asp:Label
                ID="MembershipMessage"
                runat="server"
                CssClass="membership-message">
            </asp:Label>

        </section>

        <section class="membership-notice">

            <div class="notice-icon">
                !
            </div>

            <div>

                <span class="section-label">
                    MEMBERSHIP STATUS
                </span>

                <h2>
                    Keep Your Membership
                    <span>Active</span>
                </h2>

                <p>
                    <asp:Label
                        ID="lblNotice"
                        runat="server" />
                </p>

                <a
                    href="<%= ResolveUrl("~/Member/MakePayment.aspx") %>"
                    class="membership-payment-link">
                    MAKE A PAYMENT →
                </a>

            </div>

        </section>

    </div>

    <style type="text/css">

        .membership-status-active {
            color: #146c2e !important;
        }

        .membership-status-inactive {
            color: #a51d1d !important;
        }

        .membership-message {
            display: block;
            margin: 18px 0;
            padding: 13px 15px;
            border-radius: 7px;
            background: #eef2f6;
            color: #142235;
            font-weight: 600;
        }

        .membership-payment-link {
            display: inline-block;
            margin-top: 10px;
            color: #e49400;
            font-size: 12px;
            font-weight: 900;
            text-decoration: none;
        }

    </style>

</asp:Content>