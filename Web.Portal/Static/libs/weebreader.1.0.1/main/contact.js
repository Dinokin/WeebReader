$(() =>{
    let messageContainer = $(".form .message");
    let form = $(".form");

    $("#contact,#contact-mobile").addClass("active");

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.on("submit", () => tinyMCE.triggerSave()).form({
        on: "blur",
        fields: {
            nickname: {
                identifier: "nickname",
                rules: [
                    {
                        type: "empty",
                        prompt: nicknameRequired
                    }
                ]
            },
            email: {
                identifier: "email",
                rules: [
                    {
                        type   : "empty",
                        prompt : emailRequired
                    },
                    {
                        type: "email",
                        prompt: validEmailRequired
                    }
                ]
            },
            message: {
                identifier: "message",
                rules: [
                    {
                        type: "empty",
                        prompt: messageRequired
                    }
                ]
            }
        }
    }).api({
        url: requestRoute,
        serializeForm: true,
        method: "POST",
        onSuccess: () => window.location.reload(),
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailed])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailed]))
    });
});