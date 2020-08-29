$(() => {
    let form = $(".form");
    let messageContainer = $(".form .message");
    let progressBar = $(".indicating");
    let fileInput = $("#file");

    form.form({
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
        xhr: () => {
            let xhr = new window.XMLHttpRequest();
            xhr.upload.addEventListener("progress", (event) => animateProgressBar(progressBar, event), false);

            return xhr;
        },
        url: actionRoute,
        method: method,
        processData: false,
        contentType: false,
        beforeSend: (settings) => {
            settings.data = new FormData(form.get(0));

            return settings;
        },
        onRequest: () => {
            if (fileInput[0].files.length > 0)
                progressBar.removeClass("dp-none");
        },
        onComplete: () => {
            if (fileInput[0].files.length > 0)
                progressBar.addClass("dp-none");
        },
        onSuccess: (response) => window.location.href = response.destination,
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });

    fileInput.on("change", (event) => {
        $("label.button").html("<i class=\"file icon\"></i>" + event.target.files[0].name.substr(0, 14) + (event.target.files[0].name.length > 15 ? "..." : ""));
    });
});