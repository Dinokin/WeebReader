$(() => {
    let messageContainer = $(".form .message");
    let form = $(".form");
    
    $("#parameters").addClass("active");
    $("#parameters-content").addClass("active");
    $("#parameters-ratelimit").addClass("active");
    $(".checkbox").checkbox();
    $("#api-interval,#content-interval").dropdown();

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.form({
        on: "blur",
        fields: {
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