/* =========================================================
   UKZN SPORT
   MEMBER REGISTRATION JAVASCRIPT
   ========================================================= */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        setupMemberID();

        setupPostalCode();

        setupPhoneNumber(
            '[id$="PhoneNumber"]'
        );

        setupPhoneNumber(
            '[id$="TelephoneNumber"]'
        );

        setupPhoneNumber(
            '[id$="EmergencyContactNumber"]'
        );

        setupDateOfBirth();

        setupTextFields();

        setupClearButton();

    }
);


/* =========================================================
   MEMBER ID
   ========================================================= */

function setupMemberID() {

    const field =
        document.querySelector(
            '[id$="MemberID"]'
        );


    if (!field) {
        return;
    }


    field.addEventListener(
        "input",
        function () {

            // Only numbers

            this.value =
                this.value.replace(
                    /[^0-9]/g,
                    ""
                );


            // Maximum 9 digits

            if (
                this.value.length > 9
            ) {

                this.value =
                    this.value.substring(
                        0,
                        9
                    );
            }

        }
    );

}


/* =========================================================
   POSTAL CODE
   ========================================================= */

function setupPostalCode() {

    const field =
        document.querySelector(
            '[id$="PostalCode"]'
        );


    if (!field) {
        return;
    }


    field.addEventListener(
        "input",
        function () {

            this.value =
                this.value.replace(
                    /[^0-9]/g,
                    ""
                );


            if (
                this.value.length > 4
            ) {

                this.value =
                    this.value.substring(
                        0,
                        4
                    );
            }

        }
    );

}


/* =========================================================
   PHONE NUMBERS
   ========================================================= */

function setupPhoneNumber(selector) {

    const field =
        document.querySelector(selector);


    if (!field) {
        return;
    }


    field.addEventListener(
        "input",
        function () {

            /*
             * Allow:
             *
             * 0821234567
             * +27821234567
             * 082 123 4567
             * 082-123-4567
             */

            this.value =
                this.value.replace(
                    /[^0-9+\s-]/g,
                    ""
                );


            if (
                this.value.length > 15
            ) {

                this.value =
                    this.value.substring(
                        0,
                        15
                    );
            }

        }
    );

}


/* =========================================================
   DATE OF BIRTH
   ========================================================= */

function setupDateOfBirth() {

    const field =
        document.querySelector(
            '[id$="DateOfBirth"]'
        );


    if (!field) {
        return;
    }


    /*
     * Prevent future dates.
     */

    const today =
        new Date();


    const year =
        today.getFullYear();


    const month =
        String(
            today.getMonth() + 1
        ).padStart(
            2,
            "0"
        );


    const day =
        String(
            today.getDate()
        ).padStart(
            2,
            "0"
        );


    field.max =
        year +
        "-" +
        month +
        "-" +
        day;

}


/* =========================================================
   TEXT FIELDS
   ========================================================= */

function setupTextFields() {

    const fields =
        document.querySelectorAll(
            '[id$="FirstName"],' +
            '[id$="LastName"],' +
            '[id$="City"],' +
            '[id$="EmergencyContactName"],' +
            '[id$="EmergencyRelationship"]'
        );


    fields.forEach(
        function (field) {

            field.addEventListener(
                "input",
                function () {

                    /*
                     * Prevent accidental leading spaces.
                     */

                    if (
                        this.value.length > 0 &&
                        this.value.charAt(0) === " "
                    ) {

                        this.value =
                            this.value.trimStart();
                    }

                }
            );

        }
    );

}


/* =========================================================
   CLEAR BUTTON
   ========================================================= */

function setupClearButton() {

    const button =
        document.querySelector(
            '[id$="ClearButton"]'
        );


    if (!button) {
        return;
    }


    button.addEventListener(
        "click",
        function () {

            /*
             * ASP.NET handles the actual
             * clearing through ClearForm_Click.
             *
             * This only removes any browser-side
             * validation styling.
             */

            const invalidFields =
                document.querySelectorAll(
                    ".input-error"
                );


            invalidFields.forEach(
                function (field) {

                    field.classList.remove(
                        "input-error"
                    );

                }
            );

        }
    );

}


/* =========================================================
   MEMBER ID VALIDATION
   ========================================================= */

function isValidMemberID(value) {

    return (
        /^\d{6}$/.test(value) ||
        /^\d{9}$/.test(value)
    );

}


/* =========================================================
   EMAIL VALIDATION
   ========================================================= */

function isValidEmail(value) {

    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/
        .test(value);

}


/* =========================================================
   PHONE VALIDATION
   ========================================================= */

function isValidPhone(value) {

    return /^\+?[0-9\s-]{10,15}$/
        .test(value);

}