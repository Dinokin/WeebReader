using System;

namespace WeebReader.Web.API.Settings
{
    public class ProxyConfiguration
    {
        public bool Enabled { get; set; }
        public string[] TrustedIps { get; set; } = Array.Empty<string>();
    }
}