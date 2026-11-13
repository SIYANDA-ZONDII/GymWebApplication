<%@ Page Title="UKZN Sport Gym"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="GymWebApplication._Default" %>

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
            letter-spacing: 1px;
        }

        .navbar .nav-link {
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .landing-page {
            background: #101217;
            color: #fff;
            font-family: Arial, Helvetica, sans-serif;
        }

        .landing-hero {
            position: relative;
            display: flex;
            min-height: 590px;
            align-items: center;
            justify-content: center;
            padding: 90px 24px;
            overflow: hidden;
            text-align: center;
            background-image:
                linear-gradient(rgba(7, 9, 14, .67), rgba(7, 9, 14, .75)),
                url('<%= ResolveUrl("~/Content/240_F_605112092_4t1xO4BdP61KnV5RsSYMd3YuRKDmglN5.jpg") %>');
            background-position: center;
            background-size: cover;
        }

        .landing-hero::after {
            position: absolute;
            right: 0;
            bottom: 0;
            left: 0;
            height: 5px;
            background: #f26532;
            content: "";
        }

        .landing-hero-content {
            position: relative;
            z-index: 1;
            max-width: 850px;
        }

        .landing-eyebrow {
            display: inline-block;
            margin-bottom: 20px;
            color: #ff8050;
            font-size: 13px;
            font-weight: 800;
            letter-spacing: 3px;
        }

        .landing-hero h1 {
            margin: 0;
            font-size: clamp(43px, 6vw, 78px);
            font-weight: 900;
            line-height: 1.1;
            letter-spacing: 1px;
            text-transform: uppercase;
        }

        .landing-hero h1 span {
            color: #f26532;
        }

        .landing-hero p {
            max-width: 660px;
            margin: 22px auto 0;
            color: #e5e7ea;
            font-size: 17px;
            line-height: 1.7;
        }

        .landing-actions {
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
            gap: 12px;
            margin-top: 33px;
        }

        .landing-button {
            display: inline-block;
            min-width: 150px;
            padding: 13px 23px;
            border: 1px solid #f26532;
            border-radius: 3px;
            background: #f26532;
            color: #fff;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: .6px;
            text-align: center;
            text-decoration: none;
            transition: background .2s, transform .2s;
        }

        .landing-button:hover {
            background: #d84d20;
            color: #fff;
            transform: translateY(-2px);
        }

        .landing-button-outline {
            border-color: rgba(255, 255, 255, .8);
            background: transparent;
        }

        .landing-button-outline:hover {
            border-color: #f26532;
            background: #f26532;
        }

        .landing-intro {
            max-width: 1100px;
            margin: 0 auto;
            padding: 75px 24px;
            text-align: center;
        }

        .landing-section-label {
            color: #f26532;
            font-size: 12px;
            font-weight: 800;
            letter-spacing: 2px;
            text-transform: uppercase;
        }

        .landing-intro h2 {
            margin: 13px 0 15px;
            font-size: clamp(30px, 4vw, 45px);
            font-weight: 850;
        }

        .landing-intro > p {
            max-width: 690px;
            margin: 0 auto;
            color: #bfc4cd;
            line-height: 1.8;
        }

        .landing-cards {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 18px;
            margin-top: 42px;
            text-align: left;
        }

        .landing-card {
            padding: 28px;
            border-top: 3px solid #f26532;
            background: #1b1e25;
        }

        .landing-card-number {
            color: #f26532;
            font-size: 13px;
            font-weight: 800;
        }

        .landing-card h3 {
            margin: 18px 0 10px;
            font-size: 21px;
        }

        .landing-card p {
            margin: 0;
            color: #bfc4cd;
            line-height: 1.65;
        }

        @media (max-width: 760px) {
            .landing-hero {
                min-height: 540px;
                padding: 70px 20px;
            }

            .landing-hero p {
                font-size: 15px;
            }

            .landing-cards {
                grid-template-columns: 1fr;
            }
        }
    </style>

    <main class="landing-page">
        <section class="landing-hero">
            <div class="landing-hero-content">
                <span class="landing-eyebrow">
                    UKZN SPORT · WESTVILLE CAMPUS
                </span>

                <h1>
                    TRAIN STRONG.<br />
                    <span>LIVE ACTIVE.</span>
                </h1>

                <p>
                    A place to build strength, stay active and work towards
                    your fitness goals. Welcome to UKZN Sport Gym.
                </p>

                <div class="landing-actions">
                    <a class="landing-button"
                        href="<%= ResolveUrl("~/Account/Login.aspx") %>">
                        MEMBER LOGIN
                    </a>

                    <a class="landing-button landing-button-outline"
                        href="#about-gym">
                        EXPLORE THE GYM
                    </a>
                </div>
            </div>
        </section>

        <section class="landing-intro" id="about-gym">
            <span class="landing-section-label">Welcome to UKZN Sport</span>

            <h2>Find Your Strength Here</h2>

            <p>
                Whether you are building a routine, improving your fitness
                or training with others, our gym supports your progress.
            </p>

            <div class="landing-cards">
                <article class="landing-card">
                    <span class="landing-card-number">01</span>
                    <h3>Train</h3>
                    <p>Make exercise part of your routine in a dedicated training space.</p>
                </article>

                <article class="landing-card">
                    <span class="landing-card-number">02</span>
                    <h3>Book</h3>
                    <p>Sign in to explore and manage your training session bookings.</p>
                </article>

                <article class="landing-card">
                    <span class="landing-card-number">03</span>
                    <h3>Progress</h3>
                    <p>Keep working towards your goals with the UKZN Sport community.</p>
                </article>
            </div>
        </section>
    </main>
</asp:Content>