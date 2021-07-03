using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace WeebReader.Web.API.Others.Utilities
{
    public static class Security
    {
        private static X509Certificate2? _certificate;
        
        public static X509Certificate2 Certificate
        {
            get
            {
                if (_certificate != null)
                    return _certificate;
                
                const string certName = "certificate.pfx";

                if (!File.Exists($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}"))
                {
                    var certificate = new CertificateRequest("cn=WeebReader", RSA.Create(2048), HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1)
                        .CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);

                    File.WriteAllBytes($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}", certificate.Export(X509ContentType.Pfx));
                }

                _certificate =  new($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}{certName}");

                return _certificate;
            }
        }

        public static readonly DirectoryInfo KeysDirectory = Directory.CreateDirectory($"{Location.CurrentDirectory}{Path.DirectorySeparatorChar}Keys");
    }
}