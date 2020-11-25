using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    [Description("Verify that key and certificate matches")]
    public class MatchKeyAndCertificate
    {
        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public string Certificate { get; set; }

        [Description("Key file")]
        [Name("key")]
        [Required]
        public string Key { get; set; }

        public bool AreValid()
        {
            X509Certificate2 certificate = LoadPemFile(File.ReadAllText(Certificate))[0];
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(Key));
            certificate.CopyWithPrivateKey(rsa);
            return true;
        }

        public X509Certificate2Collection LoadPemFile(ReadOnlySpan<char> data)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.ImportFromPem(data);
            return certificateCollection;
        }
    }
}
