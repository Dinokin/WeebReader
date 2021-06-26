$(() => {
    let form = $(".form");
    let messageContainer = $(".form .message");

    $("#parameters").addClass("active");
    $("#parameters-content").addClass("active");
    $("#parameters-pages").addClass("active");
    $(".checkbox").checkbox();

    form.on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });

    form.on("submit").form().api({
        url: patchRoute,
        serializeForm: true,
        method: "PATCH",
        onSuccess: () => location.reload(),
        onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
        onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
    });
});