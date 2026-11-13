<%@ Page Title="Login"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="GymWebApplication.Account.Login" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .body-content {
            max-width: none !important;
            padding: 0 !important;
            margin: 0 !important;
        }

        .navbar.bg-dark {
            background: #111318 !important;
            border-bottom: 2px solid #f26532;
        }

        .navbar-brand {
            color: #f26532 !important;
            font-weight: 900;
        }

        .login-page {
            display: grid;
            grid-template-columns: 1.05fr .95fr;
            min-height: calc(100vh - 58px);
            background: #111318;
            font-family: Arial, Helvetica, sans-serif;
        }

        .login-photo {
            position: relative;
            display: flex;
            min-height: 620px;
            align-items: flex-end;
            padding: 65px;
            background-image:
                linear-gradient(to top, rgba(7, 9, 14, .94), rgba(7, 9, 14, .13) 72%),
                url('<%= ResolveUrl("~/Content/360_F_303278340_KmeegCDEhkLAgTwJah69wQ2nXjHwmFnw.jpg") %>');
            background-position: center;
            background-size: cover;
            color: #fff;
        }

        .login-photo-content {
            position: relative;
            max-width: 520px;
        }

        .login-photo-content span {
            color: #ff8050;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 2px;
        }

        .login-photo-content h1 {
            margin: 13px 0;
            font-size: clamp(40px, 5vw, 68px);
            font-weight: 900;
            line-height: 1.05;
            text-transform: uppercase;
        }

        .login-photo-content h1 em {
            color: #f26532;
            font-style: normal;
        }

        .login-photo-content p {
            color: #e1e4e8;
            line-height: 1.7;
        }

        .login-form-side {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 60px 30px;
            background: #f5f6f8;
        }

        .login-card {
            width: 100%;
            max-width: 420px;
        }

        .login-card-label {
            color: #e95827;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 2px;
            text-transform: uppercase;
        }

        .login-card h2 {
            margin: 12px 0 8px;
            color: #131923;
            font-size: 37px;
            font-weight: 850;
        }

        .login-card-intro {
            margin-bottom: 30px;
            color: #647081;
        }

        .login-field {
            margin-bottom: 19px;
        }

        .login-field label {
            display: block;
            margin-bottom: 8px;
            color: #202936;
            font-size: 13px;
            font-weight: 700;
        }

        .login-input {
            display: block;
            width: 100%;
            box-sizing: border-box;
            padding: 13px 14px;
            border: 1px solid #ccd2db;
            border-radius: 5px;
            background: #fff;
            color: #17202b;
            font: inherit;
        }

        .login-input:focus {
            outline: none;
            border-color: #f26532;
            box-shadow: 0 0 0 3px rgba(242, 101, 50, .12);
        }

        .password-wrap {
            position: relative;
        }

        .password-wrap .login-input {
            padding-right: 73px;
        }

        .password-toggle {
            position: absolute;
            top: 50%;
            right: 8px;
            padding: 7px;
            border: 0;
            background: transparent;
            color: #a94b27;
            font-size: 12px;
            font-weight: 800;
            cursor: pointer;
            transform: translateY(-50%);
        }

        .login-validation {
            display: block;
            margin-top: 6px;
            color: #b42318;
            font-size: 12px;
        }

        .login-error {
            padding: 13px;
            margin-bottom: 20px;
            border-radius: 5px;
            background: #fff0ee;
            color: #b42318;
        }

        .login-options {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 10px;
            margin: 6px 0 25px;
            font-size: 13px;
        }

        .login-options a,
        .login-register a {
            color: #c44d21;
            font-weight: 700;
            text-decoration: none;
        }

        .login-submit {
            width: 100%;
            padding: 14px;
            border: 0;
            border-radius: 5px;
            background: #f26532;
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            letter-spacing: .5px;
            cursor: pointer;
        }

        .login-submit:hover {
            background: #d95125;
        }

        .login-register {
            margin-top: 21px;
            color: #647081;
            font-size: 13px;
            text-align: center;
        }

        @media (max-width: 850px) {
            .login-page {
                grid-template-columns: 1fr;
            }

            .login-photo {
                min-height: 330px;
                padding: 40px 25px;
            }

            .login-form-side {
                padding: 45px 22px;
            }
        }
    </style>

    <div class="login-page">
        <section class="login-photo">
            <div class="login-photo-content">
                <span>UKZN SPORT · WESTVILLE CAMPUS</span>
                <h1>YOUR FITNESS<br /><em>STARTS HERE.</em></h1>
                <p>Sign in to manage your membership, bookings and training.</p>
            </div>
        </section>

        <section class="login-form-side">
            <div class="login-card">
                <span class="login-card-label">Welcome back</span>
                <h2>Login</h2>
                <p class="login-card-intro">
                    Use your account to continue.
                </p>

                <asp:PlaceHolder ID="ErrorMessage"
                    runat="server"
                    Visible="false">
                    <div class="login-error">
                        <asp:Literal ID="FailureText" runat="server" />
                    </div>
                </asp:PlaceHolder>

                <div class="login-field">
                    <label for="<%= Email.ClientID %>">Email address</label>

                    <asp:TextBox ID="Email"
                        runat="server"
                        TextMode="Email"
                        CssClass="login-input"
                        placeholder="Enter your email"
                        autocomplete="email" />

                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="Email"
                        CssClass="login-validation"
                        Display="Dynamic"
                        ErrorMessage="Email address is required." />

                    <asp:RegularExpressionValidator
                        runat="server"
                        ControlToValidate="Email"
                        CssClass="login-validation"
                        Display="Dynamic"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        ErrorMessage="Please enter a valid email address." />
                </div>

                <div class="login-field">
                    <label for="<%= Password.ClientID %>">Password</label>

                    <div class="password-wrap">
                        <asp:TextBox ID="Password"
                            runat="server"
                            TextMode="Password"
                            CssClass="login-input"
                            placeholder="Enter your password"
                            autocomplete="current-password" />

                        <button id="passwordToggle"
                            type="button"
                            class="password-toggle"
                            aria-label="Show password"
                            aria-pressed="false">
                            SHOW
                        </button>
                    </div>

                    <asp:RequiredFieldValidator
                        runat="server"
                        ControlToValidate="Password"
                        CssClass="login-validation"
                        Display="Dynamic"
                        ErrorMessage="Password is required." />
                </div>

                <div class="login-options">
                    <span>
                        <asp:CheckBox ID="RememberMe"
                            runat="server"
                            Text="Remember me" />
                    </span>

                    <asp:HyperLink ID="ForgotPasswordHyperLink"
                        runat="server"
                        NavigateUrl="~/Account/Forgot.aspx">
                        Forgot password?
                    </asp:HyperLink>
                </div>

                <asp:Button ID="LoginButton"
                    runat="server"
                    Text="SIGN IN"
                    CssClass="login-submit"
                    OnClick="LogIn"
                    UseSubmitBehavior="false" />

                <p class="login-register">
                    New member?
                    <a href="<%= ResolveUrl("~/Account/Register.aspx") %>">
                        Create an account
                    </a>
                </p>
            </div>
        </section>
    </div>

    <script>
        (function () {
            var button = document.getElementById("passwordToggle");
            var password = document.getElementById("<%= Password.ClientID %>");

            if (!button || !password) return;

            button.addEventListener("click", function () {
                var showing = password.type === "password";
                password.type = showing ? "text" : "password";
                button.textContent = showing ? "HIDE" : "SHOW";
                button.setAttribute("aria-pressed", showing ? "true" : "false");
                button.setAttribute(
                    "aria-label",
                    showing ? "Hide password" : "Show password"
                );
            });
        })();
    </script>
</asp:Content>