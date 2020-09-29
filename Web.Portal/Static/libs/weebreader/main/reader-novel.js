$(() => {
    $("#content").api({
        url: chapterContentRoute,
        method: "GET",
        on: "now",
        onSuccess: (response) => {
            $("#content").append(response.content);

            $("#chapter-container").find("img").visibility({
                type: "image",
                transition: "fade in",
                duration: 1000
            });
        },
        onFailure: response => Array.isArray(response.messages) ? showErrorToast(response.messages[0]) : showErrorToast(requestFailedLabel),
        onError: () => showErrorToast(requestFailedLabel)
    });
});