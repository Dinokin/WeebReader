$(() => {
    let form = $(".form");
    let messageContainer = $(".form .message");

    form.on("submit", () => tinyMCE.triggerSave()).form({
        on: "blur",
        fields: {
            name: {
                identifier: "name",
                rules: [
                    {
                        type: "maxLength[100]",
                        prompt: chapterNameMaxLengthLabel
                    }
                ]
            }
        }
    }).api({
        url: actionRoute,
        method: method,
        processData: false,
        contentType: false,
        beforeSend: (settings) => {
            settings.data = new FormData(form.get(0));

            return settings;
        },
        onSuccess: (response) => window.location.href = response.destination,
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });
});