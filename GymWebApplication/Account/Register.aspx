
<%@ Page Title="Register Member" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="GymWebApplication.Account.Register" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .member-form-page {
            max-width: 950px;
            margin: 30px auto;
            padding: 30px;
            background: #fff;
            border-radius: 12px;
        }

        .member-form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 18px;
        }

        .member-form-field label {
            display: block;
            font-weight: 600;
            margin-bottom: 6px;
        }

        .member-form-field input,
        .member-form-field select {
            width: 100%;
            padding: 10px;
            border: 1px solid #bbb;
            border-radius: 6px;
            box-sizing: border-box;
        }

        .member-form-section {
            margin: 28px 0 15px;
        }

        .member-form-note {
            padding: 12px 15px;
            margin: 18px 0;
            background: #fff4d8;
            border-radius: 6px;
        }

        .member-form-message {
            display: block;
            padding: 13px;
            margin: 16px 0;
            border-radius: 6px;
        }

        .member-form-error {
            background: #ffe3e3;
            color: #841b1b;
        }

        .member-form-success {
            background: #dff5e6;
            color: #176638;
        }

        .member-form-button {
            margin-top: 24px;
            padding: 12px 26px;
            cursor: pointer;
        }

        @media (max-width: 650px) {
            .member-form-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>

    <div class="member-form-page">
        <h1>Register Member</h1>

        <p>
            Fields marked * are required to create your account.
            You may fill in the other fields now or complete them
            later in your profile.
        </p>

        <asp:PlaceHolder ID="MessagePanel"
            runat="server"
            Visible="false">
            <div id="MessageBox"
                runat="server"
                class="member-form-message">
                <asp:Literal ID="MessageText"
                    runat="server" />
            </div>
        </asp:PlaceHolder>

        <h2 class="member-form-section">Required account details</h2>

        <div class="member-form-grid">
            <div class="member-form-field">
                <label for="<%= MemberID.ClientID %>">Member ID *</label>
                <asp:TextBox ID="MemberID"
                    runat="server"
                    MaxLength="9" />
                <small>Exactly 6 or 9 digits.</small>
            </div>

            <div class="member-form-field">
                <label for="<%= FirstName.ClientID %>">First name *</label>
                <asp:TextBox ID="FirstName"
                    runat="server"
                    MaxLength="50" />
            </div>

            <div class="member-form-field">
                <label for="<%= LastName.ClientID %>">Last name *</label>
                <asp:TextBox ID="LastName"
                    runat="server"
                    MaxLength="50" />
            </div>

            <div class="member-form-field">
                <label for="<%= DateOfBirth.ClientID %>">
                    Date of birth *
                </label>
                <asp:TextBox ID="DateOfBirth"
                    runat="server"
                    TextMode="Date" />
                <small>You must be at least 18 years old.</small>
            </div>

            <div class="member-form-field">
                <label for="<%= MemberType.ClientID %>">
                    Member type *
                </label>
                <asp:DropDownList ID="MemberType"
                    runat="server">
                    <asp:ListItem Text="Select member type" Value="" />
                    <asp:ListItem Text="Normal Student"
                        Value="Normal Student" />
                    <asp:ListItem Text="Sport Science Student"
                        Value="Sport Science Student" />
                    <asp:ListItem Text="Staff"
                        Value="Staff" />
                </asp:DropDownList>
            </div>

            <div class="member-form-field">
                <label for="<%= Gender.ClientID %>">
                    Gender *
                </label>
                <asp:DropDownList ID="Gender"
                    runat="server">
                    <asp:ListItem Text="Select gender" Value="" />
                    <asp:ListItem Text="Male" Value="Male" />
                    <asp:ListItem Text="Female" Value="Female" />
                    <asp:ListItem Text="Other" Value="Other" />
                    <asp:ListItem Text="Prefer not to say"
                        Value="Prefer not to say" />
                </asp:DropDownList>
            </div>

            <div class="member-form-field">
                <label for="<%= Email.ClientID %>">
                    Email address *
                </label>
                <asp:TextBox ID="Email"
                    runat="server"
                    TextMode="Email"
                    MaxLength="100" />
            </div>
        </div>

        <h2 class="member-form-section">
            Additional personal and contact details
        </h2>

        <p>
            These fields are optional during registration. Except
            for telephone, you will need to complete them before
            making a payment.
        </p>

        <div class="member-form-grid">
            <div class="member-form-field">
                <label for="<%= PhoneNumber.ClientID %>">
                    Phone number
                </label>
                <asp:TextBox ID="PhoneNumber"
                    runat="server"
                    MaxLength="10" />
            </div>

            <div class="member-form-field">
                <label for="<%= TelephoneNumber.ClientID %>">
                    Telephone (optional even for payment)
                </label>
                <asp:TextBox ID="TelephoneNumber"
                    runat="server"
                    MaxLength="10" />
            </div>

            <div class="member-form-field">
                <label for="<%= StreetNumber.ClientID %>">
                    Street number
                </label>
                <asp:TextBox ID="StreetNumber"
                    runat="server"
                    MaxLength="10" />
            </div>

            <div class="member-form-field">
                <label for="<%= StreetName.ClientID %>">
                    Street name
                </label>
                <asp:TextBox ID="StreetName"
                    runat="server"
                    MaxLength="100" />
            </div>

            <div class="member-form-field">
                <label for="<%= City.ClientID %>">
                    City
                </label>
                <asp:TextBox ID="City"
                    runat="server"
                    MaxLength="50" />
            </div>

            <div class="member-form-field">
                <label for="<%= Province.ClientID %>">
                    Province
                </label>
                <asp:DropDownList ID="Province"
                    runat="server">
                    <asp:ListItem Text="Select province" Value="" />
                    <asp:ListItem Text="Eastern Cape"
                        Value="Eastern Cape" />
                    <asp:ListItem Text="Free State"
                        Value="Free State" />
                    <asp:ListItem Text="Gauteng"
                        Value="Gauteng" />
                    <asp:ListItem Text="KwaZulu-Natal"
                        Value="KwaZulu-Natal" />
                    <asp:ListItem Text="Limpopo"
                        Value="Limpopo" />
                    <asp:ListItem Text="Mpumalanga"
                        Value="Mpumalanga" />
                    <asp:ListItem Text="Northern Cape"
                        Value="Northern Cape" />
                    <asp:ListItem Text="North West"
                        Value="North West" />
                    <asp:ListItem Text="Western Cape"
                        Value="Western Cape" />
                </asp:DropDownList>
            </div>

            <div class="member-form-field">
                <label for="<%= PostalCode.ClientID %>">
                    Postal code
                </label>
                <asp:TextBox ID="PostalCode"
                    runat="server"
                    MaxLength="10" />
            </div>

            <div class="member-form-field">
                <label for="<%= EmergencyContactName.ClientID %>">
                    Emergency contact name
                </label>
                <asp:TextBox ID="EmergencyContactName"
                    runat="server"
                    MaxLength="100" />
            </div>

            <div class="member-form-field">
                <label for="<%= EmergencyContactNumber.ClientID %>">
                    Emergency contact number
                </label>
                <asp:TextBox ID="EmergencyContactNumber"
                    runat="server"
                    MaxLength="10" />
            </div>

            <div class="member-form-field">
                <label for="<%= EmergencyRelationship.ClientID %>">
                    Emergency contact relationship
                </label>
                <asp:TextBox ID="EmergencyRelationship"
                    runat="server"
                    MaxLength="50" />
            </div>
        </div>

        <h2 class="member-form-section">Password</h2>

        <div class="member-form-grid">
            <div class="member-form-field">
                <label for="<%= Password.ClientID %>">
                    Password *
                </label>
                <asp:TextBox ID="Password"
                    runat="server"
                    TextMode="Password"
                    MaxLength="100" />
                <small>
                    At least 8 characters, including uppercase,
                    lowercase, a number and a special character.
                </small>
            </div>

            <div class="member-form-field">
                <label for="<%= ConfirmPassword.ClientID %>">
                    Confirm password *
                </label>
                <asp:TextBox ID="ConfirmPassword"
                    runat="server"
                    TextMode="Password"
                    MaxLength="100" />
            </div>
        </div>

        <asp:Button ID="RegisterButton"
            runat="server"
            Text="Create account"
            CssClass="member-form-button"
            OnClick="RegisterMember_Click" />
    </div>
</asp:Content>
