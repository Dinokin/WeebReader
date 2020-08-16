$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

    $("#parameters").addClass("active");
    $("#parameters-content").addClass("active");
    $("#parameters-general").addClass("active");
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
            siteName: {
                identifier  : "siteName",
                rules: [
                    {
                        type   : "empty",
                        prompt : siteNameRequiredLabel
                    }
                ]
            },
            googleAnalyticsCode: {
                identifier: "googleAnalyticsCode",
                depends: "googleAnalyticsEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : googleAnalyticsCodeRequiredLabel
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