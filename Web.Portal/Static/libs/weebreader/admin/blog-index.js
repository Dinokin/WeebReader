$(() => {
    let deletionTarget;
    let deletionModal = $(".modal");
    let messageContainer = $(".message");

    $("#blog").addClass("active");

    deletionModal.modal({
        closable: false,
        onDeny: () => deletionTarget = null,
        onApprove: () => {
            deletionModal.api({
                on: "now",
                url: deletionRoute + deletionTarget,
                method: "DELETE",
                onSuccess: () => location.reload(),
                onFailure: response => Array.isArray(response.messages) ? addError(messageContainer, response) : addError(messageContainer, buildFakeResponse([requestFailedLabel])),
                onError: () => addError(messageContainer, buildFakeResponse([requestFailedLabel]))
            })
        }
    });

    $(".negative.button").on("click", (event) => {
        if ($(event.target).is("i"))
            deletionTarget = $(event.target).parent().attr("post-id");
        else
            deletionTarget = $(event.target).attr("post-id");

        deletionModal.modal("show");
    });
});