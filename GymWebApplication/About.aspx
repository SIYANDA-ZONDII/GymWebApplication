<%@ Page Title="About"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="About.aspx.cs"
    Inherits="GymWebApplication.About" %>

<asp:Content ID="BodyContent"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="about-page">

        <!-- HERO -->
        <section class="about-hero">

            <div class="about-hero-content">

                <div class="about-badge">
                    <span></span>
                    UKZN SPORT · WESTVILLE CAMPUS
                </div>

                <h1>
                    ABOUT
                    <span>UKZN SPORT</span>
                </h1>

                <p>
                    A dedicated sporting and fitness environment where
                    students, staff and members can train, stay active
                    and work towards their fitness goals.
                </p>

            </div>

            <div class="about-hero-number">
                01
            </div>

        </section>


        <!-- ABOUT THE GYM -->
        <section class="about-section">

            <div class="about-section-number">
                01
            </div>

            <div class="about-section-content">

                <span class="about-section-label">
                    ABOUT THE GYM
                </span>

                <h2>
                    Train. Move.
                    <span>Perform.</span>
                </h2>

                <p>
                    UKZN Sport provides a space for members to stay
                    active, improve their fitness and maintain a
                    healthy lifestyle.
                </p>

                <p>
                    Located at the Westville Campus, the gym provides
                    an environment where individuals can train at
                    their own pace while working towards their
                    personal fitness goals.
                </p>

            </div>

        </section>


        <!-- OUR APPROACH -->
        <section class="about-features">

            <div class="about-heading">

                <span class="about-section-label">
                    OUR APPROACH
                </span>

                <h2>
                    More Than
                    <span>A Workout</span>
                </h2>

                <p>
                    The gym is built around an active lifestyle,
                    consistency and creating a positive training
                    environment.
                </p>

            </div>


            <div class="about-feature-grid">

                <!-- FITNESS -->

                <div class="about-feature-card">

                    <div class="feature-number">
                        01
                    </div>

                    <div class="feature-icon">
                        +
                    </div>

                    <h3>
                        Fitness
                    </h3>

                    <p>
                        A place to improve strength, endurance,
                        mobility and overall physical fitness.
                    </p>

                </div>


                <!-- TRAINING -->

                <div class="about-feature-card">

                    <div class="feature-number">
                        02
                    </div>

                    <div class="feature-icon">
                        ↗
                    </div>

                    <h3>
                        Training
                    </h3>

                    <p>
                        Work towards your goals through regular
                        training and consistent exercise.
                    </p>

                </div>


                <!-- COMMUNITY -->

                <div class="about-feature-card">

                    <div class="feature-number">
                        03
                    </div>

                    <div class="feature-icon">
                        ◉
                    </div>

                    <h3>
                        Community
                    </h3>

                    <p>
                        Train alongside other members and be part
                        of an active sporting environment.
                    </p>

                </div>


                <!-- WELLNESS -->

                <div class="about-feature-card">

                    <div class="feature-number">
                        04
                    </div>

                    <div class="feature-icon">
                        ♥
                    </div>

                    <h3>
                        Wellness
                    </h3>

                    <p>
                        Support a healthier lifestyle through
                        regular physical activity and exercise.
                    </p>

                </div>

            </div>

        </section>


        <!-- FACILITIES -->
        <section class="about-roles">

            <div class="about-heading">

                <span class="about-section-label">
                    GYM EXPERIENCE
                </span>

                <h2>
                    Built For
                    <span>Training</span>
                </h2>

                <p>
                    Whether you are starting your fitness journey or
                    maintaining an established routine, the gym offers
                    a space for regular training.
                </p>

            </div>


            <div class="role-grid">

                <div class="role-card">

                    <span class="role-number">
                        01
                    </span>

                    <h3>
                        Strength Training
                    </h3>

                    <p>
                        Focus on building strength and developing
                        a consistent training routine.
                    </p>

                </div>


                <div class="role-card">

                    <span class="role-number">
                        02
                    </span>

                    <h3>
                        Cardio
                    </h3>

                    <p>
                        Improve cardiovascular fitness and
                        endurance through regular exercise.
                    </p>

                </div>


                <div class="role-card">

                    <span class="role-number">
                        03
                    </span>

                    <h3>
                        Personal Training
                    </h3>

                    <p>
                        Members can work towards their goals with
                        structured training and professional guidance.
                    </p>

                </div>


                <div class="role-card">

                    <span class="role-number">
                        04
                    </span>

                    <h3>
                        Active Lifestyle
                    </h3>

                    <p>
                        Make exercise part of your routine and
                        maintain an active lifestyle.
                    </p>

                </div>

            </div>

        </section>


        <!-- WHY TRAIN -->
        <section class="about-purpose">

            <div class="purpose-number">
                05
            </div>

            <div class="purpose-content">

                <span class="about-section-label">
                    WHY TRAIN WITH US
                </span>

                <h2>
                    Find Your
                    <span>Strength.</span>
                </h2>

                <p>
                    Training is about more than simply exercising.
                    It is about building discipline, developing
                    healthy habits and becoming more confident in
                    your abilities.
                </p>

                <p>
                    Whether your goal is to become stronger, improve
                    your fitness or simply stay active, every workout
                    is a step towards a healthier lifestyle.
                </p>

            </div>

        </section>


        <!-- QUOTE -->
        <section class="about-quote">

            <div class="quote-mark">
                "
            </div>

            <p>
                Train hard, recover harder.
                Stay consistent and keep moving forward.
            </p>

            <span>
                — UKZN SPORT
            </span>

        </section>


        <!-- CTA -->
        <section class="about-cta">

            <div>

                <span class="about-section-label">
                    UKZN SPORT · WESTVILLE CAMPUS
                </span>

                <h2>
                    Ready To
                    <span>Get Moving?</span>
                </h2>

                <p>
                    Sign in to your account and access your
                    personalised gym area.
                </p>

            </div>

            <a href="~/Account/Login.aspx"
               runat="server"
               class="about-cta-button">

                SIGN IN
                <span>→</span>

            </a>

        </section>

        </div>
</asp:Content>