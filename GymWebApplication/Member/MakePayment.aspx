<%@ Page Title="Make Payment"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    MaintainScrollPositionOnPostBack="true"
    CodeBehind="MakePayment.aspx.cs"
    Inherits="GymWebApplication.Member.MakePayment" %>

<asp:Content
    ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>

        /* =========================================================
           PAGE
           ========================================================= */

        .make-payment-page {
            max-width: 1100px;
            margin: 0 auto;
            padding: 55px 25px 80px 25px;
            font-family: Arial, Helvetica, sans-serif;
        }

        .make-payment-page * {
            box-sizing: border-box;
        }


        /* =========================================================
           PAYMENT CARD
           ========================================================= */

        .make-payment-card {
            width: 100%;
            padding: 45px;
            border-radius: 20px;
            border: 1px solid rgba(255, 255, 255, 0.10);
            background: rgba(255, 255, 255, 0.045);
            box-shadow: 0 18px 45px rgba(0, 0, 0, 0.20);
        }


        /* =========================================================
           CARD DETAILS AREA
           ========================================================= */

        .card-payment-area {
            padding: 32px;
            border: 1px solid rgba(255, 157, 0, 0.25);
            border-radius: 16px;
            background: rgba(0, 0, 0, 0.10);
        }


        /* =========================================================
           CARD HEADER
           ========================================================= */

        .card-section-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 20px;
            margin-bottom: 32px;
        }

        .card-section-title span {
            display: block;
            color: #ff9d00;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 1.5px;
            margin-bottom: 8px;
        }

        .card-section-title h2 {
            color: #ffffff;
            font-size: 25px;
            font-weight: 500;
            margin: 0;
        }

        .mastercard-badge {
            display: flex;
            align-items: center;
            justify-content: center;
            min-width: 138px;
            min-height: 48px;
            padding: 10px 18px;
            border-radius: 9px;
            background: rgba(255, 255, 255, 0.08);
            color: #ffffff;
            font-size: 14px;
            font-weight: 800;
        }


        /* =========================================================
           FORM GROUPS
           ========================================================= */

        .payment-form-group {
            margin-bottom: 25px;
        }

        .payment-form-group label {
            display: block;
            color: #b7c8ce;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 1.2px;
            margin-bottom: 10px;
        }


        /* =========================================================
           CARD NUMBER / NAME
           ========================================================= */

        .card-main-fields {
            max-width: 520px;
        }


        /* =========================================================
           INPUTS
           ========================================================= */

        .payment-input {
            display: block;
            width: 100%;
            height: 60px;
            padding: 0 17px;
            color: #ffffff !important;
            background-color: #082f39 !important;
            border: 1px solid rgba(255, 255, 255, 0.14);
            border-radius: 11px;
            outline: none;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 15px;
            transition:
                border-color 0.2s ease,
                box-shadow 0.2s ease,
                background-color 0.2s ease;
        }

        .payment-input::placeholder {
            color: #718c95;
        }

        .payment-input:focus {
            border-color: #ff9d00;
            box-shadow: 0 0 0 3px rgba(255, 157, 0, 0.10);
            background-color: #0a3742 !important;
        }

        select.payment-input {
            cursor: pointer;
        }

        select.payment-input option {
            background: #ffffff;
            color: #111111;
        }


        /* =========================================================
           EXPIRY + CVV
           ========================================================= */

        .expiry-cvv-grid {
            display: grid;
            grid-template-columns: 1fr 1fr 1fr;
            gap: 22px;
            margin-top: 4px;
        }


        /* =========================================================
           VALIDATION
           ========================================================= */

        .payment-message {
            display: block;
            margin-top: 7px;
            color: #ffb4b4;
            font-size: 12px;
            line-height: 1.5;
        }

        .payment-validation-summary {
            margin-top: 20px;
            padding: 15px 18px;
            color: #ffd1d1;
            background: rgba(255, 76, 76, 0.08);
            border: 1px solid rgba(255, 100, 100, 0.20);
            border-radius: 9px;
            font-size: 13px;
        }

        .payment-validation-summary ul {
            margin-top: 8px;
            margin-bottom: 0;
        }


        /* =========================================================
           BUTTONS
           ========================================================= */

        .payment-actions {
            display: grid;
            grid-template-columns: 1fr 140px;
            gap: 15px;
            margin-top: 34px;
        }

        .submit-payment-button,
        .cancel-payment-button {
            height: 60px;
            border-radius: 11px;
            font-size: 13px;
            font-weight: 800;
            letter-spacing: 1px;
            cursor: pointer;
            transition:
                transform 0.2s ease,
                background-color 0.2s ease,
                opacity 0.2s ease;
        }

        .submit-payment-button {
            border: none;
            background: #ff9d00;
            color: #082f39;
        }

        .submit-payment-button:hover {
            background: #ffac24;
            transform: translateY(-1px);
        }

        .submit-payment-button:disabled {
            opacity: 0.45;
            cursor: not-allowed;
            transform: none;
        }

        .cancel-payment-button {
            border: 1px solid rgba(255, 255, 255, 0.15);
            background: transparent;
            color: #d0dade;
        }

        .cancel-payment-button:hover {
            background: rgba(255, 255, 255, 0.06);
        }


        /* =========================================================
           RESPONSIVE
           ========================================================= */

        @media (max-width: 750px) {

            .make-payment-page {
                padding: 30px 15px 50px 15px;
            }

            .make-payment-card {
                padding: 20px;
            }

            .card-payment-area {
                padding: 22px;
            }

            .expiry-cvv-grid {
                grid-template-columns: 1fr;
                gap: 0;
            }

            .payment-actions {
                grid-template-columns: 1fr;
            }

            .card-main-fields {
                max-width: none;
            }

        }


        @media (max-width: 500px) {

            .card-section-header {
                flex-direction: column;
            }

            .mastercard-badge {
                min-width: 120px;
            }

            .make-payment-card {
                padding: 15px;
            }

            .card-payment-area {
                padding: 18px;
            }

        }

    </style>


    <div class="make-payment-page">

        <div class="make-payment-card">

            <!-- =====================================================
                 MASTERCARD DETAILS
                 ===================================================== -->

            <div class="card-payment-area">

                <div class="card-section-header">

                    <div class="card-section-title">

                        <span>
                            CARD DETAILS
                        </span>

                        <h2>
                            Mastercard Information
                        </h2>

                    </div>

                    <div class="mastercard-badge">
                        Mastercard
                    </div>

                </div>


                <!-- =================================================
                     MAIN CARD FIELDS
                     ================================================= -->

                <div class="card-main-fields">


                    <!-- =============================================
                         CARDHOLDER NAME
                         ============================================= -->

                    <div class="payment-form-group">

                        <label>
                            CARDHOLDER NAME
                        </label>

                        <asp:TextBox
                            ID="txtCardholderName"
                            runat="server"
                            CssClass="payment-input"
                            MaxLength="100"
                            placeholder="Name as shown on card"
                            autocomplete="cc-name">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvCardholderName"
                            runat="server"
                            ControlToValidate="txtCardholderName"
                            ErrorMessage="Cardholder name is required."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RequiredFieldValidator>

                        <asp:RegularExpressionValidator
                            ID="revCardholderName"
                            runat="server"
                            ControlToValidate="txtCardholderName"
                            ValidationExpression="^[A-Za-zÀ-ÿ][A-Za-zÀ-ÿ\s.'-]{1,99}$"
                            ErrorMessage="Please enter a valid cardholder name."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RegularExpressionValidator>

                    </div>


                    <!-- =============================================
                         MASTERCARD NUMBER
                         ============================================= -->

                    <div class="payment-form-group">

                        <label>
                            MASTERCARD NUMBER
                        </label>

                        <asp:TextBox
                            ID="txtCardNumber"
                            runat="server"
                            CssClass="payment-input"
                            MaxLength="16"
                            placeholder="Enter 16-digit Mastercard number"
                            inputmode="numeric"
                            autocomplete="cc-number">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvCardNumber"
                            runat="server"
                            ControlToValidate="txtCardNumber"
                            ErrorMessage="Card number is required."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RequiredFieldValidator>

                        <asp:RegularExpressionValidator
                            ID="revCardNumber"
                            runat="server"
                            ControlToValidate="txtCardNumber"
                            ValidationExpression="^\d{16}$"
                            ErrorMessage="Card number must contain exactly 16 digits."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RegularExpressionValidator>

                    </div>

                </div>


                <!-- =================================================
                     EXPIRY + CVV
                     ================================================= -->

                <div class="expiry-cvv-grid">


                    <!-- =============================================
                         EXPIRY MONTH
                         ============================================= -->

                    <div class="payment-form-group">

                        <label>
                            EXPIRY MONTH
                        </label>

                        <asp:DropDownList
                            ID="ddlExpiryMonth"
                            runat="server"
                            CssClass="payment-input">

                            <asp:ListItem Text="Month" Value="" />

                            <asp:ListItem Text="01" Value="1" />
                            <asp:ListItem Text="02" Value="2" />
                            <asp:ListItem Text="03" Value="3" />
                            <asp:ListItem Text="04" Value="4" />
                            <asp:ListItem Text="05" Value="5" />
                            <asp:ListItem Text="06" Value="6" />
                            <asp:ListItem Text="07" Value="7" />
                            <asp:ListItem Text="08" Value="8" />
                            <asp:ListItem Text="09" Value="9" />
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="11" Value="11" />
                            <asp:ListItem Text="12" Value="12" />

                        </asp:DropDownList>

                        <asp:RequiredFieldValidator
                            ID="rfvExpiryMonth"
                            runat="server"
                            ControlToValidate="ddlExpiryMonth"
                            InitialValue=""
                            ErrorMessage="Expiry month is required."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RequiredFieldValidator>

                    </div>


                    <!-- =============================================
                         EXPIRY YEAR
                         ============================================= -->

                    <div class="payment-form-group">

                        <label>
                            EXPIRY YEAR
                        </label>

                        <asp:DropDownList
                            ID="ddlExpiryYear"
                            runat="server"
                            CssClass="payment-input">

                            <asp:ListItem
                                Text="Select year"
                                Value="" />

                        </asp:DropDownList>

                        <asp:RequiredFieldValidator
                            ID="rfvExpiryYear"
                            runat="server"
                            ControlToValidate="ddlExpiryYear"
                            InitialValue=""
                            ErrorMessage="Expiry year is required."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RequiredFieldValidator>

                    </div>


                    <!-- =============================================
                         CVV
                         ============================================= -->

                    <div class="payment-form-group">

                        <label>
                            CVV
                        </label>

                        <asp:TextBox
                            ID="txtCVV"
                            runat="server"
                            CssClass="payment-input"
                            MaxLength="3"
                            TextMode="Password"
                            placeholder="3 digits"
                            inputmode="numeric"
                            autocomplete="cc-csc">
                        </asp:TextBox>

                        <asp:RequiredFieldValidator
                            ID="rfvCVV"
                            runat="server"
                            ControlToValidate="txtCVV"
                            ErrorMessage="CVV is required."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RequiredFieldValidator>

                        <asp:RegularExpressionValidator
                            ID="revCVV"
                            runat="server"
                            ControlToValidate="txtCVV"
                            ValidationExpression="^\d{3}$"
                            ErrorMessage="CVV must contain exactly 3 digits."
                            CssClass="payment-message"
                            Display="Dynamic"
                            ValidationGroup="PaymentValidation">
                        </asp:RegularExpressionValidator>

                    </div>

                </div>

            </div>


            <!-- =====================================================
                 VALIDATION SUMMARY
                 ===================================================== -->

            <asp:ValidationSummary
                ID="PaymentValidationSummary"
                runat="server"
                ValidationGroup="PaymentValidation"
                HeaderText="Please correct the following:"
                DisplayMode="BulletList"
                CssClass="payment-validation-summary" />


            <!-- =====================================================
                 PAYMENT MESSAGE
                 ===================================================== -->

            <asp:Label
                ID="lblPaymentMessage"
                runat="server"
                CssClass="payment-message">
            </asp:Label>


            <!-- =====================================================
                 BUTTONS
                 ===================================================== -->

            <div class="payment-actions">

                <asp:Button
                    ID="btnSubmitPayment"
                    runat="server"
                    Text="PAY WITH MASTERCARD"
                    CssClass="submit-payment-button"
                    ValidationGroup="PaymentValidation"
                    OnClientClick="if (typeof(Page_ClientValidate) == 'function') { if (!Page_ClientValidate('PaymentValidation')) return false; } return confirm('Confirm this Mastercard membership payment?');"
                    OnClick="btnSubmitPayment_Click" />

                <asp:Button
                    ID="btnCancel"
                    runat="server"
                    Text="CANCEL"
                    CssClass="cancel-payment-button"
                    CausesValidation="false"
                    OnClientClick="return confirm('Cancel this payment and return to your payment history?');"
                    PostBackUrl="~/Member/Payments.aspx" />

            </div>


            <!-- =====================================================
                 HIDDEN CONTROLS
                 Required by the existing code-behind.
                 They are NOT displayed to the member.
                 ===================================================== -->

            <div style="display: none;">

                <asp:Label
                    ID="lblMemberName"
                    runat="server" />

                <asp:Label
                    ID="lblMemberID"
                    runat="server" />

                <asp:Label
                    ID="lblMemberType"
                    runat="server" />

                <asp:Label
                    ID="lblMembershipPlan"
                    runat="server" />

                <asp:Label
                    ID="lblPlanDescription"
                    runat="server" />

                <asp:Label
                    ID="lblPlanDuration"
                    runat="server" />

                <asp:Label
                    ID="lblPaymentAmount"
                    runat="server" />

                <asp:TextBox
                    ID="txtPaymentDate"
                    runat="server" />

                <asp:TextBox
                    ID="txtPaymentMethod"
                    runat="server"
                    Text="Mastercard" />

            </div>

        </div>

    </div>

</asp:Content>