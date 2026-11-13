(function () {
    "use strict";

    function initialiseParticles() {
        const canvas =
            document.getElementById(
                "particles-canvas"
            );

        if (!canvas) {
            return;
        }

        const context =
            canvas.getContext("2d");

        if (!context) {
            return;
        }

        let particles = [];
        let animationFrameId;

        function resizeCanvas() {
            const devicePixelRatio =
                window.devicePixelRatio || 1;

            const width =
                canvas.clientWidth;

            const height =
                canvas.clientHeight;

            canvas.width =
                Math.floor(
                    width * devicePixelRatio
                );

            canvas.height =
                Math.floor(
                    height * devicePixelRatio
                );

            context.setTransform(
                devicePixelRatio,
                0,
                0,
                devicePixelRatio,
                0,
                0
            );
        }

        function createParticles() {
            particles = [];

            const totalParticles =
                window.innerWidth < 760
                    ? 25
                    : 52;

            for (
                let index = 0;
                index < totalParticles;
                index++
            ) {
                particles.push({
                    x:
                        Math.random()
                        * canvas.clientWidth,

                    y:
                        Math.random()
                        * canvas.clientHeight,

                    radius:
                        Math.random()
                        * 1.4 + 0.4,

                    velocityX:
                        (Math.random() - 0.5)
                        * 0.25,

                    velocityY:
                        (Math.random() - 0.5)
                        * 0.25,

                    opacity:
                        Math.random()
                        * 0.42 + 0.12
                });
            }
        }

        function updateParticle(
            particle) {
            particle.x +=
                particle.velocityX;

            particle.y +=
                particle.velocityY;

            if (particle.x < 0) {
                particle.x =
                    canvas.clientWidth;
            }

            if (
                particle.x >
                canvas.clientWidth
            ) {
                particle.x = 0;
            }

            if (particle.y < 0) {
                particle.y =
                    canvas.clientHeight;
            }

            if (
                particle.y >
                canvas.clientHeight
            ) {
                particle.y = 0;
            }
        }

        function drawParticle(
            particle) {
            context.beginPath();

            context.arc(
                particle.x,
                particle.y,
                particle.radius,
                0,
                Math.PI * 2
            );

            context.fillStyle =
                "rgba(212, 137, 26, "
                + particle.opacity
                + ")";

            context.fill();
        }

        function drawConnections() {
            for (
                let first = 0;
                first < particles.length;
                first++
            ) {
                for (
                    let second = first + 1;
                    second < particles.length;
                    second++
                ) {
                    const differenceX =
                        particles[first].x
                        - particles[second].x;

                    const differenceY =
                        particles[first].y
                        - particles[second].y;

                    const distance =
                        Math.sqrt(
                            differenceX
                            * differenceX
                            +
                            differenceY
                            * differenceY
                        );

                    if (distance < 85) {
                        const opacity =
                            (1 - distance / 85)
                            * 0.09;

                        context.beginPath();

                        context.moveTo(
                            particles[first].x,
                            particles[first].y
                        );

                        context.lineTo(
                            particles[second].x,
                            particles[second].y
                        );

                        context.strokeStyle =
                            "rgba(212, 137, 26, "
                            + opacity
                            + ")";

                        context.lineWidth = 0.7;
                        context.stroke();
                    }
                }
            }
        }

        function animateParticles() {
            context.clearRect(
                0,
                0,
                canvas.clientWidth,
                canvas.clientHeight
            );

            particles.forEach(
                function (particle) {
                    updateParticle(particle);
                    drawParticle(particle);
                }
            );

            drawConnections();

            animationFrameId =
                window.requestAnimationFrame(
                    animateParticles
                );
        }

        function resetParticles() {
            resizeCanvas();
            createParticles();
        }

        window.addEventListener(
            "resize",
            resetParticles
        );

        window.addEventListener(
            "beforeunload",
            function () {
                if (animationFrameId) {
                    window.cancelAnimationFrame(
                        animationFrameId
                    );
                }
            }
        );

        resetParticles();
        animateParticles();
    }

    function initialisePasswordToggle() {
        const password =
            document.querySelector(
                '[id$="Password"]'
            );

        const toggle =
            document.getElementById(
                "togglePw"
            );

        const eyeOpen =
            document.getElementById(
                "eyeOpen"
            );

        const eyeClosed =
            document.getElementById(
                "eyeClosed"
            );

        if (!password || !toggle) {
            return;
        }

        toggle.addEventListener(
            "click",
            function () {
                const passwordVisible =
                    password.type === "text";

                password.type =
                    passwordVisible
                        ? "password"
                        : "text";

                toggle.setAttribute(
                    "aria-label",
                    passwordVisible
                        ? "Show password"
                        : "Hide password"
                );

                toggle.setAttribute(
                    "aria-pressed",
                    passwordVisible
                        ? "false"
                        : "true"
                );

                if (eyeOpen) {
                    eyeOpen.classList.toggle(
                        "is-hidden",
                        !passwordVisible
                    );
                }

                if (eyeClosed) {
                    eyeClosed.classList.toggle(
                        "is-hidden",
                        passwordVisible
                    );
                }

                password.focus();
            }
        );
    }

    function initialiseTicker() {
        const ticker =
            document.getElementById(
                "ticker"
            );

        if (!ticker) {
            return;
        }

        const messages = [
            "Discipline is choosing between what you want now and what you want most.",
            "Every workout is another step towards a stronger version of yourself.",
            "Success starts with showing up.",
            "Strength grows through consistency.",
            "Your only competition is who you were yesterday."
        ];

        let currentMessage = 0;

        function displayMessage() {
            ticker.style.opacity = "0";

            window.setTimeout(
                function () {
                    ticker.textContent =
                        messages[currentMessage];

                    ticker.style.opacity = "1";

                    currentMessage =
                        (currentMessage + 1)
                        % messages.length;
                },
                250
            );
        }

        displayMessage();

        window.setInterval(
            displayMessage,
            5000
        );
    }

    function initialiseLoginPage() {
        initialiseParticles();
        initialisePasswordToggle();
        initialiseTicker();
    }

    if (
        document.readyState ===
        "loading"
    ) {
        document.addEventListener(
            "DOMContentLoaded",
            initialiseLoginPage
        );
    }
    else {
        initialiseLoginPage();
    }
})();