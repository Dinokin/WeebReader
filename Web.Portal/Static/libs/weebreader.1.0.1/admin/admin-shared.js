$(() => {
    $(".ui.sidebar").sidebar("attach events", "#toggler").accordion();

    $(".message .close").on("click", function() {
        $(this).closest(".message").transition("fade");
    });
});