namespace WeebReader.Web.API.Others.Settings
{
    public class Configuration
    {
        public string IpAddress { get; set; } = string.Empty;
        public ushort HttpPort { get; set; }
        public bool UseHttps { get; set; }
        public ushort HttpsPort { get; set; }
        public MariaDB Database { get; set; } = new();
        public Nginx Nginx { get; set; } = new();
    }
}