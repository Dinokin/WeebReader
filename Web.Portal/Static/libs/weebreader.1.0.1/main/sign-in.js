$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

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
                    }
                ]
            },
            password: {
                identifier: "password",
                rules: [
                    {
                        type: "empty",
                        prompt: passwordRequiredLabel
                    },
                    {
                        type: "minLength[8]",
                        prompt: minimumPasswordLengthLabel
                    }
                ]
            }
        }
    }).api({
        url: actionRoute,
        serializeForm: true,
        method: "POST",
        onSuccess: response => window.location.href = response.destination,
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });
});