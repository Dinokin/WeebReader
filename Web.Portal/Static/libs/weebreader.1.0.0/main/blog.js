$(() => {
    let postContainer = $("#post-container");
    let loader = $("#loader");

    $("#blog,#blog-mobile").addClass("active")

    postContainer.visibility({
        once: false,
        observeChanges: true,
        onBottomVisible: () => loadData()
    });

    function loadData() {
        if (currentPage >= totalPages)
            return;

        currentPage++;

        $.ajax({
            url: pageRequestRoute.replace("/0", `/${currentPage}`),
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
            postContainer.append(data);
        });
    }
});