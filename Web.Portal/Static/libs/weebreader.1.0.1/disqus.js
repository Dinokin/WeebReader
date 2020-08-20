var disqus_config = function () {
    this.page.url = disqusPageUrl;
    this.page.identifier = disqusIdentifier;
    this.page.title = disqusTitle;
};

(function() {
    var d = document, s = d.createElement("script");
    s.src = `https://${disqusShortname}.disqus.com/embed.js`;
    s.setAttribute("data-timestamp", +new Date());
    (d.head || d.body).appendChild(s);
})();