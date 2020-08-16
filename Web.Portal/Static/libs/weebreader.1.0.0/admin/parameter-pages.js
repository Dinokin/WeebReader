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

    form.on("submit", () => tinyMCE.triggerSave()).form({
        on: "blur",
        fields: {
            aboutContent: {
                identifier: "aboutContent",
                depends: "aboutEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : aboutPageContentRequiredLabel
                    }
                ]
            },
            patreonLink: {
                identifier: "patreonLink",
                depends: "patreonEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : patreonLinkRequiredLabel
                    },
                    {
                        type: "url",
                        prompt: patreonLinkValidUrlLabel
                    }
                ]
            },
            kofiLink: {
                identifier: "kofiLink",
                depends: "kofiEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : koFiLinkRequiredLabel
                    },
                    {
                        type: "url",
                        prompt: kofiLinkValidUrlLabel
                    }
                ]
            },
            disqusShortname: {
                identifier: "disqusShortname",
                depends: "disqusEnabled",
                rules: [
                    {
                        type   : "empty",
                        prompt : disqusShortnameRequiredLabel
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