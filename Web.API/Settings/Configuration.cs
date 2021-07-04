namespace WeebReader.Web.API.Settings
{
    public class Configuration
    {
        public string IpAddress { get; set; } = string.Empty;
        public ushort HttpPort { get; set; }
        public bool UseHttps { get; set; }
        public ushort HttpsPort { get; set; }
        public MariaDb Database { get; set; } = new();
        public Nginx Nginx { get; set; } = new();
    }
}