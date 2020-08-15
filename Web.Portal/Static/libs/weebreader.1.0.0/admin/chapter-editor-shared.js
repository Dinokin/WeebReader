$(() => {
    $(".checkbox").checkbox();
    $("#titles").addClass("active");

    $(".ui.calendar").calendar({
        ampm: false,
        formatter: {
            date: (date) => getDate(date),
            time: (date) => getTime(date)
        }
    });

    $(".form").on("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            return false;
        }
    });
});