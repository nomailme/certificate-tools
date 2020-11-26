using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    [Description("Verify certificate chain")]
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
                var certificateCollection = LoadPemFile(loadedCertificates);
                var certificate = certificateCollection[0];
                IEnumerable<X509Certificate2> intermediateCertificates =
                    GetIntermediateCertificates(certificateCollection);

                X509Chain chain2 = new X509Chain();
                chain2.ChainPolicy.ExtraStore.AddRange(intermediateCertificates.ToArray());
                // This setup does not have revocation information
                chain2.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                var isValid = chain2.Build(certificate);
                return isValid;
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

        private static IEnumerable<X509Certificate2> GetIntermediateCertificates(
            X509Certificate2Collection certificateCollection)
        {
            if (certificateCollection.Count > 1)
            {
                for (int i = 1; i < certificateCollection.Count - 1; i++)
                {
                    yield return certificateCollection[i];
                }
            }
        }
    }
}
