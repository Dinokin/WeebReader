using System;
using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Parameter : BaseEntity
    {
        public enum Types
        {
            SiteName,
            SiteDescription,
            SiteAddress,
            SiteEmail,
            EmailEnabled,
            SmtpServer,
            SmtpServerPort,
            SmtpServerUser,
            SmtpServerPassword,
            ContactEmailEnabled,
            ContactEmailRecaptchaEnabled,
            ContactEmailRecaptchaClientKey,
            ContactEmailRecaptchaServerKey,
            ContactDiscordEnabled,
            ContactDiscordLink,
            ContactDiscordMessage,
            PageBlogEnabled,
            PageSupportUsEnabled,
            PageSupportUsPatreonEnabled,
            PageSupportUsPatreonMessage,
            PageSupportUsKofiEnabled,
            PageSupportUsKofiMessage
        }
        
        public Types Type { get; }
        public string? Value { get; set; }

        public Parameter(Types type, string? value)
        {
            Type = type;
            Value = value;
        }

        public Parameter(Guid id, Types type, string? value) : base(id)
        {
            Type = type;
            Value = value;
        }
    }
}