using WeebReader.Data.Entities.Abstract;

namespace WeebReader.Data.Entities
{
    public class Setting : BaseEntity
    {
        public enum Keys
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
            BlogEnabled
        }
        
        public Keys Key { get; set; }
        public string Value { get; set; }
    }
}