$(() => {
    let chapterContainer = $("#chapter-container");

    chapterContainer.api({
        url: chapterContentRoute,
        method: "GET",
        on: "now",
        onSuccess: (response) => buildChapterContainer(response.pages),
        onFailure: response => Array.isArray(response.messages) ? showErrorToast(response.messages[0]) : showErrorToast(requestFailedLabel),
        onError: () => showErrorToast(requestFailedLabel)
    });
    
    function buildChapterContainer(pageList) {
        let skeletonPage = $("#skeleton-page");
        let skeletonDivider = $("#skeleton-divider");
        
        pageList.forEach(page => {
            skeletonPage.clone().appendTo(chapterContainer).removeAttr("id").attr("data-src", page.address);
            
            if (!longStrip && pageList[pageList.length - 1].number !== page.number) {
                skeletonDivider.clone().appendTo(chapterContainer).removeAttr("id");
            }
        });
        
        skeletonPage.remove();
        
        if (!longStrip) 
            skeletonDivider.remove();

        chapterContainer.children().visibility({
            type: "image",
            transition: "fade in",
            duration: 500,
            offset: 500
        });
    }
});