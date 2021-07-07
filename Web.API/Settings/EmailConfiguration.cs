namespace WeebReader.Web.API.Settings
{
    public class EmailConfiguration
    {
        public bool Enabled { get; set; }
        public string SmtpServerAddress { get; set; } = string.Empty;
        public ushort SmtpServerPort { get; set; }
        public string SmtpServerUser { get; set; } = string.Empty;
        public string SmtpServerPassword { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmailAddress { get; set; } = string.Empty;
    }
}