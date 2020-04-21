using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace WeebReader.Common.Utilities
{
    public static class Security
    {
        public static X509Certificate2 GetCertificate()
        {
            const string certName = "certificate.pfx";
            
            if (!File.Exists($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}"))
            {
                var certificate = new CertificateRequest("cn=WeebReader", RSA.Create(2048), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1)
                    .CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                File.WriteAllBytes($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx));
            }

            return new X509Certificate2($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}");
        }
    }
}