using System.ComponentModel.DataAnnotations;
using WeebReader.Web.Localization;

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
        SiteGoogleAnalyticsCode = 28,
        RateLimitRealIpHeader = 29,
        RateLimitContentEnabled = 30,
        RateLimitApiEnabled = 31,
        RateLimitMaxContentRequests = 32,
        RateLimitMaxApiRequests = 33,
        RateLimitPeriodContent = 34,
        RateLimitPeriodApi = 35
    }

    public enum RateLimitPeriods : byte
    {
        [Display(ResourceType = typeof(Labels), Name = "Second")]
        Second = 0,
        [Display(ResourceType = typeof(Labels), Name = "Minute")]
        Minute = 1,
        [Display(ResourceType = typeof(Labels), Name = "Hour")]
        Hour = 2
    }
}