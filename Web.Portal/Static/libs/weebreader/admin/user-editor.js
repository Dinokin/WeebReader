$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

    $("#users").addClass("active");

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.form({
        on: "blur",
        fields: {
            username: {
                identifier: "username",
                rules: [
                    {
                        type: "empty",
                        prompt: usernameRequiredLabel
                    },
                    {
                        type: "minLength[3]",
                        prompt: minimumUsernameLengthLabel
                    },
                    {
                        type: "maxLength[50]",
                        prompt: maxUsernameLengthLabel
                    }
                ]
            },
            email: {
                identifier: "email",
                rules: [
                    {
                        type: "empty",
                        prompt: emailRequiredLabel
                    },
                    {
                        type: "email",
                        prompt: validEmailRequiredLabel
                    }
                ]
            },
            role: {
                identifier  : "role",
                rules: [
                    {
                        type   : "empty",
                        prompt : roleRequiredLabel
                    }
                ]
            },
            password: {
                identifier: "password",
                optional: true,
                rules: [
                    {
                        type: "minLength[8]",
                        prompt: minimumPasswordLengthLabel
                    }
                ]
            },
            repeatNewPassword: {
                identifier: "repeatPassword",
                rules: [
                    {
                        type: "match[password]",
                        prompt: passwordsMustBeEqualLabel
                    }
                ]
            }
        }
    }).api({
        url: actionRoute,
        serializeForm: true,
        method: method,
        onSuccess: response => window.location.href = response.destination,
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });

    $(".dropdown").dropdown();
});