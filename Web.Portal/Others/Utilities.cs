using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Directory = WeebReader.Utilities.Common.Directory;

namespace WeebReader.Web.Portal.Others
{
    internal static class Utilities
    {
        public static X509Certificate2 GetCertificate()
        {
            const string certName = "certificate.pfx";
            
            if (!File.Exists($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}"))
            {
                var certificate = new CertificateRequest("cn=WeebReader", RSA.Create(2048), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1)
                    .CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                File.WriteAllBytes($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx));
            }

            return new X509Certificate2($"{Directory.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}");
        }

        public static string Encode(this string source) => source == null ? string.Empty : WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(source));

        public static string Decode(this string source) => source == null ? string.Empty : Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(source));
    }
}