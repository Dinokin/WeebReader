namespace WeebReader.Web.Portal.Others
{
    public static class Constants
    {
        public const ushort ItemsPerPageBlogAdmin = 25;
        public const ushort ItemsPerPageTitleAdmin = 25;
        public const ushort ItemsPerPageChapterAdmin = 25;
        public const ushort ItemsPerPageUserAdmin = 25;
        public const ushort ItemsPerPageReleases = 8;
        public const ushort ItemsPerPageReleasesRss = 16;
        public const ushort ItemsPerPagePosts = 8;
        public const ushort ItemsPerPageChapters = 25;
        public const ushort ItemsPerPageChaptersRss = 16;

        public const double RateLimitDefaultRequestLimit = 20;
        public const string RateLimitDefaultTimeInterval = "s";
        public const string RateLimitEndpointContent = "*:/content/*";
        public const string RateLimitEndpointContentJson = "*:*/json";
        public const string RateLimitEndpointApi = "*:/api/*";
    }
}