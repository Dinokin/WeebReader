$(() => {
    $("#switch-view-mode").on("click", () => {
        const cookieName = titleId + "_long_strip";

        if (getCookie(cookieName) === "true") {
            setCookie(cookieName, false, 3650);
        } else {
            setCookie(cookieName, true, 3650);
        }

        location.reload();
    });
}); 