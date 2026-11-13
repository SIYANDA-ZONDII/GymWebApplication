
<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Forgot.aspx.cs"
    Inherits="GymWebApplication.Account.ForgotPassword" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <main aria-labelledby="title" class="container py-4">
        <div class="row justify-content-center">
            <div class="col-12 col-md-8 col-lg-6">
                <div class="card shadow-sm">
                    <div class="card-body p-4">
                        <h2 id="title">Forgot Password</h2>
                        <p class="text-muted">
                            Enter your student number, staff ID, or registered email address.
                        </p>

                        <asp:PlaceHolder ID="ErrorMessage" runat="server" Visible="false">
                            <div class="alert alert-danger" role="alert">
                                <asp:Literal ID="FailureText" runat="server" />
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="ResetForm" runat="server" Visible="true">
                            <div class="mb-3">
                                <asp:Label ID="StudentNumberLabel" runat="server"
                                    AssociatedControlID="StudentNumberOrEmail"
                                    CssClass="form-label">
                                    Student Number / Staff ID / Email
                                </asp:Label>

                                <asp:TextBox ID="StudentNumberOrEmail" runat="server"
                                    CssClass="form-control" MaxLength="100"
                                    autocomplete="username"
                                    placeholder="Enter your ID or registered email" />

                                <asp:RequiredFieldValidator ID="MemberRequired" runat="server"
                                    ControlToValidate="StudentNumberOrEmail"
                                    CssClass="text-danger" Display="Dynamic"
                                    ValidationGroup="ForgotPassword"
                                    ErrorMessage="Please enter your student number, staff ID, or registered email address." />
                            </div>

                            <div class="d-grid gap-2">
                                <asp:Button ID="SendCodeButton" runat="server"
                                    Text="Send Reset Code" CssClass="btn btn-primary"
                                    ValidationGroup="ForgotPassword"
                                    OnClick="SendCode_Click" />

                                <a href="Login.aspx" class="btn btn-outline-secondary">
                                    Back to Login
                                </a>
                            </div>
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="DisplayEmail" runat="server" Visible="false">
                            <div class="alert alert-success" role="status">
                                <h5>Verification code sent</h5>
                                <p>A password reset code has been sent to:</p>
                                <p>
                                    <strong><asp:Literal ID="MaskedEmail" runat="server" /></strong>
                                </p>
                                <p class="mb-0">
                                    Your six-digit code expires in <strong>10 minutes</strong>.
                                </p>
                            </div>

                            <a href="ResetPassword.aspx" class="btn btn-primary">
                                Enter Reset Code
                            </a>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
        </div>
    </main>
</asp:Content>
