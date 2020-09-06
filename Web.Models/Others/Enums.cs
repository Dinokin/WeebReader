namespace WeebReader.Web.Models.Others
{
    public enum ParameterTypes : ushort
    {
        SiteName = 0,
        SiteDescription = 1,
        SiteEmail = 3,
        EmailSenderEnabled = 4,
        SmtpServer = 5,
        SmtpServerPort = 6,
        SmtpServerUser = 7,
        SmtpServerPassword = 8,
        ContactEmailEnabled = 9,
        ContactEmailRecaptchaEnabled = 10,
        ContactEmailRecaptchaClientKey = 11,
        ContactEmailRecaptchaServerKey = 12,
        ContactDiscordEnabled = 13,
        ContactDiscordLink = 14,
        ContactDiscordNotice = 15,
        PageAboutContent = 16,
        PageAboutPatreonEnabled = 17,
        PageAboutPatreonLink = 18,
        PageAboutPatreonNotice = 19,
        PageAboutKofiEnabled = 20,
        PageAboutKofiLink = 21,
        PageAboutKofiNotice = 22,
        PageAboutEnabled = 23,
        PageDisqusEnabled = 24,
        PageDisqusShortname = 25,
        PageBlogEnabled = 26,
        SiteGoogleAnalyticsEnabled = 27,
        SiteGoogleAnalyticsCode = 28
    }
}