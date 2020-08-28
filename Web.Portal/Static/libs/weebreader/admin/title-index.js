$(() => {
    let deletionTarget;
    let creationType;
    let deletionModal = $("#deletion-modal");
    let creationModal = $("#creation-modal");
    let messageContainer = $(".message");

    $("#titles").addClass("active");

    creationModal.modal({
        onApprove: () => window.location.href = `${creationRoute}?type=${creationType}`
    });

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

    $(".ui.dropdown").dropdown({
        onChange: (value) => {creationType = value; console.log(creationType)}
    });

    $("#add-title").on("click", () => creationModal.modal("show"));

    $(".negative.button").on("click", (event) => {
        if ($(event.target).is("i"))
            deletionTarget = $(event.target).parent().attr("title-id");
        else
            deletionTarget = $(event.target).attr("title-id");

        deletionModal.modal("show");
    });
});