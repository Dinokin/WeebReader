$(() => {
    let jumpButton = $("#jump-button");

    $("#titles,#titles-mobile").addClass("active");
    $(".ui.dropdown").dropdown();

    $("#chapter-selector").dropdown({
        onChange: (value) => {
            if (value !== "default") {
                window.location.href = value;
            }
        },
        apiSettings: {
            url: chapterListRoute,
            saveRemoteData: false,
            cache: false,
            onResponse: (response) => {
                let array = [];

                response.chapters.forEach(currentValue => {
                    array.push({
                        name: chapterLabel + " " + currentValue.number,
                        value: currentValue.readAddress,
                        text: chapterLabel + " " + currentValue.number,
                        class: chapterNumber === currentValue.number ? "item active selected" : "item"
                    });
                });

                return {
                    success: response.success,
                    results: array
                }
            }
        }
    });

    $("#options").popup({
        popup: $("#popup"),
        on: "click"
    });

    $("#switch-theme").on("click", () => {
        if (getCookie("reader_theme") !== "light") {
            setCookie("reader_theme", "light", 3650);
        } else {
            setCookie("reader_theme", "dark", 3650);
        }

        location.reload();
    });

    jumpButton.on("click", () => {
        scrollToTop($("#chapter-container"));
    });

    $(document).on("scroll", () => {
        if ($(document).scrollTop() > 125) {
            jumpButton.removeClass("dp-none");
        } else {
            jumpButton.addClass("dp-none");
        }
    });
});