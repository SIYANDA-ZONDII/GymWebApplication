<%@ Page Title="Manage Password"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ManagePassword.aspx.cs"
    Inherits="GymWebApplication.Account.ManagePassword" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <main class="account-page">

        <section class="account-section">

            <div class="account-header">

                <span class="section-label">
                    ACCOUNT SECURITY
                </span>

                <h1>
                    Change Your
                    <span>Password.</span>
                </h1>

                <p>
                    Update the password used to access your
                    UKZN Sport Gym Management System account.
                </p>

            </div>


            <div class="account-form">

                <!-- CURRENT PASSWORD -->

                <div class="form-group">

                    <asp:Label
                        runat="server"
                        AssociatedControlID="CurrentPassword"
                        CssClass="form-label">

                        CURRENT PASSWORD

                    </asp:Label>


                    <asp:TextBox
                        runat="server"
                        ID="CurrentPassword"
                        TextMode="Password"
                        CssClass="form-control" />


                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="CurrentPassword"
                        CssClass="text-danger"
                        ErrorMessage="Please enter your current password."
                        Display="Dynamic"
                        ValidationGroup="ChangePassword" />

                </div>


                <!-- NEW PASSWORD -->

                <div class="form-group">

                    <asp:Label
                        runat="server"
                        AssociatedControlID="NewPassword"
                        CssClass="form-label">

                        NEW PASSWORD

                    </asp:Label>


                    <asp:TextBox
                        runat="server"
                        ID="NewPassword"
                        TextMode="Password"
                        CssClass="form-control" />


                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="NewPassword"
                        CssClass="text-danger"
                        ErrorMessage="Please enter a new password."
                        Display="Dynamic"
                        ValidationGroup="ChangePassword" />

                </div>


                <!-- CONFIRM PASSWORD -->

                <div class="form-group">

                    <asp:Label
                        runat="server"
                        AssociatedControlID="ConfirmNewPassword"
                        CssClass="form-label">

                        CONFIRM NEW PASSWORD

                    </asp:Label>


                    <asp:TextBox
                        runat="server"
                        ID="ConfirmNewPassword"
                        TextMode="Password"
                        CssClass="form-control" />


                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="ConfirmNewPassword"
                        CssClass="text-danger"
                        ErrorMessage="Please confirm your new password."
                        Display="Dynamic"
                        ValidationGroup="ChangePassword" />


                    <asp:CompareValidator
                        runat="server"
                        ControlToCompare="NewPassword"
                        ControlToValidate="ConfirmNewPassword"
                        CssClass="text-danger"
                        ErrorMessage="The new passwords do not match."
                        Display="Dynamic"
                        ValidationGroup="ChangePassword" />

                </div>


                <!-- CHANGE PASSWORD -->

                <div class="form-actions">

                    <asp:Button
                        runat="server"
                        ID="ChangePasswordButton"
                        Text="CHANGE PASSWORD →"
                        CssClass="btn btn-outline-dark"
                        ValidationGroup="ChangePassword"
                        OnClick="ChangePassword_Click" />

                </div>


                <!-- BACK -->

                <div class="account-back">

                    <a href="~/Account/Manage"
                       runat="server">

                        ← BACK TO ACCOUNT

                    </a>

                </div>

            </div>

        </section>

    </main>

</asp:Content>