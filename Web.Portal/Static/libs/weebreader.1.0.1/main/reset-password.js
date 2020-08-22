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
            newPassword: {
                identifier: "newPassword",
                rules: [
                    {
                        type: "empty",
                        prompt: newPasswordRequiredLabel
                    },
                    {
                        type: "minLength[8]",
                        prompt: minimumPasswordLengthLabel
                    }
                ]
            },
            repeatNewPassword: {
                identifier: "repeatNewPassword",
                rules: [
                    {
                        type: "match[newPassword]",
                        prompt: passwordsMustBeEqualLabel
                    }
                ]
            }
        }
    }).api({
        url: actionRoute,
        serializeForm: true,
        method: "PATCH",
        onSuccess: response => window.location.href = response.destination,
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });
});