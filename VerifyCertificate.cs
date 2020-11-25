using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    [Description("Verify certificate")]
    public class VerifyCertificate
    {
        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public string Certificate { get; set; }

        public bool IsValid()
        {
            try
            {
                var loadedCertificates = File.ReadAllText(Certificate);
                X509Certificate2 certificate = LoadPemFile(loadedCertificates)[0];
                return certificate.Verify();
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Unable to load certificate from file");
            }
        }

        public X509Certificate2Collection LoadPemFile(ReadOnlySpan<char> data)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.ImportFromPem(data);
            return certificateCollection;
        }
    }
}
