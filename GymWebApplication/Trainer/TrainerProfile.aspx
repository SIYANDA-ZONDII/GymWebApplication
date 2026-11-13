<%@ Page Title="Trainer Profile"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="TrainerProfile.aspx.cs"
    Inherits="GymWebApplication.Trainer.TrainerProfile" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="trainer-page">



        <!-- =====================================================
             PROFILE INFORMATION
             ===================================================== -->

        <section class="trainer-content-section">

            <span class="trainer-section-label">
                PERSONAL INFORMATION
            </span>

            <h2>
                Trainer <span>Details.</span>
            </h2>

            <p class="trainer-description">
                Your information is retrieved from your trainer
                account. You can update your personal and contact
                information below.
            </p>


            <!-- =================================================
                 TRAINER IDENTITY
                 ================================================= -->

            <div class="trainer-info-grid">


                <!-- STAFF ID -->

                <div class="trainer-info-box">

                    <span>
                        STAFF ID
                    </span>

                    <strong>
                        <asp:Label
                            ID="lblStaffID"
                            runat="server"
                            Text="Not available" />
                    </strong>

                </div>


                <!-- ROLE -->

                <div class="trainer-info-box">

                    <span>
                        ROLE
                    </span>

                    <strong>
                        TRAINER
                    </strong>

                </div>


                <!-- STATUS -->

                <div class="trainer-info-box">

                    <span>
                        STATUS
                    </span>

                    <strong>
                        <asp:Label
                            ID="lblStatus"
                            runat="server"
                            Text="Not available" />
                    </strong>

                </div>


                <!-- SPECIALISATION -->

                <div class="trainer-info-box">

                    <span>
                        SPECIALISATION
                    </span>

                    <strong>
                        <asp:Label
                            ID="lblSpecialisation"
                            runat="server"
                            Text="Not available" />
                    </strong>

                </div>

            </div>

        </section>



        <!-- =====================================================
             EDIT PROFILE
             ===================================================== -->

        <section class="trainer-content-section">

            <span class="trainer-section-label">
                EDIT PROFILE
            </span>

            <h2>
                Update Your <span>Details.</span>
            </h2>

            <p class="trainer-description">
                Update your personal information and contact
                details. Your staff ID, role, status, join date
                and specialisation cannot be changed here.
            </p>



            <div class="trainer-edit-card">


                <!-- =================================================
                     FIRST NAME
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtFirstName.ClientID %>">
                        FIRST NAME
                    </label>

                    <asp:TextBox
                        ID="txtFirstName"
                        runat="server"
                        CssClass="trainer-form-control"
                        MaxLength="50" />

                    <asp:RequiredFieldValidator
                        ID="valFirstName"
                        runat="server"
                        ControlToValidate="txtFirstName"
                        CssClass="trainer-error"
                        Display="Dynamic"
                        ErrorMessage="First name is required." />

                </div>



                <!-- =================================================
                     LAST NAME
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtLastName.ClientID %>">
                        LAST NAME
                    </label>

                    <asp:TextBox
                        ID="txtLastName"
                        runat="server"
                        CssClass="trainer-form-control"
                        MaxLength="50" />

                    <asp:RequiredFieldValidator
                        ID="valLastName"
                        runat="server"
                        ControlToValidate="txtLastName"
                        CssClass="trainer-error"
                        Display="Dynamic"
                        ErrorMessage="Last name is required." />

                </div>



                <!-- =================================================
                     EMAIL
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtEmail.ClientID %>">
                        EMAIL ADDRESS
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="trainer-form-control"
                        TextMode="Email"
                        MaxLength="100" />

                    <asp:RequiredFieldValidator
                        ID="valEmail"
                        runat="server"
                        ControlToValidate="txtEmail"
                        CssClass="trainer-error"
                        Display="Dynamic"
                        ErrorMessage="Email address is required." />

                    <asp:RegularExpressionValidator
                        ID="valEmailFormat"
                        runat="server"
                        ControlToValidate="txtEmail"
                        CssClass="trainer-error"
                        Display="Dynamic"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Please enter a valid email address." />

                </div>



                <!-- =================================================
                     PHONE NUMBER
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtPhone.ClientID %>">
                        PHONE NUMBER
                    </label>

                    <asp:TextBox
                        ID="txtPhone"
                        runat="server"
                        CssClass="trainer-form-control"
                        MaxLength="15" />

                </div>



                <!-- =================================================
                     TELEPHONE
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtTelephone.ClientID %>">
                        TELEPHONE
                    </label>

                    <asp:TextBox
                        ID="txtTelephone"
                        runat="server"
                        CssClass="trainer-form-control"
                        MaxLength="15" />

                </div>



                <!-- =================================================
                     DATE OF BIRTH
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= txtDOB.ClientID %>">
                        DATE OF BIRTH
                    </label>

                    <asp:TextBox
                        ID="txtDOB"
                        runat="server"
                        CssClass="trainer-form-control"
                        TextMode="Date" />

                </div>



                <!-- =================================================
                     GENDER
                     ================================================= -->

                <div class="trainer-form-group">

                    <label for="<%= ddlGender.ClientID %>">
                        GENDER
                    </label>

                    <asp:DropDownList
                        ID="ddlGender"
                        runat="server"
                        CssClass="trainer-form-control">

                        <asp:ListItem
                            Text="Select Gender"
                            Value="" />

                        <asp:ListItem
                            Text="Male"
                            Value="Male" />

                        <asp:ListItem
                            Text="Female"
                            Value="Female" />

                        <asp:ListItem
                            Text="Other"
                            Value="Other" />

                    </asp:DropDownList>

                </div>



                <!-- =================================================
                     JOIN DATE
                     ================================================= -->

                <div class="trainer-form-group">

                    <label>
                        JOIN DATE
                    </label>

                    <div class="trainer-readonly-field">

                        <asp:Label
                            ID="lblJoinDate"
                            runat="server"
                            Text="Not available" />

                    </div>

                </div>



                <!-- =================================================
                     SAVE MESSAGE
                     ================================================= -->

                <asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="trainer-message" />



                <!-- =================================================
                     BUTTONS
                     ================================================= -->

                <div class="trainer-form-actions">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="SAVE CHANGES"
                        CssClass="trainer-action-button"
                        OnClick="btnSave_Click" />

                    <asp:Button
                        ID="btnCancel"
                        runat="server"
                        Text="CANCEL"
                        CssClass="trainer-cancel-button"
                        CausesValidation="false"
                        OnClick="btnCancel_Click" />

                </div>

            </div>

        </section>



       


        <!-- =====================================================
             FOOTER
             ===================================================== -->

        <section class="trainer-footer">

            <div class="trainer-footer-line"></div>

            <p>
                UKZN Sport GMS · ISTN3AS 2026 · GroupWst23
            </p>

        </section>

    </div>



    <!-- =========================================================
         PAGE CSS
         ========================================================= -->

    <style type="text/css">

        .trainer-description {
            color: #718199;

            max-width: 800px;

            line-height: 1.7;

            margin-bottom: 30px;
        }


        /* =====================================================
           EDIT CARD
           ===================================================== */

        .trainer-edit-card {
            background: #f4f6fa;

            border-radius: 12px;

            padding: 35px;

            margin-top: 30px;

            box-shadow:
                0 10px 30px rgba(0,0,0,0.08);
        }


        /* =====================================================
           FORM GRID
           ===================================================== */

        .trainer-edit-card {
            display: grid;

            grid-template-columns:
                1fr 1fr;

            gap: 25px;
        }


        .trainer-form-group {
            display: flex;

            flex-direction: column;
        }


        .trainer-form-group label {
            color: #53657c;

            font-size: 11px;

            font-weight: 700;

            letter-spacing: 1.5px;

            margin-bottom: 9px;
        }


        /* =====================================================
           INPUT
           ===================================================== */

        .trainer-form-control {
            width: 100%;

            box-sizing: border-box;

            padding: 14px 15px;

            background: #ffffff;

            border: 1px solid #dce2e9;

            border-radius: 7px;

            color: #142235;

            font-size: 14px;

            outline: none;

            transition:
                border-color 0.2s ease,
                box-shadow 0.2s ease;
        }


        .trainer-form-control:focus {
            border-color: #e49400;

            box-shadow:
                0 0 0 3px
                rgba(228,148,0,0.12);
        }


        /* =====================================================
           READ ONLY
           ===================================================== */

        .trainer-readonly-field {
            width: 100%;

            box-sizing: border-box;

            padding: 14px 15px;

            background: #e9edf2;

            border: 1px solid #d8dee6;

            border-radius: 7px;

            color: #66758a;

            font-size: 14px;
        }


        /* =====================================================
           VALIDATION
           ===================================================== */

        .trainer-error {
            color: #c0392b;

            font-size: 12px;

            margin-top: 6px;
        }


        /* =====================================================
           MESSAGE
           ===================================================== */

        .trainer-message {
            grid-column: 1 / -1;

            display: block;

            font-size: 14px;

            font-weight: 600;

            padding: 12px 15px;

            border-radius: 6px;
        }


        /* =====================================================
           ACTIONS
           ===================================================== */

        .trainer-form-actions {
            grid-column: 1 / -1;

            display: flex;

            justify-content: flex-end;

            gap: 12px;

            margin-top: 10px;
        }


        .trainer-action-button,
        .trainer-cancel-button {
            display: inline-block;

            padding: 14px 24px;

            border-radius: 6px;

            font-size: 11px;

            font-weight: 700;

            letter-spacing: 1px;

            text-decoration: none;

            cursor: pointer;

            transition: all 0.2s ease;
        }


        .trainer-action-button {
            background: #e49400;

            color: #ffffff;

            border: none;
        }


        .trainer-action-button:hover {
            background: #c97d00;

            color: #ffffff;

            text-decoration: none;

            transform: translateY(-2px);
        }


        .trainer-cancel-button {
            background: #dfe4ea;

            color: #142235;

            border: none;
        }


        .trainer-cancel-button:hover {
            background: #cbd2da;
        }


        /* =====================================================
           MOBILE
           ===================================================== */

        @media screen and (max-width: 700px) {

            .trainer-edit-card {
                grid-template-columns: 1fr;

                padding: 25px;
            }


            .trainer-form-actions {
                grid-column: auto;

                flex-direction: column;
            }


            .trainer-action-button,
            .trainer-cancel-button {
                width: 100%;

                box-sizing: border-box;

                text-align: center;
            }

        }

    </style>

</asp:Content>