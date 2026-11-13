<%@ Page Title="Contact"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs"
    Inherits="GymWebApplication.Contact" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="contact-page">

        <!-- =========================================
             HERO
             ========================================= -->

        <section class="contact-hero">

            <div class="contact-hero-content">

                <div class="contact-badge">
                    <span></span>
                    UKZN SPORT · WESTVILLE CAMPUS
                </div>

                <h1>
                    CONTACT
                    <span>US</span>
                </h1>

                <p>
                    Need assistance with your gym membership, account,
                    activities or payments? Get in touch with the
                    UKZN Sport Gym Management team.
                </p>

            </div>

            <div class="contact-hero-number">
                01
            </div>

        </section>


        <!-- =========================================
             CONTACT INFORMATION
             ========================================= -->

        <section class="contact-section">

            <div class="contact-section-heading">

                <span class="contact-section-label">
                    GET IN TOUCH
                </span>

                <h2>
                    We're Here To
                    <span>Help</span>
                </h2>

                <p>
                    Contact the appropriate department using the
                    information below.
                </p>

            </div>


            <div class="contact-grid">


                <!-- GENERAL SUPPORT -->

                <div class="contact-card">

                    <div class="contact-card-number">
                        01
                    </div>

                    <div class="contact-icon">
                        ✉
                    </div>

                    <h3>
                        General Support
                    </h3>

                    <p>
                        For general questions about the gym management
                        system, memberships and account access.
                    </p>

                    <div class="contact-detail">

                        <span>
                            EMAIL
                        </span>

                        <a href="mailto:support@ukzn.ac.za">
                            support@ukzn.ac.za
                        </a>

                    </div>

                </div>


                <!-- GYM MANAGEMENT -->

                <div class="contact-card">

                    <div class="contact-card-number">
                        02
                    </div>

                    <div class="contact-icon">
                        ◉
                    </div>

                    <h3>
                        Gym Management
                    </h3>

                    <p>
                        Contact gym management for membership,
                        facility and operational enquiries.
                    </p>

                    <div class="contact-detail">

                        <span>
                            EMAIL
                        </span>

                        <a href="mailto:gymmanagement@ukzn.ac.za">
                            gymmanagement@ukzn.ac.za
                        </a>

                    </div>

                </div>


                <!-- PHONE -->

                <div class="contact-card">

                    <div class="contact-card-number">
                        03
                    </div>

                    <div class="contact-icon">
                        ☎
                    </div>

                    <h3>
                        Telephone
                    </h3>

                    <p>
                        Speak directly with the gym team during
                        normal operating hours.
                    </p>

                    <div class="contact-detail">

                        <span>
                            PHONE
                        </span>

                        <a href="tel:+27312600000">
                            +27 31 260 0000
                        </a>

                    </div>

                </div>


            </div>

        </section>


        <!-- =========================================
             LOCATION
             ========================================= -->

        <section class="contact-location">

            <div class="location-number">
                04
            </div>

            <div class="location-content">

                <span class="contact-section-label">
                    OUR LOCATION
                </span>

                <h2>
                    UKZN Sport
                    <span>Westville Campus</span>
                </h2>

                <p>
                    Visit the UKZN Sport facilities at the Westville
                    Campus for assistance with your gym membership
                    and related services.
                </p>

                <div class="location-details">

                    <div>
                        <span>LOCATION</span>
                        <strong>Westville Campus</strong>
                    </div>

                    <div>
                        <span>CAMPUS</span>
                        <strong>University of KwaZulu-Natal</strong>
                    </div>

                    <div>
                        <span>DEPARTMENT</span>
                        <strong>UKZN Sport</strong>
                    </div>

                </div>

            </div>

        </section>


        <!-- =========================================
             MEMBER SUPPORT
             ========================================= -->

        <section class="contact-support">

            <div>

                <span class="contact-section-label">
                    MEMBER SUPPORT
                </span>

                <h2>
                    Already a
                    <span>Member?</span>
                </h2>

                <p>
                    Sign in to access your member dashboard and manage
                    your profile, membership, activities and payments.
                </p>

            </div>

            <a href="~/Account/Login.aspx"
               runat="server"
               class="contact-login-button">

                MEMBER LOGIN
                <span>→</span>

            </a>

        </section>


      
</div>
</asp:Content>