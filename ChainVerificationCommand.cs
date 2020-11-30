using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace certificate_tools
{
    [Description("Verify certificate chain")]
    public class ChainVerificationCommand
    {
        [Description("Certificate file")]
        [Name("certificate")]
        [Required]
        public FileInfo Certificate { get; set; }

        [Description("Directory that contains root certificates")]
        [Name("ca")]
        [Required]
        public DirectoryInfo Ca { get; set; }

        public bool IsValid()
        {
            try
            {
                var certificateCollection = PemTools.LoadFromPemFile(Certificate);
                var certificate = certificateCollection.First();
                if (certificate == null)
                {
                    throw new ArgumentException("Unable to load certificate");
                }

                var intermediateCertificates = certificateCollection.Skip(1);

                X509CertificateCollection rootCertificates = new X509CertificateCollection();
                if (Ca != null)
                {
                    var files = Ca.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
                        .ToList();
                    if (files.Count > 2000)
                    {
                        throw new ArgumentException("Root directory cannot contain more than 2000 files");
                    }

                    foreach (var file in files)
                    {
                        if (file.Extension is ".crt" or ".pem")
                        {
                            var certificates = PemTools.LoadFromPemFile(file);
                            certificates.OfType<X509Certificate2>().Where(x=>x.Issuer==x.Subject).ToList().ForEach(x => rootCertificates.Add(x));
                        }
                    }
                }
                var rootsLookup = rootCertificates
                    .OfType<X509Certificate2>()
                    .GroupBy(x => x.Thumbprint)
                    .Select(x => x.First());


                X509Chain chain2 = new X509Chain
                {
                    ChainPolicy =
                    {
                        TrustMode = X509ChainTrustMode.CustomRootTrust,
                        RevocationMode = X509RevocationMode.NoCheck,
                        VerificationFlags = X509VerificationFlags.NoFlag
                    }
                };

                chain2.ChainPolicy.CustomTrustStore.AddRange(rootsLookup.ToArray());

                var isValid = chain2.Build(certificate);
                return isValid;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Unable to load certificate from file");
            }
        }
    }
}
