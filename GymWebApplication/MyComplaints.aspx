<%@ Page Title="My Complaints"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="MyComplaints.aspx.cs"
    Inherits="GymWebApplication.MyComplaints" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .my-complaints-page {
            min-height: 100vh;
            padding: 45px 20px 80px;
            background: #f3f5f7;
            font-family: Arial, sans-serif;
            color: #142235;
        }

        .my-complaints-container {
            max-width: 1000px;
            margin: 0 auto;
        }

        .my-complaints-container h1 {
            margin-bottom: 8px;
        }

        .my-complaints-intro {
            margin-bottom: 28px;
            color: #667587;
        }

        .complaint-card {
            padding: 24px;
            margin-bottom: 18px;
            border-radius: 12px;
            background: #fff;
            box-shadow: 0 7px 25px rgba(20, 34, 53, .07);
        }

        .complaint-card h2 {
            margin: 0 0 12px;
            font-size: 20px;
        }

        .complaint-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 12px 22px;
            margin-bottom: 16px;
            color: #596879;
            font-size: 13px;
        }

        .complaint-description,
        .complaint-response {
            white-space: pre-wrap;
            overflow-wrap: anywhere;
            line-height: 1.6;
        }

        .complaint-response {
            padding: 14px;
            margin-top: 16px;
            border-radius: 7px;
            background: #f5f7fa;
        }

        .complaint-status {
            display: inline-block;
            padding: 5px 9px;
            border-radius: 20px;
            background: #fff1d9;
            color: #805100;
            font-weight: 700;
        }

        .complaint-empty,
        .complaint-error {
            padding: 20px;
            border-radius: 10px;
            background: #fff;
        }

        .complaint-error {
            color: #b42318;
        }

        .submit-link {
            display: inline-block;
            margin-bottom: 24px;
            color: #a46200;
            font-weight: 700;
        }
    </style>

    <div class="my-complaints-page">
        <div class="my-complaints-container">
            <h1>My Complaints</h1>

            <p class="my-complaints-intro">
                View the progress of complaints submitted using your account.
            </p>

            <a class="submit-link"
                href="<%= ResolveUrl("~/Complaint.aspx") %>">
                Submit another complaint
            </a>

            <asp:Label ID="lblError"
                runat="server"
                Visible="false"
                CssClass="complaint-error" />

            <asp:Panel ID="pnlEmpty"
                runat="server"
                Visible="false"
                CssClass="complaint-empty">
                You have no complaints linked to your account.
            </asp:Panel>

            <asp:Repeater ID="rptComplaints" runat="server">
                <ItemTemplate>
                    <article class="complaint-card">
                        <h2>Complaint #<%#: Eval("Complaint_ID") %></h2>

                        <div class="complaint-meta">
                            <span>
                                <strong>Category:</strong>
                                <%#: Eval("Category") %>
                            </span>

                            <span>
                                <strong>Location:</strong>
                                <%#: Eval("Location") %>
                            </span>

                            <span>
                                <strong>Submitted:</strong>
                                <%#: Eval("Submitted_At", "{0:yyyy-MM-dd HH:mm}") %>
                            </span>

                            <span class="complaint-status">
                                <%#: Eval("Status") %>
                            </span>
                        </div>

                        <strong>Your complaint</strong>
                        <div class="complaint-description"><%#: Eval("Description") %></div>

                        <div class="complaint-response">
                            <strong>Manager response</strong>
                            <div><%#: Eval("Display_Response") %></div>

                            <small>
                                Last updated:
                                <%#: Eval("Display_Updated_At") %>
                            </small>
                        </div>
                    </article>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>