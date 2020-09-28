$(() => {
    let pageSelector = $("#page-selector");
    let chapterContainer = $("#chapter-container");
    let pageContainer = $("#page-container");
    let page = $("#page");
    let currentPage = 0;
    let pageList;

    pageContainer.dimmer({
        closable: false,
        displayLoader: true
    });
    
    page.on("load", () => pageContainer.dimmer("hide"));

    chapterContainer.api({
        url: chapterContentRoute,
        method: "GET",
        on: "now",
        onSuccess: (response) => {
            pageList = response.pages;

            pageSelector.dropdown({
                values: pageList.map(item => createDropdownPage(item)),
                onChange: (value) => {
                    currentPage = Number(value);
                    setPage();
                    previousPagePreload();
                    nextPagePreload();
                }
            });

            setPage();
        },
        onFailure: response => Array.isArray(response.messages) ? showErrorToast(response.messages[0]) : showErrorToast(requestFailedLabel),
        onError: () => showErrorToast(requestFailedLabel)
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

    function setPage() {
        pageContainer.dimmer("show");
        page.attr("src", pageList[currentPage].address);
        pageSelector.dropdown("set selected", [currentPage]);
    }

    function goNextPage() {
        if (currentPage + 1 >= pageList.length) {
            if (nextChapterRoute.length > 0) {
                window.location.href = nextChapterRoute;
            } else {
                showErrorToast(noNextChapterLabel);
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
                showErrorToast(noPreviousChapterLabel);
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
            if (currentPage + i < pageList.length) {
                new Image().src = pageList[currentPage + i].address;
            }
        }
    }

    function previousPagePreload() {
        for (let i = 1; i <= 2; i++) {
            if (currentPage - i >= 0) {
                new Image().src = pageList[currentPage - i].address;
            }
        }
    }
    
    function createDropdownPage(page){
        return {
            name: `${pageLabel} ${page.number + 1}`,
            value: page.number,
            selected: page.number === 0
        }
    }
});