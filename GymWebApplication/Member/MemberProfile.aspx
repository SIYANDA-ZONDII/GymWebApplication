<%@ Page Title="My Profile" Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="MemberProfile.aspx.cs"
    Inherits="GymWebApplication.Member.MemberProfile" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

                    <style>
                    .member-profile-page {
                        max-width: 950px;
                        margin: 30px auto;
                        padding: 30px;
                        background: #fff;
                        color: #17283a;
                        border-radius: 12px;
                    }

                    .member-profile-page h1,
                    .member-profile-page h2,
                    .member-profile-page p {
                        color: #17283a;
                    }

                    .member-profile-grid {
                        display: grid;
                        grid-template-columns: repeat(2, minmax(0, 1fr));
                        gap: 18px;
                    }

                    .member-profile-item {
                        padding: 12px;
                        border: 1px solid #ddd;
                        border-radius: 7px;
                    }

                    .member-profile-item span {
                        display: block;
                        margin-bottom: 5px;
                        color: #555;
                    }

                    .member-profile-item strong {
                        color: #17283a;
                        overflow-wrap: anywhere;
                    }

                    .member-profile-field label {
                        display: block;
                        margin-bottom: 6px;
                        color: #17283a;
                        font-size: 14px;
                        font-weight: 600;
                    }

                    .member-profile-field input,
                    .member-profile-field select {
                        width: 100%;
                        padding: 10px;
                        border: 1px solid #bbb;
                        border-radius: 6px;
                        box-sizing: border-box;
                        color: #17283a;
                        background: #fff;
                    }

                    .member-profile-note {
                        padding: 14px;
                        margin: 16px 0;
                        background: #fff4d8;
                        color: #594300;
                        border-radius: 6px;
                    }

                    .member-profile-message {
                        display: block;
                        padding: 14px;
                        margin: 16px 0;
                        border-radius: 6px;
                    }

                    .member-profile-error {
                        background: #ffe3e3;
                        color: #841b1b;
                    }

                    .member-profile-success {
                        background: #dff5e6;
                        color: #176638;
                    }

                    .member-profile-actions {
                        margin-top: 22px;
                    }

                    .member-profile-actions input {
                        padding: 11px 22px;
                        margin-right: 10px;
                        cursor: pointer;
                    }

                    @media (max-width: 650px) {
                        .member-profile-grid {
                            grid-template-columns: 1fr;
                        }
                    }
                </style>

    <div class="member-profile-page">
        <h1>My Profile</h1>

        <p>
            View your registered details. Select
            <strong>Edit details</strong> when you want to update them.
        </p>

        <asp:Panel ID="PaymentNotice"
            runat="server"
            CssClass="member-profile-note"
            Visible="false">
            Complete the missing details before making a payment.
            Telephone is optional.
        </asp:Panel>

        <asp:Label ID="ProfileMessage"
            runat="server"
            Visible="false" />

        <asp:Panel ID="ViewPanel"
            runat="server">

            <div class="member-profile-grid">
                <div class="member-profile-item">
                    <span>Member ID</span>
                    <strong>
                        <asp:Label ID="ViewMemberID" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>First name</span>
                    <strong>
                        <asp:Label ID="ViewFirstName" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Last name</span>
                    <strong>
                        <asp:Label ID="ViewLastName" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Email</span>
                    <strong>
                        <asp:Label ID="ViewEmail" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Date of birth</span>
                    <strong>
                        <asp:Label ID="ViewDOB" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Member type</span>
                    <strong>
                        <asp:Label ID="ViewMemberType" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Join date</span>
                    <strong>
                        <asp:Label ID="ViewJoinDate" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Member status</span>
                    <strong>
                        <asp:Label ID="ViewStatus" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Phone number</span>
                    <strong>
                        <asp:Label ID="ViewPhone" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Telephone</span>
                    <strong>
                        <asp:Label ID="ViewTelephone" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Gender</span>
                    <strong>
                        <asp:Label ID="ViewGender" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Street number</span>
                    <strong>
                        <asp:Label ID="ViewStreetNumber" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Street name</span>
                    <strong>
                        <asp:Label ID="ViewStreetName" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>City</span>
                    <strong>
                        <asp:Label ID="ViewCity" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Province</span>
                    <strong>
                        <asp:Label ID="ViewProvince" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Postal code</span>
                    <strong>
                        <asp:Label ID="ViewPostalCode" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Emergency contact name</span>
                    <strong>
                        <asp:Label ID="ViewEmergencyName" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Emergency contact number</span>
                    <strong>
                        <asp:Label ID="ViewEmergencyNumber" runat="server" />
                    </strong>
                </div>

                <div class="member-profile-item">
                    <span>Emergency contact relationship</span>
                    <strong>
                        <asp:Label ID="ViewEmergencyRelationship"
                            runat="server" />
                    </strong>
                </div>
            </div>

            <div class="member-profile-actions">
                <asp:Button ID="EditButton"
                    runat="server"
                    Text="Edit details"
                    CausesValidation="false"
                    OnClick="EditButton_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="EditPanel"
            runat="server"
            Visible="false">

            <h2>Edit details</h2>

            <p>
                You can save some details now and finish the rest
                later. Telephone can always remain empty.
            </p>

            <div class="member-profile-grid">
                <div class="member-profile-field">
                    <label for="<%= DOBInput.ClientID %>">
                        Date of birth *
                    </label>
                    <asp:TextBox ID="DOBInput"
                        runat="server"
                        TextMode="Date" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= PhoneInput.ClientID %>">
                        Phone number
                    </label>
                    <asp:TextBox ID="PhoneInput"
                        runat="server"
                        MaxLength="10" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= TelephoneInput.ClientID %>">
                        Telephone (optional)
                    </label>
                    <asp:TextBox ID="TelephoneInput"
                        runat="server"
                        MaxLength="10" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= GenderInput.ClientID %>">
                        Gender
                    </label>
                    <asp:DropDownList ID="GenderInput"
                        runat="server">
                        <asp:ListItem Text="Select gender" Value="" />
                        <asp:ListItem Text="Male" Value="Male" />
                        <asp:ListItem Text="Female" Value="Female" />
                        <asp:ListItem Text="Other" Value="Other" />
                        <asp:ListItem Text="Prefer not to say"
                            Value="Prefer not to say" />
                    </asp:DropDownList>
                </div>

                <div class="member-profile-field">
                    <label for="<%= StreetNumberInput.ClientID %>">
                        Street number
                    </label>
                    <asp:TextBox ID="StreetNumberInput"
                        runat="server"
                        MaxLength="10" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= StreetNameInput.ClientID %>">
                        Street name
                    </label>
                    <asp:TextBox ID="StreetNameInput"
                        runat="server"
                        MaxLength="100" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= CityInput.ClientID %>">
                        City
                    </label>
                    <asp:TextBox ID="CityInput"
                        runat="server"
                        MaxLength="50" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= ProvinceInput.ClientID %>">
                        Province
                    </label>
                    <asp:DropDownList ID="ProvinceInput"
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

                <div class="member-profile-field">
                    <label for="<%= PostalCodeInput.ClientID %>">
                        Postal code
                    </label>
                    <asp:TextBox ID="PostalCodeInput"
                        runat="server"
                        MaxLength="10" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= EmergencyNameInput.ClientID %>">
                        Emergency contact name
                    </label>
                    <asp:TextBox ID="EmergencyNameInput"
                        runat="server"
                        MaxLength="100" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= EmergencyNumberInput.ClientID %>">
                        Emergency contact number
                    </label>
                    <asp:TextBox ID="EmergencyNumberInput"
                        runat="server"
                        MaxLength="10" />
                </div>

                <div class="member-profile-field">
                    <label for="<%= EmergencyRelationshipInput.ClientID %>">
                        Emergency contact relationship
                    </label>
                    <asp:TextBox ID="EmergencyRelationshipInput"
                        runat="server"
                        MaxLength="50" />
                </div>
            </div>

            <div class="member-profile-actions">
                <asp:Button ID="SaveButton"
                    runat="server"
                    Text="Save details"
                    OnClientClick="return confirm('Are you sure you want to save these details?');"
                    OnClick="SaveButton_Click" />

                <asp:Button ID="CancelButton"
                    runat="server"
                    Text="Cancel"
                    CausesValidation="false"
                    OnClick="CancelButton_Click" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>