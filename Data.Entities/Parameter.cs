using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Parameter : BaseEntity
    {
        public enum Types
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
        
        public Types Type { get; }
        public string? Value { get; set; }

        public Parameter(Types type, string? value) : this (default, type, value) { }

        public Parameter(Guid id, Types type, string? value) : base(id)
        {
            Type = type;
            Value = value;
        }
    }
}