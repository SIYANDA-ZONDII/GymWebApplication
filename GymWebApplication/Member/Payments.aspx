<%@ Page Title="My Payments"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="Payments.aspx.cs"
    Inherits="GymWebApplication.Member.Payments" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <link
        rel="stylesheet"
        href="<%= ResolveUrl("~/Content/Payments.css") %>" />

    <div class="payments-page">

       

        <asp:Label
            ID="lblPaymentMessage"
            runat="server"
            CssClass="payments-message">
        </asp:Label>

        <section class="payment-member-summary">

            <div class="payment-member-summary-heading">

                <span class="section-label">
                    MEMBER INFORMATION
                </span>

                <h2>
                    Your <span>Plan.</span>
                </h2>

            </div>

            <div class="payment-member-summary-grid">

                <div class="payment-summary-value">

                    <span>MEMBER</span>

                    <strong>
                        <asp:Label
                            ID="lblMemberName"
                            runat="server"
                            Text="—" />
                    </strong>

                </div>

                <div class="payment-summary-value">

                    <span>MEMBER ID</span>

                    <strong>
                        <asp:Label
                            ID="lblMemberID"
                            runat="server"
                            Text="—" />
                    </strong>

                </div>

                <div class="payment-summary-value">

                    <span>MEMBERSHIP TYPE</span>

                    <strong>
                        <asp:Label
                            ID="lblMembershipType"
                            runat="server"
                            Text="—" />
                    </strong>

                </div>

                <div class="payment-summary-value">

                    <span>PLAN PRICE</span>

                    <strong>
                        <asp:Label
                            ID="lblPlanPrice"
                            runat="server"
                            Text="—" />
                    </strong>

                </div>

            </div>

            <div class="payment-plan-description">

                <span>PLAN DESCRIPTION</span>

                <p>
                    <asp:Label
                        ID="lblMembershipDescription"
                        runat="server"
                        Text="—" />
                </p>

            </div>

            <a
                href="<%= ResolveUrl("~/Member/MakePayment.aspx") %>"
                class="make-payment-link">
                MAKE A PAYMENT →
            </a>

        </section>

        <section class="payment-statistics">

            <div class="payment-stat-card">

                <span>DISPLAYED PAYMENTS</span>

                <strong>
                    <asp:Label
                        ID="lblPaymentCount"
                        runat="server"
                        Text="0" />
                </strong>

                <p>Records matching filters</p>

            </div>

            <div class="payment-stat-card">

                <span>DISPLAYED TOTAL</span>

                <strong>
                    R
                    <asp:Label
                        ID="lblTotalPaid"
                        runat="server"
                        Text="0.00" />
                </strong>

                <p>Total displayed amount</p>

            </div>

            <div class="payment-stat-card">

                <span>LATEST PAYMENT</span>

                <strong class="payment-date-value">
                    <asp:Label
                        ID="lblLastPaymentDate"
                        runat="server"
                        Text="—" />
                </strong>

                <p>Most recent matching payment</p>

            </div>

        </section>

        <section class="payment-history-section">

            <div class="payment-history-heading">

                <span class="section-label">
                    PAYMENT HISTORY
                </span>

                <h2>
                    Payment <span>Records.</span>
                </h2>

                <p>
                    Search by payment ID, membership ID, plan or
                    payment type.
                </p>

            </div>

            <asp:UpdatePanel
                ID="PaymentsUpdatePanel"
                runat="server"
                UpdateMode="Conditional">

                <ContentTemplate>

                    <div class="payment-filters">

                        <div class="payment-filter-field">

                            <label>SEARCH</label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="payment-filter-input"
                                placeholder="Payment ID, plan or membership"
                                AutoPostBack="true"
                                OnTextChanged="txtSearch_TextChanged">
                            </asp:TextBox>

                        </div>

                        <div class="payment-filter-field">

                            <label>PAYMENT TYPE</label>

                            <asp:DropDownList
                                ID="ddlPaymentTypeFilter"
                                runat="server"
                                CssClass="payment-filter-input"
                                AutoPostBack="true"
                                OnSelectedIndexChanged="ddlPaymentTypeFilter_SelectedIndexChanged">

                                <asp:ListItem
                                    Text="All payment types"
                                    Value="All" />

                                <asp:ListItem
                                    Text="EFT"
                                    Value="EFT" />

                                <asp:ListItem
                                    Text="Cash"
                                    Value="Cash" />

                            </asp:DropDownList>

                        </div>

                        <div class="payment-filter-field">

                            <label>FROM DATE</label>

                            <asp:TextBox
                                ID="txtFromDate"
                                runat="server"
                                TextMode="Date"
                                CssClass="payment-filter-input">
                            </asp:TextBox>

                        </div>

                        <div class="payment-filter-field">

                            <label>TO DATE</label>

                            <asp:TextBox
                                ID="txtToDate"
                                runat="server"
                                TextMode="Date"
                                CssClass="payment-filter-input">
                            </asp:TextBox>

                        </div>

                        <div class="payment-filter-actions">

                            <asp:Button
                                ID="btnApplyFilters"
                                runat="server"
                                Text="APPLY"
                                CssClass="payment-filter-button"
                                CausesValidation="false"
                                OnClick="btnApplyFilters_Click" />

                            <asp:Button
                                ID="btnClearFilters"
                                runat="server"
                                Text="CLEAR"
                                CssClass="payment-clear-button"
                                CausesValidation="false"
                                OnClick="btnClearFilters_Click" />

                        </div>

                    </div>

                    <div class="payment-table-wrapper">

                        <asp:GridView
                            ID="PaymentsGrid"
                            runat="server"
                            AutoGenerateColumns="False"
                            CssClass="payment-table"
                            GridLines="None"
                            EmptyDataText="No payments match the selected filters."
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

                </ContentTemplate>

            </asp:UpdatePanel>

        </section>

    </div>

    <style type="text/css">

        .payments-message {
            display: block;
            margin: 20px 0;
            padding: 14px 16px;
            border-radius: 7px;
            background: #eef2f6;
            color: #142235;
            font-weight: 700;
        }

        .payments-message:empty {
            display: none;
        }

        .payments-success {
            border: 1px solid #8dd4a3;
            background: #e8f7ed;
            color: #146c2e;
        }

        .payments-error {
            border: 1px solid #efaaaa;
            background: #fdecec;
            color: #a51d1d;
        }

        .payment-member-summary {
            margin: 30px 0;
            padding: 28px;
            border-radius: 12px;
            background: #ffffff;
            color: #142235;
            box-shadow: 0 8px 24px rgba(0, 0, 0, .18);
        }

        .payment-member-summary-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 16px;
            margin: 22px 0;
        }

        .payment-summary-value {
            padding: 17px;
            border-radius: 7px;
            background: #eef2f6;
        }

        .payment-summary-value span,
        .payment-plan-description span {
            display: block;
            margin-bottom: 7px;
            color: #637083;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .payment-plan-description {
            padding: 17px;
            border-left: 5px solid #e49400;
            background: #fff8e8;
        }

        .payment-plan-description p {
            margin: 0;
        }

        .make-payment-link {
            display: inline-block;
            margin-top: 20px;
            padding: 12px 17px;
            border-radius: 6px;
            background: #e49400;
            color: #111d2b;
            font-size: 12px;
            font-weight: 900;
            text-decoration: none;
        }

        .payment-statistics {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin: 28px 0;
        }

        .payment-stat-card {
            padding: 22px;
            border-radius: 10px;
            border-bottom: 4px solid #e49400;
            background: #111d2b;
            color: #ffffff;
        }

        .payment-stat-card span {
            color: #e49400;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .payment-stat-card strong {
            display: block;
            margin: 7px 0;
            font-size: 29px;
        }

        .payment-stat-card p {
            margin: 0;
            color: #cbd3dc;
        }

        .payment-date-value {
            font-size: 19px !important;
        }

        .payment-history-section {
            margin: 40px 0;
        }

        .payment-filters {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr 1fr auto;
            gap: 14px;
            align-items: end;
            margin: 23px 0;
            padding: 20px;
            border-radius: 10px;
            background: #eef2f6;
        }

        .payment-filter-field label {
            display: block;
            margin-bottom: 7px;
            color: #142235;
            font-size: 11px;
            font-weight: 900;
            letter-spacing: 1px;
        }

        .payment-filter-input {
            width: 100%;
            min-height: 45px;
            padding: 10px 12px;
            border: 1px solid #cbd3dc;
            border-radius: 6px;
            background: #ffffff;
            color: #142235;
        }

        .payment-filter-actions {
            display: flex;
            gap: 7px;
        }

        .payment-filter-button,
        .payment-clear-button {
            min-height: 45px;
            padding: 10px 14px;
            border: none;
            border-radius: 6px;
            font-weight: 900;
            cursor: pointer;
        }

        .payment-filter-button {
            background: #e49400;
            color: #111d2b;
        }

        .payment-clear-button {
            background: #142235;
            color: #ffffff;
        }

        @media screen and (max-width: 1050px) {

            .payment-filters {
                grid-template-columns: 1fr 1fr;
            }

            .payment-member-summary-grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        @media screen and (max-width: 650px) {

            .payment-filters,
            .payment-member-summary-grid,
            .payment-statistics {
                grid-template-columns: 1fr;
            }
        }

    </style>

</asp:Content>