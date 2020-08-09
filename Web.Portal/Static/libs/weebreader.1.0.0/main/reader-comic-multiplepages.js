$(() => {
    let pageSelector = $("#page-selector");
    let chapterContainer = $("#chapter-container");
    let pageContainer = $("#page-container");
    let page = $("#page");
    let currentPage = 0;

    pageSelector.dropdown({
        onChange: (value) => {
            currentPage = Number(value);
            setPage(value);
            previousPagePreload();
            nextPagePreload();
        }
    });

    pageContainer.dimmer({
        closable: false,
        displayLoader: true
    });

    chapterContainer.on("click", (event) => {
        let pageWidth = chapterContainer.innerWidth();
        let pageOffset = chapterContainer.offset();
        let clickLocation = event.pageX - pageOffset.left;

        if (pageWidth / 2 > clickLocation) {
            goPreviousPage();
        } else {
            goNextPage();
        }
    });


    page.on("load", () => pageContainer.dimmer("hide"));

    setPage();

    function setPage() {
        pageContainer.dimmer("show");
        page.attr("src", chapterPages[currentPage]);
        pageSelector.dropdown("set selected", [currentPage]);
    }

    function goNextPage() {
        if (currentPage + 1 >= chapterPages.length) {
            if (nextChapterRoute.length > 0) {
                window.location.href = nextChapterRoute;
            } else {
                $("body").toast({
                    class: "error",
                    message: noNextChapterLabel
                });
            }

            return;
        }

        currentPage++;
        setPage();
        scrollToTop(chapterContainer);
        nextPagePreload();
    }

    function goPreviousPage() {
        if (currentPage - 1 < 0) {
            if (previousChapterRoute.length > 0) {
                window.location.href = previousChapterRoute;
            } else {
                $("body").toast({
                    class: "error",
                    message: noPreviousChapterLabel
                });
            }

            return;
        }

        currentPage--;
        setPage();
        scrollToTop(chapterContainer);
        previousPagePreload();
    }

    function nextPagePreload() {
        for (let i = 1; i <= 2; i++) {
            if (currentPage + i < chapterPages.length) {
                new Image().src = chapterPages[currentPage + i];
            }
        }
    }

    function previousPagePreload() {
        for (let i = 1; i <= 2; i++) {
            if (currentPage - i >= 0) {
                new Image().src = chapterPages[currentPage - i];
            }
        }
    }
});