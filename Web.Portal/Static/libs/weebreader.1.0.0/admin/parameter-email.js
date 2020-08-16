$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

    $("#parameters").addClass("active");
    $("#parameters-content").addClass("active");
    $("#parameters-email").addClass("active");
    $(".checkbox").checkbox();

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.form({
        on: "blur",
        fields: {
            siteEmail: {
                identifier: "siteEmail",
                depends: "emailEnabled",
                rules: [
                    {
                        type: "empty",
                        prompt: siteEmailRequiredLabel
                    },
                    {
                        type: "email",
                        prompt: validEmailRequiredSiteEmailLabel
                    }
                ]
            },
            smtpServer: {
                identifier: "smtpServer",
                depends: "emailEnabled",
                rules: [
                    {
                        type: "empty",
                        prompt: smtpServerRequiredLabel
                    }
                ]
            },
            smtpServerPort: {
                identifier: "smtpServerPort",
                depends: "emailEnabled",
                rules: [
                    {
                        type: "empty",
                        prompt: smtpServerPortRequiredLabel
                    },
                    {
                        type: "integer[0,65535]",
                        prompt: smtpServerPortOutOfRangeLabel
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