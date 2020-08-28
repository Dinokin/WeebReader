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