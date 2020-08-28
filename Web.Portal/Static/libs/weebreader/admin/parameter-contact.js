$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

    $("#parameters").addClass("active");
    $("#parameters-content").addClass("active");
    $("#parameters-contact").addClass("active");
    $(".checkbox").checkbox();

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.on("submit", () => tinyMCE.triggerSave()).form({
        on: "blur",
        fields: {
            recaptchaClientKey: {
                identifier: "recaptchaClientKey",
                depends: "recaptchaEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : reCaptchaClientKeyRequiredLabel
                    }
                ]
            },
            reCaptchaServerKey: {
                identifier: "reCaptchaServerKey",
                depends: "recaptchaEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : reCaptchaServerKeyRequiredLabel
                    }
                ]
            },
            discordLink: {
                identifier: "discordLink",
                depends: "contactDiscordEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : discordLinkRequiredLabel
                    },
                    {
                        type: "url",
                        prompt: discordLinkValidUrlLabel
                    }
                ]
            }
        }
    }).api({
        url: patchRoute,
        serializeForm: true,
        method: "PATCH",
        onSuccess: () => location.reload(),
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });
});