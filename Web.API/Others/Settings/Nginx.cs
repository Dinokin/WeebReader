using System;

namespace WeebReader.Web.API.Others.Settings
{
    public class Nginx
    {
        public bool Enabled { get; set; }
        public string[] TrustedIps { get; set; } = Array.Empty<string>();
    }
}