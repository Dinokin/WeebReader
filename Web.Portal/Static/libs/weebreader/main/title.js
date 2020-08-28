$(() => {
    let chapterContainer = $("#chapters");
    let loader = $("#loader");

    $("#titles,#titles-mobile").addClass("active");
    $(".tabular.menu .item").tab();

    chapterContainer.visibility({
        once: false,
        observeChanges: true,
        onBottomVisible: () => loadData()
    });

    function loadData() {
        if (currentPage < totalPages) {
            currentPage++;

            $.ajax({
                url: pageRequestRoute.replace(new RegExp("\/0$"), `/${currentPage}`),
                method: "GET",
                cache: false,
                beforeSend: () => {
                    loader.removeClass("disabled");
                    loader.addClass("active");
                },
                complete: () => {
                    loader.removeClass("active");
                    loader.addClass("disabled");
                }
            }).done((data) => {
                chapterContainer.append(data);
            });
        }
        else
        {
            $("#previous-chapters").removeClass("dp-none");
        }
    }
});