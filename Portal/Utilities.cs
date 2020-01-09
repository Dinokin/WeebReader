using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace WeebReader.Web.Portal
{
    internal static class Utilities
    {
        public static DirectoryInfo CurrentDirectory => new FileInfo(Uri.UnescapeDataString(new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath)).Directory;

        public static X509Certificate2 GetCertificate()
        {
            const string certName = "certificate.pfx";
            
            if (!File.Exists($"{CurrentDirectory}{Path.DirectorySeparatorChar}{certName}"))
            {
                var certificate = new CertificateRequest("cn=WeebReader", RSA.Create(2048), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1)
                    .CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                File.WriteAllBytes($"{CurrentDirectory}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx));
            }

            return new X509Certificate2($"{CurrentDirectory}{Path.DirectorySeparatorChar}{certName}");
        }
    }
}