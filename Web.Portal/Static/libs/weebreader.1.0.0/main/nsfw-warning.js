$(() => {
    $("#titles,#titles-mobile").addClass("active");

    $("#nsfw-confirm").on("click", () => {
        setCookie("seek_nsfw_content", "true", 0);
        location.reload();
    });
});