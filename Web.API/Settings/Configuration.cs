using System;

namespace WeebReader.Web.API.Settings
{
    public class Configuration
    {
        public string IpAddress { get; set; } = string.Empty;
        public ushort HttpPort { get; set; }
        public bool UseHttps { get; set; }
        public ushort HttpsPort { get; set; }
        public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
        public bool ServeStaticFiles { get; set; }
        public MariaDbConfiguration Database { get; set; } = new();
        public ProxyConfiguration Proxy { get; set; } = new();
        public EmailConfiguration Email { get; set; } = new();
    }
}