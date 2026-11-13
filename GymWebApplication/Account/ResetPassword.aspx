<%@ Page Title="Reset Password"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs"
    Inherits="GymWebApplication.Account.ResetPassword" %>

<asp:Content
    runat="server"
    ID="BodyContent"
    ContentPlaceHolderID="MainContent">

    <main aria-labelledby="title" class="container py-4">
        <div class="row justify-content-center">
            <div class="col-12 col-md-9 col-lg-7">
                <div class="card shadow-sm">
                    <div class="card-body p-4">
                        <h2 id="title" class="mb-2">Reset Password</h2>
                        <p class="text-muted mb-4">
                            Enter the six-digit code sent to your registered email address,
                            then choose a new password.
                        </p>

                        <asp:PlaceHolder ID="ErrorMessage" runat="server" Visible="false">
                            <div class="alert alert-danger" role="alert">
                                <asp:Literal ID="FailureText" runat="server" />
                            </div>
                        </asp:PlaceHolder>

                        <div class="mb-3">
                            <asp:Label ID="MemberLabel" runat="server"
                                AssociatedControlID="StudentNumberOrEmail"
                                CssClass="form-label">Student Number / Registered Email</asp:Label>
                            <asp:TextBox ID="StudentNumberOrEmail" runat="server"
                                CssClass="form-control"
                                MaxLength="100"
                                autocomplete="username"
                                placeholder="Enter your student number or email address" />
                            <asp:RequiredFieldValidator ID="MemberRequired" runat="server"
                                ControlToValidate="StudentNumberOrEmail"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="ResetPassword"
                                ErrorMessage="Enter your student number or registered email address." />
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="CodeLabel" runat="server"
                                AssociatedControlID="ResetCode"
                                CssClass="form-label">Six-Digit Reset Code</asp:Label>
                            <asp:TextBox ID="ResetCode" runat="server"
                                CssClass="form-control"
                                MaxLength="6"
                                TextMode="SingleLine"
                                inputmode="numeric"
                                autocomplete="one-time-code"
                                placeholder="e.g. 123456" />
                            <asp:RequiredFieldValidator ID="CodeRequired" runat="server"
                                ControlToValidate="ResetCode"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="ResetPassword"
                                ErrorMessage="Enter the reset code sent to your email." />
                        </div>

                        <div class="mb-3">
                            <asp:Label ID="PasswordLabel" runat="server"
                                AssociatedControlID="Password"
                                CssClass="form-label">New Password</asp:Label>
                            <asp:TextBox ID="Password" runat="server"
                                TextMode="Password"
                                CssClass="form-control"
                                MaxLength="100"
                                autocomplete="new-password"
                                placeholder="Enter a new password (at least 8 characters)" />
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                                ControlToValidate="Password"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="ResetPassword"
                                ErrorMessage="Enter a new password." />
                        </div>

                        <div class="mb-4">
                            <asp:Label ID="ConfirmPasswordLabel" runat="server"
                                AssociatedControlID="ConfirmPassword"
                                CssClass="form-label">Confirm New Password</asp:Label>
                            <asp:TextBox ID="ConfirmPassword" runat="server"
                                TextMode="Password"
                                CssClass="form-control"
                                MaxLength="100"
                                autocomplete="new-password"
                                placeholder="Re-enter your new password" />
                            <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server"
                                ControlToValidate="ConfirmPassword"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="ResetPassword"
                                ErrorMessage="Confirm your new password." />
                        </div>

                        <div class="d-grid gap-2">
                            <asp:Button ID="ResetButton" runat="server"
                                Text="Reset Password"
                                CssClass="btn btn-primary"
                                ValidationGroup="ResetPassword"
                                OnClick="Reset_Click" />
                            <a href="Forgot.aspx" class="btn btn-outline-secondary">Request a New Code</a>
                            <a href="Login.aspx" class="text-center">Back to Login</a>
                        </div>

                        <p class="text-muted small mt-3 mb-0">
                            Your reset code expires after 10 minutes and can only be used once.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
