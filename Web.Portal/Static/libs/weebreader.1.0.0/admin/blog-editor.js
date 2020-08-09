$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");

    $(".checkbox").checkbox();
    $("#blog").addClass("active");

    $(".ui.calendar").calendar({
        ampm: false,
        formatter: {
            date: (date) => getDate(date),
            time: (date) => getTime(date)
        }
    });

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.on("submit", () => tinyMCE.triggerSave()).form({
        on: "blur",
        fields: {
            title: {
                identifier: "title",
                rules: [
                    {
                        type: "empty",
                        prompt: postMustHaveTitleLabel
                    },
                    {
                        type: "maxLength[100]",
                        prompt: postTitleMaxLengthLabel
                    }
                ]
            },
            content: {
                identifier: "content",
                rules: [
                    {
                        type   : "empty",
                        prompt : postMustHaveContentLabel
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
});